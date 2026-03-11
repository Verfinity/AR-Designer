using UnityEngine;

[RequireComponent(typeof(ModelCreator))]
public class ModelSetuper : MonoBehaviour
{
    [SerializeField]
    private ModelCreator _modelCreator;
    [SerializeField]
    private float _startScale = 0.3f;

    private GlobalEvents _globalEvents;

    private void Awake()
    {
        _globalEvents = GlobalEvents.GetInstance();
    }

    private void SetupModel(GameObject modelObj)
    {
        var modelVisual = modelObj.transform.GetChild(0).gameObject;

        modelVisual.SetActive(false);
        modelVisual.AddComponent<MeshCollider>();
        modelVisual.transform.localScale = Vector3.one * _startScale;
    }

    private void OnUnsetupedModelCreated(string modelId, GameObject modelObj)
    {
        SetupModel(modelObj);
        _globalEvents.ModelCreated?.Invoke(modelId, modelObj);
        _globalEvents.ModelSelected?.Invoke(modelId);
    }

    private void OnEnable()
    {
        _modelCreator.UnsetupedModelCreated += OnUnsetupedModelCreated;
    }

    private void OnDisable()
    {
        _modelCreator.UnsetupedModelCreated -= OnUnsetupedModelCreated;
    }
}
