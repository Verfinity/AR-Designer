using System;

public class GlobalEvents
{
    private static GlobalEvents _globalEvents;

    public static GlobalEvents GetInstance()
    {
        if (_globalEvents == null)
            _globalEvents = new GlobalEvents();

        return _globalEvents;
    }

    public Action<string> ImageLoaded;
}
