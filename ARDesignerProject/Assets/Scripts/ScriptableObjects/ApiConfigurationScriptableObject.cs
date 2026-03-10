using UnityEngine;

[CreateAssetMenu(fileName = "ApiConfiguration", menuName = "ProjectConfiguration/ApiConfiguration")]
public class ApiConfigurationScriptableObject : ScriptableObject
{
    public string ApiUrl;
    public string ApiKey;
}
