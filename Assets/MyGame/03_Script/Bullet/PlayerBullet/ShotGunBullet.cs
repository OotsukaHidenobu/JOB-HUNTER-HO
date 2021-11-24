using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//減速する弾
public class ShotGunBullet : MonoBehaviour
{
    GameObject muzzle;
    Vector3 muzzlePosition;
    [SerializeField] float speed = default;
    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        muzzle = GameObject.FindGameObjectWithTag("Muzzle");
        muzzlePosition = muzzle.transform.position;
    }

    private void FixedUpdate()
    {
        Vector3 direction = muzzlePosition - this.transform.position;
        rb.AddForce(direction * speed, ForceMode.Acceleration);

        if (rb.velocity.magnitude <= 20)
        {
            Destroy(this.gameObject);
        }
    }
    private void OnEnable()
    {
        muzzlePosition = muzzle.transform.position;
    }
    private void OnDisable()
    {
        rb.velocity = Vector3.zero;
    }
}
