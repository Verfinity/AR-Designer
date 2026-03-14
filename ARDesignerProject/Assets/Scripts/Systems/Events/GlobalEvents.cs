using System;
using UnityEngine;
using static ModelGenerationEvents;

public class GlobalEvents : Singleton<GlobalEvents>
{
    public Action<string> ImageUrlLoaded;
    public Action<string> ImageFileLoaded;

    public GenerationModel ModelSelected;
    public CreationModel ModelCreated;

    public Action DeleteModel;

    public delegate void CreationModel(string modelId, GameObject modelObj); 
}
