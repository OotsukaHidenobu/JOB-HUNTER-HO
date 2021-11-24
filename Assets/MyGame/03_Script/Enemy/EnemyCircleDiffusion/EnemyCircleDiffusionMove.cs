using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCircleDiffusionMove : MonoBehaviour
{
    [SerializeField, Tooltip("インパクト")] float impact = default;
    [SerializeField, Tooltip("何秒待つか")] float delay = default;
    float delayCount;
    Rigidbody rb;
    GameObject player;
    Vector3 targetPos;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        delayCount = delay;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        delayCount -= Time.deltaTime;
        if (delayCount < 0)
        {
            targetPos = player.transform.position;
            Vector3 direction = targetPos - transform.position;

            rb.AddForce(direction.normalized * impact, ForceMode.VelocityChange);

            delayCount = delay;

        }


    }
}
