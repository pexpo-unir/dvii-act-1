using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    private Light _light;

    [SerializeField] private float minIntensity = 0.05f;

    [SerializeField] private float maxIntensity = 0.75f;

    [SerializeField] private float flickerSpeed = 0.1f;

    [SerializeField] private float flickerSpeedChange = 10f;

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

        _light.intensity = Mathf.Lerp(_light.intensity, _targetIntensity, Time.deltaTime * flickerSpeedChange);
    }
}