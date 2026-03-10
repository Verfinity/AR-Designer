using GLTFast;
using System.Threading.Tasks;
using UnityEngine;

public class ModelCreator : MonoBehaviour
{
    public event GlobalEvents.CreationModel UnsetupedModelCreated;

    [SerializeField]
    private Transform _spawnPrefab;

    private GlobalEvents _globalEvents;
    private ModelGenerationEvents _modelGenerationEvents;

    private void Awake()
    {
        _globalEvents = GlobalEvents.GetInstance();
        _modelGenerationEvents = ModelGenerationEvents.GetInstance();
    }

    private async Task CreateModelAsync(string modelId, string modelUrl)
    {
        var gltfImport = new GltfImport();
        var modelLoaded = await gltfImport.Load(modelUrl);
        if (!modelLoaded)
            return;

        var instantiatedParent = Instantiate(_spawnPrefab, transform);
        var modelCreated = await gltfImport.InstantiateMainSceneAsync(instantiatedParent);
        if (!modelCreated)
            return;

        UnsetupedModelCreated?.Invoke(modelId, instantiatedParent.gameObject);
    }

    private async void OnModelGenerated(string modelId, string modelUrl, string modelImageUrl)
    {
        await CreateModelAsync(modelId, modelUrl);
    }

    private void OnEnable()
    {
        _modelGenerationEvents.ModelGenerationSucceeded += OnModelGenerated;
    }

    private void OnDisable()
    {
        _modelGenerationEvents.ModelGenerationSucceeded -= OnModelGenerated;
    }
}
