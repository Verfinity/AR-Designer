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

    private void SetupCollider(GameObject modelObj, GameObject modelVisual)
    {
        var interactables = modelObj.GetComponents<ARBaseGestureInteractable>();
        foreach (var interactable in interactables)
            interactable.enabled = false;
        SetBoxCollider(modelVisual);
        foreach (var interactable in interactables)
            interactable.enabled = true;
    }

    private void SetupSelectionObject(Transform selectionObject, GameObject modelVisual)
    {
        var renderer = modelVisual.GetComponent<Renderer>();

        selectionObject.position = renderer.bounds.center + new Vector3(0, renderer.bounds.size.y / 2, 0);
        selectionObject.localRotation = Quaternion.identity;
        selectionObject.localScale = renderer.bounds.size;
    }

    private void SetupModelVisual(GameObject modelVisual)
    {
        modelVisual.SetActive(false);

        var renderer = modelVisual.GetComponent<Renderer>();

        modelVisual.transform.position += new Vector3(0, renderer.bounds.size.y / 2, 0);
        modelVisual.transform.localRotation = Quaternion.identity;
        modelVisual.transform.localScale = Vector3.one * _startScale;
    }

    private void SetupModel(GameObject modelObj)
    {
        // SelectionVisualization object has 0 index
        var modelVisual = modelObj.transform.GetChild(1).gameObject;

        SetupCollider(modelObj, modelVisual);

        ARSelectionInteractable selectionInteractable;
        if (modelObj.TryGetComponent<ARSelectionInteractable>(out selectionInteractable))
            SetupSelectionObject(selectionInteractable.selectionVisualization.transform, modelVisual);

        SetupModelVisual(modelVisual);
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
