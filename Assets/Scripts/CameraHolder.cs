﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CameraHolder : MonoBehaviour
{
    [SerializeField] float positionStrength;
    [SerializeField] float rotationStrength;
    [SerializeField] float shakeDuration;
    [SerializeField] float delay;

    [SerializeField] float timeReset;
    float nextTimeReset;
    public void Update()
    {
        if (Time.time > nextTimeReset)
        {
            nextTimeReset = timeReset + Time.time;
            ResetCameraTransform();
        }
    }
    public void Shake()
    {
        transform.DOShakePosition(shakeDuration, positionStrength, fadeOut: true).SetDelay(delay);
        transform.DOShakeRotation(shakeDuration, rotationStrength, fadeOut: true).SetDelay(delay);
    }

    private void ResetCameraTransform()
    {
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.identity;
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
