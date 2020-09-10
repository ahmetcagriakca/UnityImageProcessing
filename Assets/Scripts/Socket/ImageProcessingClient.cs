using System;
using System.Collections;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ImageProcessingClient : MonoBehaviour
{
    private ImageProcessingRequester _imageProcessingRequester;
    [Header("Images")]
    public RawImage SentImage;
    public RawImage ReceivedImage;
    public RawImage CameraImage;
    public RawImage ShowImage;
    [Header("Buttons")]
    public Button SendButton;
    public Button UploadButton;
    public Button CaptureButton;
    public Button ShowButton;
    public Text Errors;

    private int maxSize = 1024;

    //private static string host = "http://localhost:8255/";
    //private static string host = "http://7f5fc4467e53.ngrok.io/";
    private static string host = "http://35.234.76.109/";
    private string endpoint = $"{host}api/imageprocessing/";
    private void Start()
    {
        SendButton.onClick.AddListener(GetImage);
        UploadButton.onClick.AddListener(UploadImage);
        CaptureButton.onClick.AddListener(CaptureImage);
        //ShowButton.onClick.AddListener(ShowProcessedImage);
    }

    public void ShowError(string message)
    {
        Errors.gameObject.SetActive(true);
        Errors.text = message;
    }


    private void ShowProcessedImage()
    {
        ShowImage.gameObject.SetActive(true);
        ShowImage.texture = ReceivedImage.texture;
    }

    Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
    {
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;

        int iRotated, iOriginal;

        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }

        Texture2D rotatedTexture = new Texture2D(h, w);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }
    private void CaptureImage()
    {
        try
        {
            var texture = CameraImage.texture as Texture2D;
            var textureImageBase64String = texture.TextureImageToBase64String();

            byte[] decodedBytes = Convert.FromBase64String(textureImageBase64String);
            var capturedTexture = new Texture2D(2, 2);
            capturedTexture.LoadImage(decodedBytes); //..this will auto-resize the texture dimensions.
            //SentImage.texture = capturedTexture;
            SentImage.texture = rotateTexture(capturedTexture,true);
        }
        catch (Exception ex)
        {
            ShowError($"Getting Error: {ex.Message} StackTrace:{ex.StackTrace}");
        }
    }

    private void UploadImage()
    {
        try
        {
#if UNITY_EDITOR
            var loadImage = Path.Combine(Application.dataPath, "Images", "ai.jpg");
            var loadedPngFromFile = loadImage.LoadPngFromFile();
            //var textureImageBase64String = loadedPngFromFile.TextureImageToBase64String();

            SentImage.texture = loadedPngFromFile;
            //Texture2D texture = Selection.activeObject as Texture2D;
            //if (texture == null)
            //{
            //    EditorUtility.DisplayDialog("Select Texture", "You must select a texture first!", "OK");
            //    return;
            //}

            //string path = EditorUtility.OpenFilePanel("Overwrite with png", "", "png");
            //if (path.Length != 0)
            //{
            //    var fileContent = File.ReadAllBytes(path);
            //    texture.LoadImage(fileContent);
            //}
            //SentImage.texture = loadedPngFromFile;
#else

      
#endif
            NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
            {
                Debug.Log("Image path: " + path);
                if (path != null)
                {
                    // Create Texture from selected image
                    Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize,markTextureNonReadable:false);
                    if (texture == null)
                    {
                        Debug.Log("Couldn't load texture from " + path);
                        return;
                    }
                    SentImage.texture = texture;

                }
            }, "Select a PNG image", "image/png");
            ShowError("Permission result: " + permission);
            Debug.Log("Permission result: " + permission);

        }
        catch (Exception ex)
        {
            ShowError($"Getting Error: {ex.Message} StackTrace:{ex.StackTrace}");
        }
    }

    private void OnDestroy()
    {
    }

    public void GetImage()
    {
        try
        {
            //var loadImage = Path.Combine(Application.dataPath, "Images", "ai.jpg");
            //var loadedPngFromFile = loadImage.LoadPngFromFile();
            //var textureImageBase64String = loadedPngFromFile.TextureImageToBase64String();

            //Sent image showing on screen
            //SentImage.texture = loadedPngFromFile;
            var texture = SentImage.texture as Texture2D;
            var textureImageBase64String = texture.TextureImageToBase64String();

            _imageProcessingRequester = new ImageProcessingRequester();
            //StartCoroutine(GetImage("processed_tests.jpg"));
            StartCoroutine(UploadFile());
            //StartCoroutine(_imageProcessingRequester.Start(textureImageBase64String, ImageProcessingFinish));
        }
        catch (Exception ex)
        {
            ShowError($"Getting Error: {ex.Message} StackTrace:{ex.StackTrace}");
        }
    }

    public IEnumerator GetImage(string image_name)
    {
        string url = $"{endpoint}get_image?ImageName=" + image_name;


        using (UnityWebRequest request = new UnityWebRequest
        {
            url = url,
            method = "GET",
            downloadHandler = new DownloadHandlerBuffer(),
            timeout = 60
        })
        {
            LoadingManager.Instance.ShowLoading(true);
            yield return request.SendWebRequest();
            Debug.Log("Image complete!");
            ReceivedImage.texture = request.downloadHandler.data.LoadImageFromBase64();
            LoadingManager.Instance.ShowLoading(false);
        }
    }


    public IEnumerator UploadFile()
    {
        LoadingManager.Instance.ShowLoading(true);
        var texture = SentImage.texture as Texture2D;
        byte[] myData = texture.EncodeToPNG();
        //string url = "http://localhost:1532/upload_file";

        string url = $"{endpoint}upload_file";
        UnityWebRequest request = UnityWebRequest.Put(url, myData);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            var response = JsonConvert.DeserializeObject<ServiceResult<FileInfo>>(request.downloadHandler.text);

            Debug.Log("Upload complete!");
            yield return StartPrediction(response.result);
        }
        LoadingManager.Instance.ShowLoading(false);
    }

    public IEnumerator StartPrediction(FileInfo file)
    {
        LoadingManager.Instance.ShowLoading(true);
        string url = $"{endpoint}start_operation";
        string data = JsonConvert.SerializeObject(file);

        using (UnityWebRequest request = new UnityWebRequest
        {
            url = url,
            method = "POST",
            downloadHandler = new DownloadHandlerBuffer(),
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data)),
            timeout = 300
        })
        {

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                var response = JsonConvert.DeserializeObject<ServiceResult<ProcessInfo>>(request.downloadHandler.text);
                yield return GetImage(response.result.ProcessedImageName);
                Debug.Log("Operation complete!");
            }
        }
        LoadingManager.Instance.ShowLoading(false);
    }


    public void ImageProcessingFinish(string image)
    {
        ReceivedImage.texture = image.LoadImageFromBase64String();
    }

}

public class ServiceResult<Response>
{
    public bool isSuccess { get; set; }
    public Response result { get; set; }

}
public class FileInfo
{
    public string FileOid { get; set; }
}
public class ProcessInfo
{
    public string ProcessedImageName { get; set; }
}