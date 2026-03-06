using UnityEngine;
using UnityEngine.UI;

public class KeepUIElementProportions  : MonoBehaviour
{
    private void Awake()
    {
        var canvas = GetComponentInParent<CanvasScaler>();
        var rt = GetComponent<RectTransform>();
        rt.localScale = new Vector2(canvas.referenceResolution.x / Screen.width, 1);
    }
}
