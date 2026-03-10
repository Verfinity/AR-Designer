using UnityEngine;

public class ModelIdentity : MonoBehaviour
{
    public string ModelId { get; private set; } = string.Empty;
    
    public void SetModelId(string modelId)
    {
        if (ModelId != string.Empty)
            return;

        ModelId = modelId;
    }
}
