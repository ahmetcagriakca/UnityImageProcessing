using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ImageProcessingExtensions
{
    public static Texture2D LoadPngFromFile(this string filePath)
    {
        Texture2D texture2D = null;

        if (!File.Exists(filePath)) return texture2D;

        var fileData = File.ReadAllBytes(filePath);
        texture2D = new Texture2D(2, 2);
        texture2D.LoadImage(fileData); //..this will auto-resize the texture dimensions.

        return texture2D;
    }

    public static Texture2D LoadImageFromBase64String(this string base64ImageString)
    {
        var decodedBytes = Convert.FromBase64String(base64ImageString);
        var texture2D = new Texture2D(2, 2);
        texture2D.LoadImage(decodedBytes); //..this will auto-resize the texture dimensions.
        return texture2D;
    }
    public static Texture2D LoadImageFromBase64(this byte[] decodedBytes)
    {
        var texture2D = new Texture2D(2, 2);
        texture2D.LoadImage(decodedBytes); //..this will auto-resize the texture dimensions.
        return texture2D;
    }

    public static string TextureImageToBase64String(this Texture2D texture)
    {
        var base64String = Convert.ToBase64String(texture.EncodeToPNG());
        return base64String;
    }
}