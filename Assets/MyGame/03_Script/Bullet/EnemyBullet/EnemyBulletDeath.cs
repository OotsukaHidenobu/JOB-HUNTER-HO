using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//弾の消滅処理
public class EnemyBulletDeath : MonoBehaviour
{
    [SerializeField, Tooltip("弾が死ぬまでの時間")] float destroyTime = default;
    private void Start()
    {
        StartCoroutine("Death");
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
    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     gameObject.SetActive(false);
    // }
}
