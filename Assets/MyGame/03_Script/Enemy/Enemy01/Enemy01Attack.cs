using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01Attack : MonoBehaviour
{
    [SerializeField, Tooltip("EnemyBullet_velocity")] GameObject bullet = default;//攻撃の弾
    [SerializeField, Tooltip("攻撃間隔")] float attackInterval = default;
    [SerializeField, Tooltip("弾のスピード")] float bulletSpeed = default;

    [SerializeField, Tooltip("リロードまでの弾")] float ammo = default;
    [SerializeField, Tooltip("リロード時間")] float reloadtime = default;

    [SerializeField, Tooltip("弾のぶれる具合")] float diffusionR = default;

    //カウントダウン用変数
    float attackIntervalCountdown;
    float reloadTimeCountdown;
    float ammoCountdown;

    GameObject player;


    void Start()
    {
        //プレイヤーの中心値
        player = GameObject.FindGameObjectWithTag("PlayerCenter");

        attackIntervalCountdown = attackInterval;
        reloadTimeCountdown = reloadtime;
        ammoCountdown = ammo;
    }


    void Update()
    {
        //プレイヤーの位置
        Vector3 targetPos = player.transform.position;

        //=================================================================================
        //攻撃
        //=================================================================================

        //弾のブレ幅
        float rand0 = Random.Range(-1.0f, 1.0f);
        float rand1 = Random.Range(-1.0f, 1.0f);
        float r = Random.Range(-diffusionR, diffusionR);

        Vector3 diffusion = new Vector3(rand0, rand1, 0).normalized * r;

        //プレイヤーの方向
        Vector3 targetDirection = targetPos - transform.position + diffusion;


        //攻撃間隔ごとに弾を撃つ
        attackIntervalCountdown -= Time.deltaTime;
        if (attackIntervalCountdown <= 0 && ammoCountdown > 0)
        {
            //弾の生成
            BulletPoolInstantate.Instance.InstBulletRigidbodyMove(bullet, transform.position, Quaternion.LookRotation(targetPos), targetDirection, bulletSpeed, 1);

            //弾消費
            ammoCountdown--;
            //音の再生
            AudioManager.Instance.PlaySE(AUDIO.SE_ENEMY_SHOOT, gameObject);
            attackIntervalCountdown = attackInterval;


        }
        else if (ammoCountdown <= 0)
        {
            reloadTimeCountdown -= Time.deltaTime;
            if (reloadTimeCountdown <= 0)
            {
                ammoCountdown = ammo;
                reloadTimeCountdown = reloadtime;
            }
        }
    }
}
