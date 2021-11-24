using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDeath : MonoBehaviour
{
    [SerializeField, Tooltip("弾が死ぬまでの時間")] float destroyTime = 10;
    [SerializeField, Tooltip("パーティクルSparks")] GameObject particle = default;
    Rigidbody Rb = default;

    private Vector3 lastVelocity;//速度ベクトル

    void Start()
    {
        Rb = GetComponent<Rigidbody>();
        StartCoroutine("Death");
    }

    // Update is called once per frame
    void Update()
    {
        //最後のvelocityの値を保存
        lastVelocity = Rb.velocity;
        // if (Rb.velocity.magnitude <= 20)
        // {
        //     gameObject.SetActive(false);
        // }
    }
    private IEnumerator Death()
    {
        yield return new WaitForSeconds(destroyTime);

        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        StartCoroutine("Death");
    }

    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
        //壁の法線ベクトル
        Quaternion rotate = Quaternion.LookRotation(collision.contacts[0].normal);
        ParticleOn(rotate);
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
        Quaternion rotate = Quaternion.LookRotation((other.transform.position - this.transform.position).normalized);
        ParticleOn(rotate);
    }

    //パーティクル生成
    private void ParticleOn(Quaternion rotate)
    {
        BulletPoolInstantate.Instance.InstParticle(particle, transform.position, rotate);
    }

}
