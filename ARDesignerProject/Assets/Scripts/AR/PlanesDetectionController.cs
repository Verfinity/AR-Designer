using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneManager))]
public class PlanesDetectionController : MonoBehaviour
{
    [SerializeField]
    private ARPlaneManager _planeManager;

    private bool _isFloorDetected = false;

    private void OnPlanesChanged(ARTrackablesChangedEventArgs<ARPlane> planes)
    {
        if (planes.added.Count == 0)
            return;

        foreach (var plane in planes.added)
        {
            if (_isFloorDetected)
            {
                plane.gameObject.SetActive(false);
                continue;
            }

            if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp &&
                !_isFloorDetected)
                _isFloorDetected = true;
        }
    }

    private void OnEnable()
    {
        _planeManager.trackablesChanged.AddListener(OnPlanesChanged);
    }

    private void OnDisable()
    {
        _planeManager.trackablesChanged.RemoveListener(OnPlanesChanged);
    }
}
