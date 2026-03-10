using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ModelSelectionUIController : MonoBehaviour
{
    [SerializeField]
    private ModelIdentity _modelIdentity;
    [SerializeField]
    private Button _button;

    private GlobalEvents _globalEvents;

    private void Awake()
    {
        _globalEvents = GlobalEvents.GetInstance();
    }

    private void OnModelCreated(string modelId, GameObject modelObj)
    {
        _button.onClick.AddListener(OnButtonPressed);
    }

    private void OnButtonPressed()
    {
        _globalEvents.ModelSelected?.Invoke(_modelIdentity.ModelId);
    }

    private void OnEnable()
    {
        _globalEvents.ModelCreated += OnModelCreated;
    }

    private void OnDisable()
    {
        _globalEvents.ModelCreated -= OnModelCreated;
    }
}
