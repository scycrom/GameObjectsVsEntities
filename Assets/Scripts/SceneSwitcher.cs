using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages switching between benchmark scenes along two axes:
///   - Module:  Cubes vs. ComplexWorld
///   - Backend: ECS vs. GameObject
/// Tab toggles backend, M toggles module.
/// </summary>
public class SceneSwitcher : MonoBehaviour
{
    public enum SpawnerMode { ECS, GameObject }
    public enum BenchmarkModule { Cubes, ComplexWorld }

    [Header("Cubes module")]
    [SerializeField] private string _cubesEcsScene = "EntitiesScene";
    [SerializeField] private string _cubesGoScene  = "GameObjectsScene";

    [Header("Complex World module")]
    [SerializeField] private string _complexWorldEcsScene = "ComplexWorldEntitiesScene";
    [SerializeField] private string _complexWorldGoScene  = "ComplexWorldGameObjectsScene";

    private SpawnerMode _backend = SpawnerMode.ECS;
    private BenchmarkModule _module = BenchmarkModule.Cubes;
    private string _loadedScene;
    private bool _isSwitching;

    public static SceneSwitcher Instance { get; private set; }
    public static event Action<SpawnerMode> ModeChanged;
    public static event Action<BenchmarkModule> ModuleChanged;

    public SpawnerMode CurrentMode => _backend;
    public BenchmarkModule CurrentModule => _module;

    private void Awake() => Instance = this;

    private void Start()
    {
        _loadedScene = SceneFor(_module, _backend);
        SceneManager.LoadSceneAsync(_loadedScene, LoadSceneMode.Additive);
        ModeChanged?.Invoke(_backend);
        ModuleChanged?.Invoke(_module);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;
        if (kb.tabKey.wasPressedThisFrame) ToggleMode();
        if (kb.mKey.wasPressedThisFrame) ToggleModule();
    }

    public void ToggleMode()
    {
        var next = _backend == SpawnerMode.ECS ? SpawnerMode.GameObject : SpawnerMode.ECS;
        SwitchTo(_module, next);
    }

    public void ToggleModule()
    {
        var next = _module == BenchmarkModule.Cubes ? BenchmarkModule.ComplexWorld : BenchmarkModule.Cubes;
        SwitchTo(next, _backend);
    }

    private void SwitchTo(BenchmarkModule module, SpawnerMode backend)
    {
        if (_isSwitching) return;

        string toLoad = SceneFor(module, backend);
        if (toLoad == _loadedScene && module == _module && backend == _backend) return;

        _isSwitching = true;
        bool moduleChanged = module != _module;
        bool backendChanged = backend != _backend;
        _module = module;
        _backend = backend;

        string toUnload = _loadedScene;
        _loadedScene = toLoad;

        var unloadOp = SceneManager.UnloadSceneAsync(toUnload);
        unloadOp.completed += _ =>
        {
            var loadOp = SceneManager.LoadSceneAsync(toLoad, LoadSceneMode.Additive);
            loadOp.completed += __ => _isSwitching = false;
        };

        if (backendChanged) ModeChanged?.Invoke(_backend);
        if (moduleChanged) ModuleChanged?.Invoke(_module);
    }

    private string SceneFor(BenchmarkModule module, SpawnerMode backend)
    {
        return (module, backend) switch
        {
            (BenchmarkModule.Cubes,        SpawnerMode.ECS)        => _cubesEcsScene,
            (BenchmarkModule.Cubes,        SpawnerMode.GameObject) => _cubesGoScene,
            (BenchmarkModule.ComplexWorld, SpawnerMode.ECS)        => _complexWorldEcsScene,
            (BenchmarkModule.ComplexWorld, SpawnerMode.GameObject) => _complexWorldGoScene,
            _ => _cubesEcsScene,
        };
    }
}
