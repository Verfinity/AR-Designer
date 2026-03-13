using System;

public class ModelGenerationEvents : Singleton<ModelGenerationEvents>
{
    public GenerationModel ModelGenerationStarted;
    public GenerationModelStatus ModelGenerationStatusUpdated;
    public GenerationModel ModelGenerationFailed;
    public ModelData ModelGenerationSucceeded;
    public Action RequestSendingFailed;

    public delegate void ModelData(string modelId, string modelUrl, string modelImageUrl);
    public delegate void GenerationModel(string modelId);
    public delegate void GenerationModelStatus(string modelId, int progress);
}
