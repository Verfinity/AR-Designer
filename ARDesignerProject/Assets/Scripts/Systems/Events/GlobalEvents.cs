using System;
using static ModelGenerationEvents;

public class GlobalEvents : Singleton<GlobalEvents>
{
    public Action<string> ImageLoaded;
    public GenerationModel ModelSelected;
    public GenerationModel ModelCreated;
}
