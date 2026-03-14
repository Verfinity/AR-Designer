using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotMaker : MonoBehaviour
{
    [SerializeField]
    private Button _button;
    [SerializeField]
    private Canvas _canvas;

    private void TakeScreenshot()
    {
        StartCoroutine(TakeScreenshotCoroutine());
    }

    private IEnumerator TakeScreenshotCoroutine()
    {
        _canvas.enabled = false;

        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();
        _canvas.enabled = true;

        string name = DateTime.Now.ToString();
        NativeGallery.SaveImageToGallery(ss, Application.productName + " Photos", name);
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(TakeScreenshot);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(TakeScreenshot);
    }
}
