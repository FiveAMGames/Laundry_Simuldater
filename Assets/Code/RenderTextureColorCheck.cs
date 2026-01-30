using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderTextureColorCheck : MonoBehaviour
{
    public RenderTexture panoramaRT;
    public Camera textCamera;
    public List<Color> validColors = new List<Color>();

    void OnEnable()
    {

        if (panoramaRT != null)
        {
            textCamera.gameObject.SetActive(true);
            Invoke("SamplePanorama", 0.2f);
        }
    }

    private void SamplePanorama()
    {
        RenderTexture rt = panoramaRT;
        var tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        Color[] colors = tex.GetPixels();
        List<Color> goodColors = new List<Color>();
        List<Color> invalidColors = new List<Color>();
        Debug.Log(colors.Length);

        foreach (Color c in colors)
        {
            if (validColors.Contains(c))
            {
                goodColors.Add(c);
            }
            else invalidColors.Add(c);
        }
        Debug.Log("valid " + goodColors.Count + " invalid " + invalidColors.Count);
    }
}
