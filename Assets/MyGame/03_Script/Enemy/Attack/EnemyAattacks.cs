using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAattacks : MonoBehaviour
{
    public bool attackEnd = false;
    /// <summary>
    /// ランダム射撃
    /// </summary>
    /// <param name="position">自身のポジション</param>
    /// <param name="player">プレイヤー</param>
    /// <param name="bulletPrefab">弾のプレハブ</param>
    /// <param name="bulletSpeed">弾の速度</param>
    /// <param name="diffusionR">どれだけぶれるか</param>
    /// <param name="attackInterval">攻撃間隔</param>
    /// <param name="attackIntervalCountdown">攻撃間隔のカウント</param>
    /// <param name="ammo">撃てる弾数</param>
    /// <param name="ammoCountdown">撃てる弾数のカウント</param>
    /// <param name="reloadtime">リロード時間</param>
    /// <param name="reloadTimeCountdown">リロード時間のカウント</param>
    public void RandomShooting(Vector3 position, Vector3 targetPos, GameObject bulletPrefab, float bulletSpeed, float diffusionR,
     float attackInterval, float attackIntervalCountdown, float ammo, float ammoCountdown, float reloadtime, float reloadTimeCountdown)
    {
        attackEnd = false;
        //プレイヤーの位置

        //弾のブレ幅
        float rand0 = Random.Range(-1.0f, 1.0f);
        float rand1 = Random.Range(-1.0f, 1.0f);
        float r = Random.Range(-diffusionR, diffusionR);

        Vector3 diffusion = new Vector3(rand0, rand1, 0).normalized * r;

        targetPos.x = targetPos.x + diffusion.x;

        //プレイヤーの方向
        Vector3 targetDirection = targetPos - position;


        //攻撃間隔ごとに弾を撃つ
        attackIntervalCountdown -= Time.deltaTime;
        if (attackIntervalCountdown <= 0 && ammoCountdown > 0)
        {
            GameObject bullets = Instantiate(bulletPrefab, position, Quaternion.LookRotation(targetPos)) as GameObject;
            bullets.GetComponent<Rigidbody>().velocity = targetDirection.normalized * bulletSpeed;
            //弾消費
            ammoCountdown--;
            attackIntervalCountdown = attackInterval;


        }
        else if (ammoCountdown <= 0)
        {
            reloadTimeCountdown -= Time.deltaTime;
            if (reloadTimeCountdown <= 0)
            {
                ammoCountdown = ammo;
                reloadTimeCountdown = reloadtime;
                attackEnd = true;
            }

        }
    }
}
