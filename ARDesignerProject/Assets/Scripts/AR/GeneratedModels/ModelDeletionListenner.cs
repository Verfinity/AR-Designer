using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

[RequireComponent(typeof(ARSelectionInteractable))]
public class ModelDeletionListenner : MonoBehaviour
{
    [SerializeField]
    private ARSelectionInteractable _selectionInteractable;

    private GlobalEvents _globalEvents;

    private void Awake()
    {
        _globalEvents = GlobalEvents.GetInstance();
    }

    private void OnModelDelete()
    {
        if (_selectionInteractable.isSelected)
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        _globalEvents.DeleteModel += OnModelDelete;
    }

    private void OnDisable()
    {
        _globalEvents.DeleteModel -= OnModelDelete;
    }
}
