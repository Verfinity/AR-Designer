using UnityEngine;

public class ModelSpawnVisualizer : MonoBehaviour
{
    private void OnEnable()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
