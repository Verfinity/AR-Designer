using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ModelSelectionButtonCreator : MonoBehaviour
{
    [SerializeField]
    private RectTransform _instantiateParentLayoutGroup;
    [SerializeField]
    private RectTransform _instantiateButton;

    private ModelGenerationEvents _modelGenerationEvents;

    private void Awake()
    {
        _modelGenerationEvents = ModelGenerationEvents.GetInstance();
    }

    private void OnModelGenerationStarted(string taskId)
    {
        var obj = Instantiate(_instantiateButton, _instantiateParentLayoutGroup);
        StartCoroutine(UpdateLayoutGroup());

        ModelIdentity modelSelectionButton;
        if (obj.TryGetComponent<ModelIdentity>(out modelSelectionButton))
            modelSelectionButton.SetModelId(taskId);
    }

    private IEnumerator UpdateLayoutGroup()
    {
        yield return new WaitForEndOfFrame();

        LayoutRebuilder.ForceRebuildLayoutImmediate(_instantiateParentLayoutGroup);
    }

    private void OnEnable()
    {
        _modelGenerationEvents.ModelGenerationStarted += OnModelGenerationStarted;
    }

    private void OnDisable()
    {
        _modelGenerationEvents.ModelGenerationStarted -= OnModelGenerationStarted;
    }
}
