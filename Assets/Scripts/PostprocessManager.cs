using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostprocessManager : MonoBehaviour
{
    [SerializeField] private Volume ppVolume;

    [SerializeField] private Vector2 breathingAberrationRangeInMenu = new(0.0f, 0.15f);

    [SerializeField] private float breathingAberrationVariation = 0.25f;

    private ChromaticAberration _chromaticAberration;

    [SerializeField] private float aberrationFlickerSpeed = 0.1f;

    private float _nextFlickerTime = 0f;

    private float _targetIntensity;
    
    private bool _gameplayStarted;

    private void Awake()
    {
        ppVolume.profile.TryGet(out _chromaticAberration);
    }

    private void Update()
    {
        if (_gameplayStarted)
        {
            return;
        }

        if (Time.time >= _nextFlickerTime)
        {
            _targetIntensity = Random.Range(breathingAberrationRangeInMenu.x, breathingAberrationRangeInMenu.y);
            _nextFlickerTime = Time.time + aberrationFlickerSpeed;
        }

        _chromaticAberration.intensity.value =
            Mathf.Lerp(_chromaticAberration.intensity.value, _targetIntensity, Time.deltaTime * 10f);
    }

    public void StartGameplay()
    {
        _gameplayStarted = true;
        _chromaticAberration.intensity.value = 0f;
    }

    public void UpdateChromaticAberration(float normalizedIntensity)
    {
        if (!_chromaticAberration)
        {
            return;
        }

        float intensity = Mathf.Lerp(1.0f, 0.0f, normalizedIntensity);
        _chromaticAberration.intensity.value = intensity;
    }
}