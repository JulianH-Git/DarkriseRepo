using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CamShake : MonoBehaviour
{
    public static CamShake Instance {get; private set;}
    private CinemachineVirtualCamera vCam;
    [SerializeField] private float shakeIntensity = 1.0f;
    [SerializeField] private float shakeTime = 0.2f;

    private float timer;
    private CinemachineBasicMultiChannelPerlin _cbmcp;

    private void Start() {
        StopShake();
    }
    void Awake() {
        Instance = this;
        vCam = GetComponent<CinemachineVirtualCamera>();
    }

    public void Shake(){
        CinemachineBasicMultiChannelPerlin _cbmcp = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = shakeIntensity;

        timer = shakeTime;
    }
    public void StopShake(){
        CinemachineBasicMultiChannelPerlin _cbmcp = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = 0f;
        timer = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if(timer > 0){
            timer -= Time.deltaTime;

            if(timer <= 0){
                StopShake();
            }
        }
    }
}
