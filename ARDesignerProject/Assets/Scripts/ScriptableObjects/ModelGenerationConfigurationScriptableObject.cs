using UnityEngine;

[CreateAssetMenu(fileName = "ModelGenerationConfiguration", menuName = "ProjectConfiguration/ModelGenerationConfiguration")]
public class ModelGenerationConfigurationScriptableObject : ScriptableObject
{
    public string ApiUrl;
    public string ApiKey;
}
