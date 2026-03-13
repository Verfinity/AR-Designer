using UnityEngine;

public abstract class Identity : MonoBehaviour
{
    public string Id { get; private set; } = string.Empty;

    public void SetIdentity(string id)
    {
        if (Id != string.Empty)
            return;

        Id = id;
    }
}
