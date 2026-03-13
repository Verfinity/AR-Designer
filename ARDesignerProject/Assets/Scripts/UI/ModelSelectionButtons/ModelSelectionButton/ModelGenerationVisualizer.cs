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
    private TextMeshProUGUI _modelGenerationProgressText;
    [SerializeField]
    private RawImage _rawImage;

    private ModelGenerationEvents _modelGenerationEvents;
    private GlobalEvents _globalEvents;

    // Texture applying
    private bool _modelCreated = false;
    private Texture2D _modelTexture;

    private void Awake()
    {
        _modelGenerationEvents = ModelGenerationEvents.GetInstance();
        _globalEvents = GlobalEvents.GetInstance();
        _modelGenerationProgressText.text = "Waiting...";
    }

    private void OnModelGenerationStatusUpdated(string modelId, int progress)
    {
        if (_modelIdentity.ModelId != modelId)
            return;

        _modelGenerationProgressText.text = $"{progress}%";
    }

    private void OnModelGenerationFailed(string modelId)
    {
        if (_modelIdentity.ModelId != modelId)
            return;

        Destroy(gameObject);
    }

    private void OnModelGenerationSucceeded(string modelId, string modelUrl, string modelImageUrl)
    {
        if (_modelIdentity.ModelId != modelId)
            return;

        _modelGenerationProgressText.text = "Creating...";
        StartCoroutine(SetTexture(modelImageUrl));
    }

    private void OnModelCreated(string modelId, GameObject modelObj)
    {
        _modelGenerationProgressText.gameObject.SetActive(false);
        _modelCreated = true;
        if (_modelTexture != null)
            _rawImage.texture = _modelTexture;
    }

    private IEnumerator SetTexture(string imageUrl)
    {
        using (var request = new UnityWebRequest(imageUrl, "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                _modelTexture = Texture2DExt.CreateTexture2DFromWebP(request.downloadHandler.data, true, true, out _);
                if (_modelCreated)
                    _rawImage.texture = _modelTexture;
            }
        }
    }

    private void OnEnable()
    {
        _modelGenerationEvents.ModelGenerationSucceeded += OnModelGenerationSucceeded;
        _modelGenerationEvents.ModelGenerationFailed += OnModelGenerationFailed;
        _modelGenerationEvents.ModelGenerationStatusUpdated += OnModelGenerationStatusUpdated;
        _globalEvents.ModelCreated += OnModelCreated;
    }

    private void OnDisable()
    {
        _modelGenerationEvents.ModelGenerationSucceeded -= OnModelGenerationSucceeded;
        _modelGenerationEvents.ModelGenerationFailed -= OnModelGenerationFailed;
        _modelGenerationEvents.ModelGenerationStatusUpdated -= OnModelGenerationStatusUpdated;
        _globalEvents.ModelCreated -= OnModelCreated;
    }
}
