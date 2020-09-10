using UnityEngine;
using UnityEngine.UI;
public class RawImg : MonoBehaviour
{
    public RawImage rawImg = null;
    public byte alpha = 255;
    public void Update()
    {
        Color color;
        color = new Color32(0, 0, 0, alpha);
        if (rawImg)
        {
            rawImg.color = new Color(rawImg.color.r, rawImg.color.g, rawImg.color.b, color.a);
            //rawImg.texture = rotateTexture(rawImg.texture as Texture2D, true);
        }
    }


    //Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
    //{
    //    Color32[] original = originalTexture.GetPixels32();
    //    Color32[] rotated = new Color32[original.Length];
    //    int w = originalTexture.width;
    //    int h = originalTexture.height;

    //    int iRotated, iOriginal;

    //    for (int j = 0; j < h; ++j)
    //    {
    //        for (int i = 0; i < w; ++i)
    //        {
    //            iRotated = (i + 1) * h - j - 1;
    //            iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
    //            rotated[iRotated] = original[iOriginal];
    //        }
    //    }

    //    Texture2D rotatedTexture = new Texture2D(h, w);
    //    rotatedTexture.SetPixels32(rotated);
    //    rotatedTexture.Apply();
    //    return rotatedTexture;
    //}
}