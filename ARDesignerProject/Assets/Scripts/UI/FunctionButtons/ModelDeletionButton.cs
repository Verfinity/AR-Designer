using UnityEngine;
using UnityEngine.UI;

public class ModelDeletionButton : MonoBehaviour
{
    [SerializeField]
    private Button _button;

    private GlobalEvents _globalEvents;

    private void Awake()
    {
        _globalEvents = GlobalEvents.GetInstance();
    }

    private void OnClick()
    {
        _globalEvents.DeleteModel?.Invoke();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClick);
    }
}
