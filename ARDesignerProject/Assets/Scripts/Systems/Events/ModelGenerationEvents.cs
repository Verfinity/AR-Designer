using System;

public class ModelGenerationEvents : Singleton<ModelGenerationEvents>
{
    public Action ModelGenerationFailed;
    public ModelData ModelGenerationSucceeded;
    public Action<int> ModelGenerationStatusUpdated;

    public delegate void ModelData(string modelUrl, string modelImage);
}
