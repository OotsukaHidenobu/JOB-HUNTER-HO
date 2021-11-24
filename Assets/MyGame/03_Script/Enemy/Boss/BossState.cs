using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossState : MonoBehaviour
{
    public float hp;
    [HideInInspector] public float maxHP;
    [SerializeField, Tooltip("EnemyDeathEffect")] GameObject enemyDeathEffect = default;
    [SerializeField, Tooltip("バーチャルカメラ")] GameObject vcam = default;

    [HideInInspector] public bool clear = false;

    [HideInInspector] public bool bossDestory = false;
    HPBar hpBar;
    Vector3 hitPosition;

    private void Start()
    {
        maxHP = hp;

        hpBar = GameObject.Find("BossHP").GetComponent<HPBar>();
        hpBar.SetEnemy(this);
    }
    void Update()
    {
        if (hp <= 0)
        {
            clear = true;
            //Destroy(this.gameObject);
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            vcam.transform.parent = null;
            //Destroy(hpBar.gameObject);
            GetComponent<BossEnemy>().enabled = false;
            StartCoroutine(Clear(2.0f));
            AudioManager.Instance.PlaySE(AUDIO.SE_CURRENT, gameObject, 0.5f, 0, 50, true, true);
        }
    }
    IEnumerator Clear(float time)
    {
        yield return new WaitForSeconds(time);
        AudioManager.Instance.StopSE(AUDIO.SE_CURRENT, gameObject);
        AudioManager.Instance.PlaySE(AUDIO.SE_SMALL_EXPLOSION1, gameObject, 0.5f, 0, 50, false, false);
        AudioManager.Instance.PlaySE(AUDIO.SE_ENEMYDEATH, gameObject, 0.8f, 0, 50, false, false);
        hitPosition = new Vector3(hitPosition.x, transform.position.y, hitPosition.z);
        Vector3 direction = transform.position - hitPosition;
        Instantiate(enemyDeathEffect, transform.position, Quaternion.LookRotation(direction));
        bossDestory = true;
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            float power = other.gameObject.GetComponent<OffensivePower>().Power;
            hp -= power;
            hpBar.GaugeReduction(power);
            hitPosition = other.ClosestPointOnBounds(this.transform.position);
            AudioManager.Instance.PlaySE(AUDIO.SE_LANDING_SOUND, gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //弾に当たったらダメージ
        if (collision.gameObject.CompareTag("Bullet"))
        {
            float power = collision.gameObject.GetComponent<OffensivePower>().Power;
            hp -= power;
            hpBar.GaugeReduction(power);
            AudioManager.Instance.PlaySE(AUDIO.SE_LANDING_SOUND, gameObject);
            foreach (ContactPoint point in collision.contacts)
            {
                hitPosition = point.point;
            }
        }
    }
}
