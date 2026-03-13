using System;

public class UIEvents : Singleton<UIEvents>
{
    public ModelSelectionButton DestroyModelSelectionButton;
    public ModelSelectionButton ModelSelectionButtonDestroyed;

    public delegate void ModelSelectionButton(string buttonId);
}
