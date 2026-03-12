using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ModelGeneration
{
    public class ModelGenerator : MonoBehaviour
    {
        [SerializeField]
        private ApiConfigurationScriptableObject _modelGenerationConfig;
        [SerializeField]
        private int _attempsToAskFailedTask;

        private GlobalEvents _globalEvents;
        private ModelGenerationEvents _modelGenerationEvents;

        private int _currentAttemsToAskFaieldTask = 0;

        private void Awake()
        {
            _globalEvents = GlobalEvents.GetInstance();
            _modelGenerationEvents = ModelGenerationEvents.GetInstance();
        }

        private void OnImageUrlLoaded(string imageUrl)
        {
            StartCoroutine(GenerateModelCoroutine(GetGenerateModelFromImageUrlData(imageUrl)));
        }

        private void OnImageFileLoaded(string imagePath)
        {
            StartCoroutine(UploadImageCoroutine(imagePath));
        }

        private string GetImageType(string imageUrl)
        {
            var imageArr = imageUrl.Split('.');
            string type = imageArr[imageArr.Length - 1];
            return type;
        }

        private IEnumerator UploadImageCoroutine(string imagePath)
        {
            var wwwForm = new WWWForm();
            byte[] imageData = File.ReadAllBytes(imagePath);
            wwwForm.AddBinaryData("file", imageData, imagePath, $"image/{GetImageType(imagePath)}");
            using (var request = UnityWebRequest.Post($"{_modelGenerationConfig.ApiUrl}/upload/sts", wwwForm))
            {
                request.downloadHandler = new DownloadHandlerBuffer();

                request.SetRequestHeader("Authorization", $"Bearer {_modelGenerationConfig.ApiKey}");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseText = request.downloadHandler.text;
                    var responseData = JsonUtility.FromJson<UploadFileResponse>(responseText);
                    string imageToken = responseData.data.image_token;

                    Debug.Log($"Image token: {imageToken}");
                    StartCoroutine(GenerateModelCoroutine(GetGenerateModelFromImageFileData(imageToken)));
                }
                else
                {
                    Debug.Log($"Can't load file with status code: {request.responseCode}");
                    Debug.Log(request.downloadHandler.text);
                }
            }
        }

        private string GetGenerateModelFromImageUrlData(string imageUrl)
        {
            var body = new GenerateModelFromImageUrlRequest
            {
                type = "image_to_model",
                file = new GenerateModelFromImageUrlFile
                {
                    type = GetImageType(imageUrl),
                    url = imageUrl
                }
            };
            string jsonBody = JsonUtility.ToJson(body);

            return jsonBody;
        }

        private string GetGenerateModelFromImageFileData(string imageToken)
        {
            var body = new GenerateModelFromImageDataRequest
            {
                type = "image_to_model",
                file = new GenerateModelFromImageDataFile
                {
                    type = GetImageType(imageToken),
                    file_token = imageToken
                }
            };
            string jsonBody = JsonUtility.ToJson(body);

            return jsonBody;
        }

        private IEnumerator GenerateModelCoroutine(string jsonBody)
        {
            using (var request = UnityWebRequest.Post($"{_modelGenerationConfig.ApiUrl}/task", jsonBody, "application/json"))
            {
                request.downloadHandler = new DownloadHandlerBuffer();

                request.SetRequestHeader("Authorization", $"Bearer {_modelGenerationConfig.ApiKey}");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseText = request.downloadHandler.text;
                    var responseData = JsonUtility.FromJson<GenerateModelResponse>(responseText);
                    string taskId = responseData.data.task_id;

                    Debug.Log($"Task id: {taskId}");
                    StartCoroutine(CheckTaskCoroutine(taskId));
                }
                else
                {
                    Debug.Log($"Can't create task with status code: {request.responseCode}");
                    Debug.Log(request.downloadHandler.text);
                }
            }
        }

        private IEnumerator CheckTaskCoroutine(string taskId)
        {
            _modelGenerationEvents.ModelGenerationStarted?.Invoke(taskId);
            _currentAttemsToAskFaieldTask = 0;
            while (true)
            {
                using (var request = UnityWebRequest.Get($"{_modelGenerationConfig.ApiUrl}/task/{taskId}"))
                {
                    request.downloadHandler = new DownloadHandlerBuffer();

                    request.SetRequestHeader("Authorization", $"Bearer {_modelGenerationConfig.ApiKey}");

                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        var taskStatusResponse = JsonUtility.FromJson<TaskStatusResponse>(request.downloadHandler.text);
                        if (taskStatusResponse.data.status != "queued" && taskStatusResponse.data.status != "running")
                        {
                            if (taskStatusResponse.data.status == "success")
                            {
                                _modelGenerationEvents.ModelGenerationSucceeded?.Invoke(taskId, taskStatusResponse.data.output.pbr_model, taskStatusResponse.data.output.rendered_image);
                                Debug.Log($"Model URL: {taskStatusResponse.data.output.pbr_model}");
                                Debug.Log($"Rendered image URL: {taskStatusResponse.data.output.rendered_image}");
                            }
                            else
                            {
                                _modelGenerationEvents.ModelGenerationFailed?.Invoke(taskId);
                                Debug.Log("Model generation failed!");
                            }
                            break;
                        }
                        else
                        {
                            _modelGenerationEvents.ModelGenerationStatusUpdated?.Invoke(taskId, taskStatusResponse.data.progress);
                            Debug.Log($"Task status: {taskStatusResponse.data.status}");
                            Debug.Log($"Task progress: {taskStatusResponse.data.progress}");
                        }
                    }
                    else
                    {
                        Debug.Log($"Attempts left: {_attempsToAskFailedTask - _currentAttemsToAskFaieldTask}");
                        Debug.Log($"Can't get task information with status code: {request.result}");
                        Debug.Log(request.downloadHandler.text);
                        if (_currentAttemsToAskFaieldTask == _attempsToAskFailedTask)
                        {
                            _modelGenerationEvents.ModelGenerationFailed?.Invoke(taskId);
                            Debug.Log("Model generation failed!");
                            break;
                        }
                        _currentAttemsToAskFaieldTask++;
                    }
                }

                yield return new WaitForSeconds(5f);
            }
        }

        private void OnEnable()
        {
            _globalEvents.ImageUrlLoaded += OnImageUrlLoaded;
            _globalEvents.ImageFileLoaded += OnImageFileLoaded;
        }

        private void OnDisable()
        {
            _globalEvents.ImageUrlLoaded -= OnImageUrlLoaded;
            _globalEvents.ImageFileLoaded -= OnImageFileLoaded;
        }
    }

    #region GENERATE_MODEL_REQUEST

    #region GENERATE_MODEL_FROM_IMAGE_URL

    [Serializable]
    public class GenerateModelFromImageUrlRequest
    {
        public string type;
        public GenerateModelFromImageUrlFile file;
    }

    [Serializable]
    public class GenerateModelFromImageUrlFile
    {
        public string type;
        public string url;
    }

    #endregion

    #region GENERATE_MODEL_FROM_IMAGE_DATA

    [Serializable]
    public class GenerateModelFromImageDataRequest
    {
        public string type;
        public GenerateModelFromImageDataFile file;
    }

    [Serializable]
    public class GenerateModelFromImageDataFile
    {
        public string type;
        public string file_token;
    }

    #endregion

    #endregion

    #region GENERATE_MODEL_RESPONSE

    [Serializable]
    public class GenerateModelResponse
    {
        public GenerateModelTaskData data;
    }

    [Serializable]
    public class GenerateModelTaskData
    {
        public string task_id;
    }

    #endregion

    #region UPLOAD_FILE_RESPONSE
    [Serializable]
    public class UploadFileResponse
    {
        public UploadFileData data;
    }

    [Serializable]
    public class UploadFileData
    {
        public string image_token;
    }
    #endregion

    #region TASK_STATUS_RESPONSE

    [Serializable]
    public class TaskStatusResponse
    {
        public TaskStatusData data;
    }

    [Serializable]
    public class TaskStatusData
    {
        public string status;
        public TaskStatusOutput output;
        public int progress;
    }

    [Serializable]
    public class TaskStatusOutput
    {
        public string pbr_model;
        public string rendered_image;
    }

    #endregion
}