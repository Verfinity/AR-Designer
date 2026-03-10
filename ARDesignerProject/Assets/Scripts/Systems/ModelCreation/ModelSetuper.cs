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
        modelObj.SetActive(false);
        modelObj.AddComponent<MeshCollider>();
        modelObj.transform.localScale = Vector3.one * _startScale;
    }

    private void OnUnsetupedModelCreated(string modelId, GameObject modelObj)
    {
        SetupModel(modelObj.transform.GetChild(0).gameObject);
        _globalEvents.ModelCreated?.Invoke(modelId, modelObj);
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
