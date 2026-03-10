using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class ModelSelector : MonoBehaviour
{
    [SerializeField]
    private ARPlacementInteractable _placementInteractable;

    private GlobalEvents _globalEvents;
    private Dictionary<string, GameObject> _createdModels = new Dictionary<string, GameObject>();

    private void Awake()
    {
        _globalEvents = GlobalEvents.GetInstance();
    }

    private void OnModelCreated(string modelId, GameObject modelObj)
    {
        _createdModels.Add(modelId, modelObj);
    }

    private void OnModelSelected(string modelId)
    {
        _placementInteractable.placementPrefab = _createdModels[modelId];
    }

    private void OnEnable()
    {
        _globalEvents.ModelCreated += OnModelCreated;
        _globalEvents.ModelSelected += OnModelSelected;
    }

    private void OnDisable()
    {
        _globalEvents.ModelCreated -= OnModelCreated;
        _globalEvents.ModelSelected -= OnModelSelected;
    }
}
