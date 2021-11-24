using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundBulletDeath : MonoBehaviour
{
    [SerializeField, Tooltip("弾が死ぬまでの時間")] float destroyTime = 10;
    [SerializeField, Tooltip("パーティクルSparks")] GameObject particle = default;
    Rigidbody Rb = default;
    void Start()
    {
        Rb = GetComponent<Rigidbody>();
        StartCoroutine("Death");
    }

    // Update is called once per frame
    void Update()
    {

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
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("EnemyAndBullet"))
        {
            gameObject.SetActive(false);

            Quaternion rotate = Quaternion.LookRotation(collision.contacts[0].normal);
            ParticleOn(rotate);
        }
    }

    private void OnDisable()
    {
        //Rb.velocity = Vector3.zero;
    }

    //パーティクル生成
    private void ParticleOn(Quaternion rotate)
    {
        BulletPoolInstantate.Instance.InstParticle(particle, transform.position, rotate);
    }
}
