using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveTracking : MonoBehaviour
{
    [SerializeField, Tooltip("速度")] float speed = default;
    [SerializeField, Tooltip("何秒待つか")] float delay = default;
    Rigidbody rb;

    GameObject player;

    Vector3 targetPos;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");

    }
    private void FixedUpdate()
    {

        if (delay > 0)
        {
            delay -= Time.deltaTime;
            return;
        }
        targetPos = player.transform.position;
        Vector3 direction = targetPos - transform.position;

        rb.AddForce(direction.normalized * speed, ForceMode.Acceleration);

    }
}
