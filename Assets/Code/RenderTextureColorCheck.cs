using UnityEngine;

public class RenderTextureColorCheck : MonoBehaviour
{
    public Scratch scratchScript;

    [Header("Completion")]
    [Range(0f, 1f)]
    public float requiredScratchPercent = 0.8f;

    [Range(0f, 1f)]
    public float scratchThreshold = 0.1f;

    [Header("Performance")]
    public int checkResolution = 256;


    public bool SamplePanorama()
    {
        RenderTexture scratchRT =
            scratchScript.GetScratchTexture();

        RenderTexture stainRT =
            scratchScript.GetStainAreaTexture();

        if (scratchRT == null || stainRT == null)
        {
            Debug.LogWarning("Scratch or stain mask missing.");
            return false;
        }


        RenderTexture smallScratch =
            RenderTexture.GetTemporary(
                checkResolution,
                checkResolution,
                0,
                RenderTextureFormat.ARGB32
            );

        RenderTexture smallStain =
            RenderTexture.GetTemporary(
                checkResolution,
                checkResolution,
                0,
                RenderTextureFormat.ARGB32
            );


        Graphics.Blit(scratchRT, smallScratch);
        Graphics.Blit(stainRT, smallStain);


        Texture2D scratchTexture =
            ReadTexture(smallScratch);

        Texture2D stainTexture =
            ReadTexture(smallStain);


        Color32[] scratchPixels =
            scratchTexture.GetPixels32();

        Color32[] stainPixels =
            stainTexture.GetPixels32();



        byte scratchLimit =
            (byte)(scratchThreshold * 255f);


        float totalStainWeight = 0f;
        float scratchedStainWeight = 0f;

        for (int i = 0; i < stainPixels.Length; i++)
        {
            // 0 = no stain, 1 = fully inside stain.
            float stainAmount = stainPixels[i].r / 255f;

            // 0 = untouched, 1 = fully scratched.
            float scratchAmount = scratchPixels[i].r / 255f;

            totalStainWeight += stainAmount;

            // Only scratching that overlaps the stain contributes.
            scratchedStainWeight += stainAmount * scratchAmount;
        }

        float scratchedPercent = 0f;

        if (totalStainWeight > 0f)
        {
            scratchedPercent =
                scratchedStainWeight /
                totalStainWeight;
        }

        Debug.Log(
            $"Stain scratched: {scratchedPercent:P1}"
        );

        return scratchedPercent >= requiredScratchPercent;
    }


    private Texture2D ReadTexture(
        RenderTexture rt)
    {
        RenderTexture previous =
            RenderTexture.active;

        RenderTexture.active = rt;

        Texture2D texture =
            new Texture2D(
                rt.width,
                rt.height,
                TextureFormat.RGB24,
                false
            );

        texture.ReadPixels(
            new Rect(
                0,
                0,
                rt.width,
                rt.height
            ),
            0,
            0,
            false
        );

        texture.Apply(false);

        RenderTexture.active =
            previous;

        return texture;
    }
}