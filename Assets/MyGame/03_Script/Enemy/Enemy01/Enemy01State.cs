using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01State : MonoBehaviour
{
    [SerializeField, Tooltip("体力")] float hp;
    [SerializeField, Tooltip("EnemyDeathEffect")] GameObject enemyDeathEffect = default;

    Vector3 hitPosition;

    public bool Hit { get; set; }
    void Update()
    {
        //死亡処理
        if (hp <= 0)
        {
            Destroy(this.gameObject);
            AudioManager.Instance.PlaySE(AUDIO.SE_BREAK_FAST_STEREO, gameObject, 0.8f, 0, 50, false, false);
            hitPosition = new Vector3(hitPosition.x, transform.position.y, hitPosition.z);
            Vector3 direction = transform.position - hitPosition;
            Instantiate(enemyDeathEffect, transform.position, Quaternion.LookRotation(direction));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //弾に当たった処理
        if (other.gameObject.CompareTag("Bullet"))
        {
            float power = other.gameObject.GetComponent<OffensivePower>().Power;
            hp -= power;
            hitPosition = other.ClosestPointOnBounds(this.transform.position);
            Hit = true;

            AudioManager.Instance.PlaySE(AUDIO.SE_LANDING_SOUND, gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //弾に当たった処理
        if (collision.gameObject.CompareTag("Bullet"))
        {
            float power = collision.gameObject.GetComponent<OffensivePower>().Power;
            hp -= power;
            AudioManager.Instance.PlaySE(AUDIO.SE_LANDING_SOUND, gameObject);
            Hit = true;
            foreach (ContactPoint point in collision.contacts)
            {
                hitPosition = point.point;
            }
        }
    }

}
