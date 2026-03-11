using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

[RequireComponent(typeof(ARPlacementInteractable))]
public class ModelSpawnVisualizerActivator : MonoBehaviour
{
    [SerializeField]
    private ARPlacementInteractable _placementInteratable;

    private void OnObjectPlaced(ARObjectPlacementEventArgs eventArgs)
    {
        ModelSpawnVisualizer modelSpawnVisualizer;
        if (eventArgs.placementObject.TryGetComponent<ModelSpawnVisualizer>(out modelSpawnVisualizer))
            modelSpawnVisualizer.enabled = true;
    }

    private void OnEnable()
    {
        _placementInteratable.objectPlaced.AddListener(OnObjectPlaced);
    }

    private void OnDisable()
    {
        _placementInteratable.objectPlaced.RemoveListener(OnObjectPlaced);
    }
}
