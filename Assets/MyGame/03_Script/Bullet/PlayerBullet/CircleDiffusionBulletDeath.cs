using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CircleDiffusionBulletDeath : MonoBehaviour
{
    [SerializeField]
    float deathTime = default;
    [SerializeField]
    float duration = default;

    [SerializeField]
    Vector3 scale = default;

    Rigidbody rb = default;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        transform.DOScale(scale, duration);
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(this.gameObject, deathTime);

    }
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
    }
}
