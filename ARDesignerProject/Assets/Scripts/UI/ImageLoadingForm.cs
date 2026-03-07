using TMPro;
using UnityEngine;

public class ImageLoadingForm : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField _inputField;

    private GlobalEvents _globalEvents;

    private void Awake()
    {
        _globalEvents = GlobalEvents.GetInstance();
    }

    private void OnEnable()
    {
        _inputField.text = string.Empty;
    }

    public void ImageLoaded()
    {
        if (_inputField.text == string.Empty)
            return;

        _globalEvents.ImageLoaded?.Invoke(_inputField.text);
        Debug.Log($"Entered URL: {_inputField.text}");
    }
}
