using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private UIEvents _uiEvents;

    private Dictionary<string, RectTransform> _buttonWithoutModelIdentity = new Dictionary<string, RectTransform>();

    private void Awake()
    {
        _modelGenerationEvents = ModelGenerationEvents.GetInstance();
        _globalEvents = GlobalEvents.GetInstance();
        _uiEvents = UIEvents.GetInstance();
    }

    private void InitializeButton(RectTransform button)
    {
        string buttonId = Guid.NewGuid().ToString();

        ModelSelectionButtonIdentity buttonIdentity;
        if (button.TryGetComponent<ModelSelectionButtonIdentity>(out buttonIdentity))
            buttonIdentity.SetIdentity(buttonId);

        _buttonWithoutModelIdentity.Add(buttonId, button);
    }

    private void OnImageLoaded(string _)
    {
        var obj = Instantiate(_instantiateButton, _instantiateParentLayoutGroup);
        InitializeButton(obj);   

        StartCoroutine(UpdateLayoutGroup());
    }

    private void OnModelGenerationStarted(string taskId)
    {
        var item = _buttonWithoutModelIdentity.FirstOrDefault();
        var obj = item.Value;
        _buttonWithoutModelIdentity.Remove(item.Key);

        ModelIdentity modelSelectionButton;
        if (obj.TryGetComponent<ModelIdentity>(out modelSelectionButton))
            modelSelectionButton.SetIdentity(taskId);
    }

    private void OnGenerationTokensEnded()
    {
        var item = _buttonWithoutModelIdentity.FirstOrDefault();
        var obj = item.Value;
        _buttonWithoutModelIdentity.Remove(item.Key);

        _uiEvents.DestroyModelSelectionButton?.Invoke(item.Key);
    }

    private void OnButtonDestroyed(string buttonId)
    {
        StartCoroutine(UpdateLayoutGroup());
    }

    private IEnumerator UpdateLayoutGroup()
    {
        yield return new WaitForEndOfFrame();

        LayoutRebuilder.ForceRebuildLayoutImmediate(_instantiateParentLayoutGroup);
    }

    private void OnEnable()
    {
        _modelGenerationEvents.ModelGenerationStarted += OnModelGenerationStarted;
        _modelGenerationEvents.RequestSendingFailed += OnGenerationTokensEnded;
        _globalEvents.ImageFileLoaded += OnImageLoaded;
        _globalEvents.ImageUrlLoaded += OnImageLoaded;
        _uiEvents.ModelSelectionButtonDestroyed += OnButtonDestroyed;
    }

    private void OnDisable()
    {
        _modelGenerationEvents.ModelGenerationStarted -= OnModelGenerationStarted;
        _modelGenerationEvents.RequestSendingFailed -= OnGenerationTokensEnded;
        _globalEvents.ImageFileLoaded -= OnImageLoaded;
        _globalEvents.ImageUrlLoaded -= OnImageLoaded;
        _uiEvents.ModelSelectionButtonDestroyed -= OnButtonDestroyed;
    }
}
