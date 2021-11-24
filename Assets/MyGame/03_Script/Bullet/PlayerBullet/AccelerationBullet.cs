using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationBullet : MonoBehaviour
{
    [HideInInspector] public float maxSpeed = 50; //最大速度
    [HideInInspector] public float accelerationDuration = 5;//何秒で最大か
    [HideInInspector] public AnimationCurve curve;

    Rigidbody rb;

    float startTime;

    float currentSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        startTime = Time.time;
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;
        float rate = (Time.time - startTime) / accelerationDuration;
        currentSpeed = curve.Evaluate(rate) * maxSpeed;
        this.transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }
    private void OnEnable()
    {
        startTime = Time.time;
    }
    private void OnDisable()
    {
        maxSpeed = 0;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        //rb.velocity = Vector3.zero;
    }
}
