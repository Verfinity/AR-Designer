using UnityEngine;

public class ImageFileLoader : MonoBehaviour
{
    private GlobalEvents _globalEvents;

    private void Awake()
    {
        _globalEvents = GlobalEvents.GetInstance();
    }

    public void LoadImage()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path == null)
                return;

            _globalEvents.ImageFileLoaded?.Invoke(path);
            Debug.Log($"Loaded file path: {path}");
        });
    }
}
