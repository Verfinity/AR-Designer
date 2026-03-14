using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ARPlanesVisualizationController : MonoBehaviour
{
    [SerializeField]
    private ARPlaneManager _planeManager;
    [SerializeField]
    private Button _button;
    [SerializeField]
    private Image _image;
    [SerializeField]
    private Sprite _planesVisualizationEnableSprite;
    [SerializeField]
    private Sprite _planesVisualizationDisableSprite;

    private bool _planesVisualizationEnabled = true;

    private void OnClick()
    {
        _planesVisualizationEnabled = !_planesVisualizationEnabled;
        UpdateSprite();
        UpdateInstantiatedPlanesVisualization();
    }

    private void UpdateInstantiatedPlanesVisualization()
    {
        foreach (var plane in _planeManager.trackables)
        {
            if (plane.gameObject.activeSelf)
                plane.GetComponent<ARPlaneMeshVisualizer>().enabled = _planesVisualizationEnabled;
        }
    }

    private void UpdateSprite()
    {
        if (_planesVisualizationEnabled)
            _image.sprite = _planesVisualizationEnableSprite;
        else
            _image.sprite = _planesVisualizationDisableSprite;
    }

    private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARPlane> args)
    {
        foreach (var plane in args.added)
            plane.GetComponent<ARPlaneMeshVisualizer>().enabled = _planesVisualizationEnabled;
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClick);
        _planeManager.trackablesChanged.AddListener(OnTrackablesChanged);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClick);
        _planeManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
    }
}
