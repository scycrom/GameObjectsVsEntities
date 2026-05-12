using ECS;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class HudController : MonoBehaviour
{
    private PanelRenderer _panelRenderer;

    private Label _moduleLabel;
    private Label _modeLabel;
    private Label _spawnCountLabel;
    private Label _fpsLabel;
    private Label _cpuLabel;
    private Label _gpuLabel;
    private VisualElement _cubesSection;

    private readonly FrameTiming[] _frameTimings = new FrameTiming[1];

    private const float TimingSmoothing = 0.1f;
    private const float RefreshInterval = 0.25f;
    private float _smoothedDeltaTime;
    private double _smoothedCpuMs;
    private double _smoothedGpuMs;
    private float _refreshTimer;

    private void OnEnable()
    {
        _panelRenderer = GetComponent<PanelRenderer>();
        _panelRenderer.RegisterUIReloadCallback(OnUIReload);

        SpawnerEventChannel.OnSpawn += HandleSpawn;
        SceneSwitcher.ModeChanged += HandleModeChanged;
        SceneSwitcher.ModuleChanged += HandleModuleChanged;
    }

    private void OnDisable()
    {
        if (_panelRenderer != null)
            _panelRenderer.UnregisterUIReloadCallback(OnUIReload);

        SpawnerEventChannel.OnSpawn -= HandleSpawn;
        SceneSwitcher.ModeChanged -= HandleModeChanged;
        SceneSwitcher.ModuleChanged -= HandleModuleChanged;
    }

    private void OnUIReload(PanelRenderer panelRenderer, VisualElement root)
    {
        _moduleLabel     = root.Q<Label>("module-label");
        _modeLabel       = root.Q<Label>("mode-label");
        _spawnCountLabel = root.Q<Label>("spawn-count-label");
        _fpsLabel        = root.Q<Label>("fps-label");
        _cpuLabel        = root.Q<Label>("cpu-label");
        _gpuLabel        = root.Q<Label>("gpu-label");
        _cubesSection    = root.Q<VisualElement>("cubes-section");

        root.Q<Button>("toggle-mode-button")    .clicked += () => SceneSwitcher.Instance?.ToggleMode();
        root.Q<Button>("toggle-module-button")  .clicked += () => SceneSwitcher.Instance?.ToggleModule();
        root.Q<Button>("size-decrement-button") .clicked += () => SpawnerCommon.Enqueue(SizeCommand.Decrement);
        root.Q<Button>("size-increment-button") .clicked += () => SpawnerCommon.Enqueue(SizeCommand.Increment);
        root.Q<Button>("size-halve-button")     .clicked += () => SpawnerCommon.Enqueue(SizeCommand.Halve);
        root.Q<Button>("size-double-button")    .clicked += () => SpawnerCommon.Enqueue(SizeCommand.Double);

        if (SceneSwitcher.Instance != null)
        {
            HandleModeChanged(SceneSwitcher.Instance.CurrentMode);
            HandleModuleChanged(SceneSwitcher.Instance.CurrentModule);
        }
    }

    private void Update()
    {
        SampleTimings();

        _refreshTimer -= Time.unscaledDeltaTime;
        if (_refreshTimer > 0f) return;
        _refreshTimer = RefreshInterval;

        RefreshLabels();
    }

    private void SampleTimings()
    {
        float dt = Time.unscaledDeltaTime;
        _smoothedDeltaTime += (dt - _smoothedDeltaTime) * TimingSmoothing;

        FrameTimingManager.CaptureFrameTimings();
        if (FrameTimingManager.GetLatestTimings((uint)_frameTimings.Length, _frameTimings) <= 0) return;

        _smoothedCpuMs += (_frameTimings[0].cpuFrameTime - _smoothedCpuMs) * TimingSmoothing;
        _smoothedGpuMs += (_frameTimings[0].gpuFrameTime - _smoothedGpuMs) * TimingSmoothing;
    }

    private void RefreshLabels()
    {
        if (_fpsLabel != null)
        {
            float fps = _smoothedDeltaTime > 0f ? 1f / _smoothedDeltaTime : 0f;
            _fpsLabel.text = $"FPS: {fps:F0} ({_smoothedDeltaTime * 1000f:F1} ms)";
        }
        if (_cpuLabel != null) _cpuLabel.text = $"CPU: {_smoothedCpuMs:F2} ms";
        if (_gpuLabel != null) _gpuLabel.text = $"GPU: {_smoothedGpuMs:F2} ms";
    }

    private void HandleSpawn(int count)
    {
        if (_spawnCountLabel != null)
            _spawnCountLabel.text = $"Spawned Cubes: {count}";
    }

    private void HandleModeChanged(SceneSwitcher.SpawnerMode mode)
    {
        if (_modeLabel != null)
            _modeLabel.text = $"Backend: {mode}";
    }

    private void HandleModuleChanged(SceneSwitcher.BenchmarkModule module)
    {
        if (_moduleLabel != null)
            _moduleLabel.text = $"Module: {module}";

        if (_cubesSection != null)
            _cubesSection.style.display = module == SceneSwitcher.BenchmarkModule.Cubes
                ? DisplayStyle.Flex
                : DisplayStyle.None;
    }
}
