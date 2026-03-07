using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ModelGeneration
{
    public class ModelGenerator : MonoBehaviour
    {
        [SerializeField]
        private ModelGenerationConfigurationScriptableObject _modelGenerationConfig;

        private GlobalEvents _globalEvents;

        private void Awake()
        {
            _globalEvents = GlobalEvents.GetInstance();
        }

        private void OnImageLoaded(string imageUrl)
        {
            StartCoroutine(GetModelGenerationTask(imageUrl));
        }

        private string GetImageType(string imageUrl)
        {
            var imageArr = imageUrl.Split('.');
            string type = imageArr[imageArr.Length - 1];
            return type;
        }

        private IEnumerator GetModelGenerationTask(string imageUrl)
        {
            var body = new GenerateModelRequest
            {
                type = "image_to_model",
                file = new GenerateModelFile
                {
                    type = GetImageType(imageUrl),
                    url = imageUrl
                }
            };

            string jsonBody = JsonUtility.ToJson(body);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

            using (var request = new UnityWebRequest(_modelGenerationConfig.ApiUrl, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();

                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {_modelGenerationConfig.ApiKey}");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseText = request.downloadHandler.text;
                    var responseData = JsonUtility.FromJson<GenerateModelResponse>(responseText);
                    string taskId = responseData.data.task_id;

                    Debug.Log($"Task id: {taskId}");
                    StartCoroutine(CheckTask(taskId));
                }
                else
                {
                    Debug.Log($"Can't create task with status code: {request.responseCode}");
                    Debug.Log(request.downloadHandler.text);
                }
            }
        }

        private IEnumerator CheckTask(string taskId)
        {
            while (true)
            {
                using (var request = new UnityWebRequest($"{_modelGenerationConfig.ApiUrl}/{taskId}", "GET"))
                {
                    request.downloadHandler = new DownloadHandlerBuffer();

                    request.SetRequestHeader("Authorization", $"Bearer {_modelGenerationConfig.ApiKey}");

                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        var taskStatusResponse = JsonUtility.FromJson<TaskStatusResponse>(request.downloadHandler.text);
                        Debug.Log($"Task status: {taskStatusResponse.data.status}");
                        Debug.Log($"Task progress: {taskStatusResponse.data.progress}");

                        if (taskStatusResponse.data.status == "success")
                        {
                            Debug.Log($"Model URL: {taskStatusResponse.data.output.pbr_model}");
                            Debug.Log($"Rendered image URL: {taskStatusResponse.data.output.rendered_image}");
                        }

                        if (taskStatusResponse.data.status != "queued" && taskStatusResponse.data.status != "running")
                            break;
                    }
                    else
                    {
                        Debug.Log($"Can't get task information with status code: {request.result}");
                        Debug.Log(request.downloadHandler.text);
                    }
                }

                yield return new WaitForSeconds(5f);
            }
        }

        private void OnEnable()
        {
            _globalEvents.ImageLoaded += OnImageLoaded;
        }

        private void OnDisable()
        {
            _globalEvents.ImageLoaded -= OnImageLoaded;
        }
    }

    #region GENERATE_MODEL_REQUEST

    [Serializable]
    public class GenerateModelRequest
    {
        public string type;
        public GenerateModelFile file;
    }

    [Serializable]
    public class GenerateModelFile
    {
        public string type;
        public string url;
    }

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