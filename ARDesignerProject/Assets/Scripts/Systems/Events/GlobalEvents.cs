using System;
using UnityEngine;
using static ModelGenerationEvents;

public class GlobalEvents : Singleton<GlobalEvents>
{
    public Action<string> ImageLoaded;
    public GenerationModel ModelSelected;
    public CreationModel ModelCreated;

    public delegate void CreationModel(string modelId, GameObject modelObj);
}
