using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxRotation : MonoBehaviour
{
    [SerializeField, Range(0.01f, 0.1f), Tooltip("回転スピード")] float rotateSpeed = default;
    [SerializeField, Tooltip("SkyBox")] Material sky = default;

    float rotationRepeatValue;

    void Update()
    {

        rotationRepeatValue = Mathf.Repeat(sky.GetFloat("_Rotation") + rotateSpeed, 360f);

        sky.SetFloat("_Rotation", rotationRepeatValue);

    }
}
