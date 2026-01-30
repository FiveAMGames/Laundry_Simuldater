/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2021 Kurt Dekker/PLBM Games All rights reserved.

	http://www.twitter.com/kurtdekker

	Redistribution and use in source and binary forms, with or without
	modification, are permitted provided that the following conditions are
	met:

	Redistributions of source code must retain the above copyright notice,
	this list of conditions and the following disclaimer.

	Redistributions in binary form must reproduce the above copyright
	notice, this list of conditions and the following disclaimer in the
	documentation and/or other materials provided with the distribution.

	Neither the name of the Kurt Dekker/PLBM Games nor the names of its
	contributors may be used to endorse or promote products derived from
	this software without specific prior written permission.

	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
	IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
	TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
	PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
	HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
	SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
	TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
	PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
	LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
	NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
	SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scratchers2D : MonoBehaviour
{
	[Header( "Payload Texture (what to reveal)")]
	public Texture2D UnderlyingTexture;
	[Header( "Gray obscuration covering texture")]
	public Texture2D ObscuringTexture;

	[Header( "What our finger scratches upon.")]
	public Collider TouchableCollider;

	[Header( "Where we display our result.")]
	public Renderer FinalResultRenderer;

	[Header( "The stage where stuff is filmed.")]
	// what will film each chunk of work as we scratch
	public Camera StageCamera;
	// what will display each chunk of work as we scratch
	public Renderer StageRenderer;
	public MeshFilter StageMeshFilter;
	public RenderTexture RT;

	public Material UnlitMaterial;

	[Header( "Coordinates in 1x1 UV space.")]
	public float MinScratchStreakWidth = 0.025f;
	public float MaxScratchStreakWidth = 0.035f;
	public float ScratchMarkSpacing = 0.030f;

	Material UnderlyingMaterial;

	Material ResultMaterial;
	Mesh workMesh;

	Camera mainCam;

	void Start ()
	{
		mainCam = Camera.main;

		UnderlyingMaterial = new Material( UnlitMaterial);
		UnderlyingMaterial.mainTexture = UnderlyingTexture;

		ResultMaterial = new Material( UnlitMaterial);
		ResultMaterial.mainTexture = RT;
		FinalResultRenderer.material = ResultMaterial;

		workMesh = new Mesh();
	}

	void StrikeTriangle( Vector3 position1, Vector3 position2)
	{
		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<int> tris = new List<int>();

		// streak between the two points given
		int triangleCount = 1 + (int)(Vector3.Distance( position1, position2) / ScratchMarkSpacing);

		// strike each series of triangles from position1 to position2
		for (int point = 0; point < triangleCount; point++)
		{
			float fraction = (point + 1.0f) / triangleCount;

			Vector3 position = Vector3.Lerp( position1, position2, fraction);

			// each one can be a slightly different size
			float size = Random.Range( MinScratchStreakWidth, MaxScratchStreakWidth);

			// and each one is differently-rotated
			float angle = Random.Range( 0.0f, 360.0f);

			// make the verts going around this particular triangle
			for (int i = 0; i < 3; i++)
			{
				Quaternion rotation = Quaternion.Euler( 0, 0, angle);

				Vector3 pointPosition = rotation * Vector3.right * size;

				// done first so count is correct (+0, +1, +2)
				tris.Add( vertices.Count);

				Vector3 finalVertexPosition = position + pointPosition;

				vertices.Add( finalVertexPosition);

				// UV is same as vert, but offset half a unit
				uvs.Add( finalVertexPosition + Vector3.one / 2);

				// rotate clockwise to match winding order of triangles
				angle -= 120;
			}
		}

		workMesh.vertices = vertices.ToArray();
		workMesh.uv = uvs.ToArray();
		workMesh.triangles = tris.ToArray();

		workMesh.RecalculateBounds();
		workMesh.RecalculateNormals();

		StageMeshFilter.mesh = workMesh;
	}

	// We scratch from this position to the current position,
	// which mean swe need two consecutive good touches for
	// any rendering to happen.
	Vector3? PreviousPosition;

	void UpdateScratching()
	{
		Vector3? CurrentPosition = null;

		if (Input.GetMouseButton(0))
		{
			var ray = mainCam.ScreenPointToRay( Input.mousePosition);

			RaycastHit hitInfo;

			if (TouchableCollider.Raycast( ray, out hitInfo, 20))
			{
				Vector3 position = hitInfo.point;

				position = TouchableCollider.transform.InverseTransformPoint( position);

				CurrentPosition = position;

				if ((PreviousPosition != null) &&
					(CurrentPosition != null))
				{
					StrikeTriangle(
						(Vector3) PreviousPosition,
						(Vector3) CurrentPosition);
				}
			}
		}

		PreviousPosition = CurrentPosition;
	}

	// to ensure we don't scratch until the obscuration has
	// had a chance to render properly.
	float startupTimer;
	const float startupDelay = 0.1f;

	void Update ()
	{
		if (startupTimer > startupDelay)
		{
			// As soon as we're ready, wipe out the mesh utterly so that
			// we don't just give away the entire under surface immediately.
			workMesh.Clear();
			StageMeshFilter.mesh = workMesh;

			StageRenderer.material = UnderlyingMaterial;

			UpdateScratching();
		}

		startupTimer += Time.deltaTime;
	}
}
