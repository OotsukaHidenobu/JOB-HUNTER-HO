using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ショットガンのような球を打つ敵
public class EnemyDiffusionAttack : MonoBehaviour
{
    [SerializeField, Tooltip("EnemyBullet")] GameObject bullet = default;
    [SerializeField, Tooltip("攻撃間隔")] float attackInterval = default;
    [SerializeField, Tooltip("弾のスピード")] float bulletSpeed = default;

    [SerializeField, Tooltip("リロードまでの弾")] float ammo = default;
    [SerializeField, Tooltip("リロード時間")] float reloadtime = default;

    [SerializeField, Tooltip("弾のぶれる縦の角度")] float diffusionAngleVertical = 15;
    [SerializeField, Tooltip("弾のぶれる横の角度")] float diffusionAngleHorizontal = 15;

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
            AudioManager.Instance.PlaySE(AUDIO.SE_ENEMY_SHOOT2, gameObject);

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
        for (int i = 0; i < diffusionBullets; i++)
        {
            //プレイヤーの位置
            Vector3 targetPos = player.transform.position;

            //弾のブレ幅
            float rand0 = Random.Range(-diffusionAngleVertical, diffusionAngleVertical);
            float rand1 = Random.Range(-diffusionAngleHorizontal, diffusionAngleHorizontal);
            float rand2 = Random.Range(-diffusionAngleVertical, diffusionAngleVertical);



            BulletPoolInstantate.Instance.InstBullet(bullet, transform.position, Quaternion.Euler(rand0, rand1, rand2) * transform.rotation, bulletSpeed, power);
        }

    }
}
