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
    private string _modelImageUrl = string.Empty;

    private void Awake()
    {
        _modelGenerationEvents = ModelGenerationEvents.GetInstance();
        _globalEvents = GlobalEvents.GetInstance();
        _modelGenerationProgressText.text = "0%";
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
        _modelImageUrl = modelImageUrl;
    }

    private void OnModelCreated(string modelId, GameObject modelObj)
    {
        _modelGenerationProgressText.gameObject.SetActive(false);
        StartCoroutine(SetTexture(_modelImageUrl));
    }

    private IEnumerator SetTexture(string imageUrl)
    {
        using (var request = new UnityWebRequest(imageUrl, "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var texture2D = Texture2DExt.CreateTexture2DFromWebP(request.downloadHandler.data, true, true, out _);
                _rawImage.texture = texture2D;
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
