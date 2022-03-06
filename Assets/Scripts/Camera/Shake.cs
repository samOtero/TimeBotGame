using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public float shakeTimer;
    public float shakeTimerTotal;
    public float totalIntensity;
    public float bombIntensity;
    public float bombTime;
    public CinemachineVirtualCamera virtualCamera;
    public CinemachineBasicMultiChannelPerlin cameraMultiChannelPerlin;
    public IntEvent CameraShakeBombEvent;
    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        cameraMultiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Start()
    {
        CameraShakeBombEvent.RegisterListener(ShakeCameraBomb);
    }

    private void OnDisable()
    {
        CameraShakeBombEvent.UnregisterListener(ShakeCameraBomb);
    }

    public int ShakeCameraBomb(int value)
    {
        ShakeCamera(bombIntensity, bombTime);
        return value;
    }

    public void ShakeCamera(float intensity, float time)
    {
        cameraMultiChannelPerlin.m_AmplitudeGain = intensity;
        shakeTimer = time;
        shakeTimerTotal = time;
        totalIntensity = intensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer > 0f)
            {
                cameraMultiChannelPerlin.m_AmplitudeGain = 
                    Mathf.Lerp(totalIntensity, 0f, 1.0f-(shakeTimer/shakeTimerTotal));
            }
            else
            {
                cameraMultiChannelPerlin.m_AmplitudeGain = 0f;
            }
        }
    }
}
