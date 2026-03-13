using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelSelectionButtonCreator : MonoBehaviour
{
    [SerializeField]
    private RectTransform _instantiateParentLayoutGroup;
    [SerializeField]
    private RectTransform _instantiateButton;

    private ModelGenerationEvents _modelGenerationEvents;
    private GlobalEvents _globalEvents;

    private List<RectTransform> _lastInstantiatedButtons = new List<RectTransform>();

    private void Awake()
    {
        _modelGenerationEvents = ModelGenerationEvents.GetInstance();
        _globalEvents = GlobalEvents.GetInstance();
    }

    private void OnImageLoaded(string _)
    {
        var obj = Instantiate(_instantiateButton, _instantiateParentLayoutGroup);
        _lastInstantiatedButtons.Add(obj);
        StartCoroutine(UpdateLayoutGroup());
    }

    private void OnModelGenerationStarted(string taskId)
    {
        var obj = _lastInstantiatedButtons[0];
        _lastInstantiatedButtons.RemoveAt(0);

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
        _globalEvents.ImageFileLoaded += OnImageLoaded;
        _globalEvents.ImageUrlLoaded += OnImageLoaded;
    }

    private void OnDisable()
    {
        _modelGenerationEvents.ModelGenerationStarted -= OnModelGenerationStarted;
        _globalEvents.ImageFileLoaded -= OnImageLoaded;
        _globalEvents.ImageUrlLoaded -= OnImageLoaded;
    }
}
