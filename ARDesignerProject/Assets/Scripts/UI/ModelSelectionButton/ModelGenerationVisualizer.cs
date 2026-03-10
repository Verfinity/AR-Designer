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

    private void Awake()
    {
        _modelGenerationEvents = ModelGenerationEvents.GetInstance();
        _modelGenerationProgressText.text = "0%";
    }

    private void OnModelGenerationStatusUpdated(string taskId, int progress)
    {
        if (_modelIdentity.ModelId != taskId)
            return;

        _modelGenerationProgressText.text = $"{progress}%";
    }

    private void OnModelGenerationFailed(string taskId)
    {
        if (_modelIdentity.ModelId != taskId)
            return;

        Destroy(gameObject);
    }

    private void OnModelGenerationSucceeded(string taskId, string modelUrl, string modelImageUrl)
    {
        if (_modelIdentity.ModelId != taskId)
            return;

        _modelGenerationProgressText.gameObject.SetActive(false);
        StartCoroutine(SetTexture(modelImageUrl));
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
    }

    private void OnDisable()
    {
        _modelGenerationEvents.ModelGenerationSucceeded -= OnModelGenerationSucceeded;
        _modelGenerationEvents.ModelGenerationFailed -= OnModelGenerationFailed;
        _modelGenerationEvents.ModelGenerationStatusUpdated -= OnModelGenerationStatusUpdated;
    }
}
