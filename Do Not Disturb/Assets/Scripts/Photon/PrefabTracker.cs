using UnityEngine;

public class PrefabTracker : MonoBehaviour
{
    public ResourceManager resourceManager;

    private void OnDestroy()
    {
        if (resourceManager != null)
        {
            resourceManager.RemovePrefab(gameObject, gameObject);
        }
    }
}
