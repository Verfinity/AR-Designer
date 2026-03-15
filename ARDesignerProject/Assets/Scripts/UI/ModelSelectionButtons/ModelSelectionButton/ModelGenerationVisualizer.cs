using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using WebP;

public class ModelGenerationVisualizer : MonoBehaviour
{
    [SerializeField]
    private ModelIdentity _modelIdentity;
    [SerializeField]
    private ModelSelectionButtonIdentity _buttonIdentity;
    [SerializeField]
    private TextMeshProUGUI _modelGenerationProgressText;
    [SerializeField]
    private RawImage _rawImage;
    [SerializeField]
    private Color _failedColor = Color.red;
    [SerializeField]
    private float _failedSecondsCount = 1f;

    private ModelGenerationEvents _modelGenerationEvents;
    private GlobalEvents _globalEvents;
    private UIEvents _uiEvents;

    // Texture applying
    private bool _modelCreated = false;
    private Texture2D _modelTexture;

    private void Awake()
    {
        _modelGenerationEvents = ModelGenerationEvents.GetInstance();
        _globalEvents = GlobalEvents.GetInstance();
        _uiEvents = UIEvents.GetInstance();
        _modelGenerationProgressText.text = "Waiting...";
    }

    private IEnumerator DestroyButton()
    {
        _modelGenerationProgressText.color = _failedColor;
        _modelGenerationProgressText.text = "Failed";

        yield return new WaitForSeconds(_failedSecondsCount);

        _uiEvents.ModelSelectionButtonDestroyed?.Invoke(_buttonIdentity.Id);
        Destroy(gameObject);
    }

    private void OnButtonDeleted(string buttonId)
    {
        if (_buttonIdentity.Id != buttonId)
            return;

        StartCoroutine(DestroyButton());
    }

    private void OnModelGenerationStatusUpdated(string modelId, int progress)
    {
        if (_modelIdentity.Id != modelId)
            return;

        _modelGenerationProgressText.text = $"{progress}%";
    }

    private void OnModelGenerationFailed(string modelId)
    {
        if (_modelIdentity.Id != modelId)
            return;

        StartCoroutine(DestroyButton());
    }

    private void SetTexture()
    {
        _rawImage.texture = _modelTexture;
        _rawImage.color = Color.white;
    }

    private void OnModelGenerationSucceeded(string modelId, string modelUrl, string modelImageUrl)
    {
        if (_modelIdentity.Id != modelId)
            return;

        _modelGenerationProgressText.text = "Creating...";
        StartCoroutine(SetTextureCoroutine(modelImageUrl));
    }

    private void OnModelCreated(string modelId, GameObject modelObj)
    {
        if (_modelIdentity.Id != modelId)
            return;

        _modelGenerationProgressText.gameObject.SetActive(false);
        _modelCreated = true;
        if (_modelTexture != null)
            SetTexture();
    }

    private IEnumerator SetTextureCoroutine(string imageUrl)
    {
        using (var request = new UnityWebRequest(imageUrl, "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                _modelTexture = Texture2DExt.CreateTexture2DFromWebP(request.downloadHandler.data, true, true, out _);
                if (_modelCreated)
                    SetTexture();
            }
        }
    }

    private void OnEnable()
    {
        _modelGenerationEvents.ModelGenerationSucceeded += OnModelGenerationSucceeded;
        _modelGenerationEvents.ModelGenerationFailed += OnModelGenerationFailed;
        _modelGenerationEvents.ModelGenerationStatusUpdated += OnModelGenerationStatusUpdated;
        _globalEvents.ModelCreated += OnModelCreated;
        _uiEvents.DestroyModelSelectionButton += OnButtonDeleted;
    }

    private void OnDisable()
    {
        _modelGenerationEvents.ModelGenerationSucceeded -= OnModelGenerationSucceeded;
        _modelGenerationEvents.ModelGenerationFailed -= OnModelGenerationFailed;
        _modelGenerationEvents.ModelGenerationStatusUpdated -= OnModelGenerationStatusUpdated;
        _globalEvents.ModelCreated -= OnModelCreated;
        _uiEvents.DestroyModelSelectionButton -= OnButtonDeleted;
    }
}
