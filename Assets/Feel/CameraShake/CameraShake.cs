using UnityEngine;
using Cinemachine;
public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera Camera;
    public float shakeIntensity;
    public float shakeTime;

    public float Timer;
    private CinemachineBasicMultiChannelPerlin noise;
    private void Awake()
    {
        Camera = GetComponent<CinemachineVirtualCamera>();
    }
    void Start()
    {
        StopShake();
    }

    public void ShakeCamera(float Intensity, float time)
    {
        shakeIntensity = Intensity;
        Timer = time;
        CinemachineBasicMultiChannelPerlin perlin = Camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = shakeIntensity;
        perlin.m_FrequencyGain = Timer;

        Timer = shakeTime;
    }
    void StopShake()
    {
        CinemachineBasicMultiChannelPerlin perlin = Camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = 0f;
        Timer = 0;
    }
    private void Update()
    {
        if(Timer > 0)
        {
            Timer -= Time.deltaTime;
            if(Timer <= 0)
            {
                StopShake();
            }
        }
    }
}
