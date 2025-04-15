using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    private Light _light;

    private float _flicker;
    public float minIntensity = 0.05f;
    public float maxIntensity = 0.75f;
    public float flickerSpeed = 0.1f;

    private float _nextFlickerTime = 0f;
    private float _targetIntensity;

    private void Awake()
    {
        _light = GetComponent<Light>();
    }

    private void Start()
    {
        _targetIntensity = Random.Range(minIntensity, maxIntensity);
    }

    private void Update()
    {
        if (Time.time >= _nextFlickerTime)
        {
            _targetIntensity = Random.Range(minIntensity, maxIntensity);
            _nextFlickerTime = Time.time + flickerSpeed;
        }

        _light.intensity = Mathf.Lerp(_light.intensity, _targetIntensity, Time.deltaTime * 10f);
    }
}