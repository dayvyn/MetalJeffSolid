using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class CameraMovement : MonoBehaviour
{
    CinemachineVirtualCamera cam;
    PlayerController player;
    private void OnEnable()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        player = FindAnyObjectByType<PlayerController>();
        player.CrouchedEvent += CameraShift;
    }

    private void OnDisable()
    {
        player.CrouchedEvent -= CameraShift;
    }

    void CameraShift(bool lowered)
    {
        if (lowered)
        {
            var lerpCamOffset = Mathf.LerpUnclamped(12.18f, 1, 1);
            cam.GetCinemachineComponent<Cinemachine3rdPersonFollow>().ShoulderOffset.y = lerpCamOffset;
        }
        if (!lowered)
        {
            var lerpCamOffset = Mathf.LerpUnclamped(1, 12.18f, 1);
            cam.GetCinemachineComponent<Cinemachine3rdPersonFollow>().ShoulderOffset.y = lerpCamOffset;
        }
    }
}
