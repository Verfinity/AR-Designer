using UnityEngine;

public class ModelSelectionVisualizer : MonoBehaviour
{
    [SerializeField]
    private ModelIdentity _modelIdentity;
    [SerializeField]
    private float _selectedScale;

    private RectTransform _rectTransform;

    private GlobalEvents _globalEvents;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        _globalEvents = GlobalEvents.GetInstance();
    }

    private void OnModelSelected(string selectedModelId)
    {
        if (_modelIdentity.ModelId != selectedModelId)
            _rectTransform.localScale = Vector2.one;
        else
            _rectTransform.localScale = Vector2.one * _selectedScale;
    }

    private void OnEnable()
    {
        _globalEvents.ModelSelected += OnModelSelected;
    }

    private void OnDisable()
    {
        _globalEvents.ModelSelected -= OnModelSelected;
    }
}
