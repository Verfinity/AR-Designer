using UnityEngine;

public class ModelSpawnVisualizer : MonoBehaviour
{
    private void OnEnable()
    {
        transform.GetChild(1).gameObject.SetActive(true);
    }
}
