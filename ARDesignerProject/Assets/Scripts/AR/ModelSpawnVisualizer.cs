using UnityEngine;

public class ModelSpawnVisualizer : MonoBehaviour
{
    private void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
