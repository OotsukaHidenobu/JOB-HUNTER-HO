using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCircleDiffusionAttack : MonoBehaviour
{
    [SerializeField, Tooltip("EnemyBullet")] GameObject bullet = default;
    [SerializeField, Tooltip("攻撃間隔")] float attackInterval = default;
    [SerializeField, Tooltip("弾のスピード")] float bulletSpeed = default;

    [SerializeField, Tooltip("リロードまでの弾")] float ammo = default;
    [SerializeField, Tooltip("リロード時間")] float reloadtime = default;

    [SerializeField, Tooltip("拡散角度")] float diffusionAngle = 45;

    [SerializeField, Tooltip("縦方向の拡散角度")] float diffusionVerticalAngle = 45;

    [SerializeField, Tooltip("拡散数")] int diffusionBullets = 10;

    [SerializeField, Tooltip("攻撃力")] float power = 1;

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
        if (Time.timeScale == 0) return;
    }

    private void FixedUpdate()
    {
        //=================================================================================
        //攻撃
        //=================================================================================
        //攻撃間隔ごとに弾を撃つ
        attackIntervalCountdown -= Time.deltaTime;
        if (attackIntervalCountdown <= 0 && ammoCountdown > 0)
        {
            Shot();
            //弾消費
            ammoCountdown--;
            attackIntervalCountdown = attackInterval;
            AudioManager.Instance.PlaySE(AUDIO.SE_ENEMY_SHOOT);

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

        //プレイヤーの方向を銃口が見る
        transform.LookAt(player.transform.position);
    }

    void Shot()
    {
        float m_OneShotAngle = 0;
        if (diffusionBullets - 1 > 0)
        {
            m_OneShotAngle = diffusionAngle / (diffusionBullets - 1);
        }


        for (int i = 0; i < diffusionBullets; i++)
        {
            Quaternion rotate = Quaternion.Euler(0, -diffusionAngle / 2, 0) * transform.rotation * Quaternion.Euler(0, m_OneShotAngle * i, 0);

            rotate = Quaternion.Euler(-diffusionVerticalAngle, rotate.eulerAngles.y, 0);

            Vector3 direction = Quaternion.Euler(rotate.eulerAngles) * Vector3.forward;

            BulletPoolInstantate.Instance.InstBulletRigidbodyMove(bullet, transform.position, rotate, direction, bulletSpeed, power);
        }

    }
}
