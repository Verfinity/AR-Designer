using System;

public class ModelGenerationEvents : Singleton<ModelGenerationEvents>
{
    public GenerationModel ModelGenerationStarted;
    public GenerationModelStatus ModelGenerationStatusUpdated;
    public GenerationModel ModelGenerationFailed;
    public ModelData ModelGenerationSucceeded;

    public delegate void ModelData(string taskId, string modelUrl, string modelImage);
    public delegate void GenerationModel(string taskId);
    public delegate void GenerationModelStatus(string taskId, int progress);
}
