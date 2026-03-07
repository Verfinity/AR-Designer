using System;

public class GlobalEvents : Singleton<GlobalEvents>
{
    public Action<string> ImageLoaded;
}
