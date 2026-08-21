using UnityEngine;

public class Scratch : MonoBehaviour
{
    public Camera mainCamera;
    public Camera scratchCamera;


    [Tooltip("Material used by the stain SpriteRenderers")]
    public Material stainMaterial;

    [Tooltip("1024 is usually plenty for a scratch mask")]
    public int textureSize = 1024;

    private RenderTexture scratchTexture;

    private static readonly int ScratchMaskID =
        Shader.PropertyToID("_ScratchMask");

    public Camera stainMaskCamera;

    private RenderTexture stainAreaTexture;

    public RenderTexture GetStainAreaTexture()
    {
        return stainAreaTexture;
    }



    private void Awake()
    {
        CreateRenderTexture();
    }

    private void OnEnable()
    {
        scratchCamera.transform.position = mainCamera.transform.position;
        scratchCamera.transform.rotation = mainCamera.transform.rotation;

        scratchCamera.orthographic = mainCamera.orthographic;
        scratchCamera.orthographicSize = mainCamera.orthographicSize;

        scratchCamera.aspect = mainCamera.aspect;
        scratchCamera.rect = mainCamera.rect;


        if (scratchTexture == null)
            CreateRenderTexture();

        Clear();
    }


    private void CreateRenderTexture()
    {
        ReleaseRenderTexture();

        scratchTexture = new RenderTexture(
            textureSize,
            textureSize,
            0,
            RenderTextureFormat.ARGB32
        );

        scratchTexture.name = "ScratchMask_RT";

        scratchTexture.wrapMode = TextureWrapMode.Clamp;
        scratchTexture.filterMode = FilterMode.Bilinear;

        scratchTexture.Create();

        scratchCamera.targetTexture = scratchTexture;

        stainMaterial.SetTexture(
            ScratchMaskID,
            scratchTexture
        );

        stainAreaTexture = new RenderTexture(
    textureSize,
    textureSize,
    0,
    RenderTextureFormat.ARGB32
);

        stainAreaTexture.name = "StainArea_RT";
        stainAreaTexture.wrapMode = TextureWrapMode.Clamp;
        stainAreaTexture.filterMode = FilterMode.Bilinear;
        stainAreaTexture.Create();

        stainMaskCamera.targetTexture = stainAreaTexture;
    }

    public void CaptureStainArea()
    {
        if (stainAreaTexture == null)
            return;

        RenderTexture previous = RenderTexture.active;

        RenderTexture.active = stainAreaTexture;
        GL.Clear(true, true, Color.black);

        stainMaskCamera.Render();

        RenderTexture.active = previous;
    }


    public void Clear()
    {
        if (scratchTexture == null)
            return;

        RenderTexture previous =
            RenderTexture.active;

        RenderTexture.active =
            scratchTexture;

        GL.Clear(
            true,
            true,
            Color.black
        );

        RenderTexture.active =
            previous;
    }


    private void ReleaseRenderTexture()
    {
        if (scratchTexture == null)
            return;

        if (scratchCamera != null &&
            scratchCamera.targetTexture == scratchTexture)
        {
            scratchCamera.targetTexture = null;
        }

        scratchTexture.Release();
        Destroy(scratchTexture);

        scratchTexture = null;

        if (stainAreaTexture != null)
        {
            if (stainMaskCamera.targetTexture == stainAreaTexture)
                stainMaskCamera.targetTexture = null;

            stainAreaTexture.Release();
            Destroy(stainAreaTexture);
            stainAreaTexture = null;
        }
    }


    private void OnDestroy()
    {
        ReleaseRenderTexture();
    }


    public RenderTexture GetScratchTexture()
    {
        return scratchTexture;
    }
}