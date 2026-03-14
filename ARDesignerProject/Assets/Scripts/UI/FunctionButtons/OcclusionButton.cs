using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class OcclusionButton : MonoBehaviour
{
    [SerializeField]
    private Button _button;
    [SerializeField]
    private AROcclusionManager _occlusionManager;
    [SerializeField]
    private Image _image;
    [SerializeField]
    private Sprite _occlusionEnableSprite;
    [SerializeField]
    private Sprite _occlusionDisableSprite;

    private void Awake()
    {
        SetOcclusionStateSprite(_occlusionManager.enabled);
    }

    private void SetOcclusionStateSprite(bool occlusionState)
    {
        if (occlusionState)
            _image.sprite = _occlusionEnableSprite;
        else
            _image.sprite = _occlusionDisableSprite;
    }

    private void OnClick()
    {
        _occlusionManager.enabled = !_occlusionManager.enabled;
        SetOcclusionStateSprite(_occlusionManager.enabled);
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClick);
    }
}
