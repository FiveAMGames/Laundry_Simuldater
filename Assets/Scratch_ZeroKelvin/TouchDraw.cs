using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDraw : MonoBehaviour
{
    private Coroutine drawing;

    public GameObject linePrefab;

    public List<GameObject> allTheLines =
        new List<GameObject>();

    public Camera mainCam;

    public static List<LineRenderer> drawnLineRenderers =
        new List<LineRenderer>();

    [Header("Drawing")]
    public float minimumPointDistance = 0.015f;


    private void OnEnable()
    {
        drawing = null;

        foreach (GameObject go in allTheLines)
        {
            if (go != null)
                Destroy(go);
        }

        allTheLines.Clear();

        // Important because this is static.
        drawnLineRenderers.Clear();
    }


    private void OnDisable()
    {
        if (drawing != null)
        {
            StopCoroutine(drawing);
            drawing = null;
        }

        Logic.Instance?.StopScrubbingSound();
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Logic.Instance?.PlayScrubbingSound();

            StartLine();
        }

        if (Input.GetMouseButtonUp(0))
        {
            FinishLine();

            Logic.Instance?.StopScrubbingSound();
        }
    }


    public void StartLine()
    {
        if (drawing != null)
        {
            StopCoroutine(drawing);
            drawing = null;
        }

        drawing =
            StartCoroutine(DrawLine());
    }


    public void FinishLine()
    {
        if (drawing != null)
        {
            StopCoroutine(drawing);
            drawing = null;
        }
    }


    private IEnumerator DrawLine()
    {
        GameObject newGameObject =
            Instantiate(
                linePrefab,
                Vector3.zero,
                Quaternion.identity
            );

        allTheLines.Add(newGameObject);

        LineRenderer line =
            newGameObject.GetComponent<LineRenderer>();

        drawnLineRenderers.Add(line);

        line.positionCount = 0;

        Vector3 previousPosition =
            new Vector3(
                float.MaxValue,
                float.MaxValue,
                0
            );

        while (true)
        {
            Vector3 position =
                mainCam.ScreenToWorldPoint(
                    Input.mousePosition
                );

            position.z = 0;

            if (
                line.positionCount == 0 ||
                Vector3.Distance(
                    position,
                    previousPosition
                ) >= minimumPointDistance
            )
            {
                int index =
                    line.positionCount;

                line.positionCount =
                    index + 1;

                line.SetPosition(
                    index,
                    position
                );

                previousPosition =
                    position;
            }

            yield return null;
        }
    }
}