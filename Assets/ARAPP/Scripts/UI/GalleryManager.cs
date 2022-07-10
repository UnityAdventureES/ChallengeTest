using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{

    [Header("Images Render")]
    public RawImage showImageGallery;
    public RawImage panel;
    [HideInInspector]
    public int currentIndex = 2;
    [HideInInspector]
    public List<Texture2D> Images = new List<Texture2D>();
    FileInfo[] pathImages;

    public void TakePhoto()
    {
        Debug.Log("ScreenShot");
        StartCoroutine(CaptureScreen());
    }

    private IEnumerator CaptureScreen()
    {
        yield return new WaitForEndOfFrame();

        var texture = ScreenCapture.CaptureScreenshotAsTexture();

        StartCoroutine(SaveTextureAsImage(texture));
    }

    private IEnumerator SaveTextureAsImage(Texture2D texture)
    {
        yield return new WaitForEndOfFrame();

        byte[] bytes = texture.EncodeToPNG();
        string fileName = ImgName();
        File.WriteAllBytes(fileName, bytes);
        UIManager.instance.ActivateGalleryCanvas();
    }

    private string ImgName()
    {

        string path = string.Format("{0}/Img_{1}.png", Application.persistentDataPath+"/Images", System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
#if UNITY_EDITOR
        path = string.Format("{0}/Img_{1}.png", Application.dataPath + "/StreamingAssets", System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")); 
#endif
        return path;
    }
   
    public void LoadImages()
    {
        Images = new List<Texture2D>();
        string filePath = Application.persistentDataPath + "/Images";

#if UNITY_EDITOR
        filePath = Application.dataPath + "/StreamingAssets";
#endif
        DirectoryInfo dirInfo = new DirectoryInfo(filePath);

         pathImages = dirInfo.GetFiles("*.*");


        
        foreach (var path in pathImages)
        {
            string pathString = path.ToString();
            char last = pathString[pathString.Length -1];
            if (File.Exists(pathString) && last == 'g')
            {

                byte[] bytes = File.ReadAllBytes(path.ToString());
                Texture2D texture = new Texture2D(Screen.width, Screen.height);
                texture.LoadImage(bytes);
                Images.Add(texture);
            }
        }

        ShowImage();
    }

    public void ChangeARPicture(int index)
    {

        if (Images.Count == 0)
        {
            currentIndex = 0;
        }
        else
        {
            int newIndex = currentIndex + index;

            if (newIndex < 0)
            {
                newIndex = Images.Count - 1;
            }
            else if (newIndex > Images.Count - 1)
            {
                newIndex = 0;
            }


            currentIndex = newIndex;
        }
        if (currentIndex == 1)
        {

            LoadImages();
        }

        ShowImage();


    }

    public void ShowImage() 
    {
        var texturePhoto = Images[currentIndex];
        var recImg = showImageGallery.GetComponent<RectTransform>();
        var recPanel = panel.GetComponent<RectTransform>();
        recImg.sizeDelta = new Vector2(texturePhoto.width, texturePhoto.height);
        recPanel.sizeDelta = recImg.sizeDelta;
        recPanel.localScale = new Vector3(0.42f, 0.41f, 0.42f);
        recImg.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        showImageGallery.texture = Images[currentIndex];
    }

    public void GoToGallery()
    {
        StartCoroutine(ShowGallerry());
    }

    IEnumerator ShowGallerry()
    {
        yield return new WaitForSeconds(1.5f);
        UIManager.instance.ShowGallery();
        LoadImages();
    }



    public void Share()
    {
        StartCoroutine(TakeScreenshotAndShare());
    }
    private IEnumerator TakeScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();
#if UNITY_EDITOR

        Debug.LogWarning("This feature is only available for Android and iOs");
#endif

        string filePath = pathImages[currentIndex].ToString();
        new NativeShare().AddFile(filePath)
            .SetSubject("Subject goes here").SetText("Explore the Real-World Metaverse... #TemplatesChallenge #Niantic")
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();

    }
}
