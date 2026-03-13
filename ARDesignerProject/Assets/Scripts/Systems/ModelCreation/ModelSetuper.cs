using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

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

    private BoxCollider SetBoxCollider(GameObject modelVisual)
    {
        var boxCollider = modelVisual.AddComponent<BoxCollider>();
        var renderer = modelVisual.GetComponent<Renderer>();

        boxCollider.center = modelVisual.transform.InverseTransformPoint(renderer.bounds.center);
        boxCollider.size = renderer.bounds.size;

        return boxCollider;
    }

    private void SetupModel(GameObject modelObj)
    {
        var modelVisual = modelObj.transform.GetChild(0).gameObject;

        modelVisual.SetActive(false);

        var collider = SetBoxCollider(modelVisual);
        ARSelectionInteractable selectionInteractable;
        if (modelObj.TryGetComponent<ARSelectionInteractable>(out selectionInteractable))
            selectionInteractable.colliders.Add(collider);
        
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
