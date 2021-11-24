using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    //状態列挙
    enum State
    {
        Wait,
        Shot01,
        Shot02,
        Shot03,
        Shot04,
        Dead
    }

    //現在の状態
    State currentState;
    //移動速度
    [SerializeField, Tooltip("移動速度")] float moveSpeed = 8;

    //=================================================================================
    //攻撃01
    //=================================================================================

    [Header("乱射攻撃")]
    [SerializeField, Tooltip("EnemyBullet_velocity")] GameObject bulletPrefab01 = default;//攻撃の弾
    [SerializeField, Tooltip("攻撃間隔")] float attackInterval01 = 0.1f;
    [SerializeField, Tooltip("弾のスピード")] float bulletSpeed01 = 15;

    [SerializeField, Tooltip("撃てる弾数")] float ammo01 = 50;
    [SerializeField, Tooltip("リロード時間")] float reloadtime01 = 2;

    [SerializeField, Tooltip("弾のぶれる具合")] float diffusionR01 = 10;

    [SerializeField, Tooltip("攻撃力")] float power01 = 1;

    //カウントダウン用変数
    float attackIntervalCountdown01;
    float reloadTimeCountdown01;
    float ammoCountdown01;

    //=================================================================================
    //攻撃02
    //=================================================================================
    [Header("波状攻撃")]
    [SerializeField, Tooltip("EnemyBullet")] GameObject bulletPrefab02 = default;
    [SerializeField, Tooltip("攻撃間隔")] float attackInterval02 = 1;
    [SerializeField, Tooltip("弾のスピード")] float bulletSpeed02 = 18;

    [SerializeField, Tooltip("撃てる弾数")] float ammo02 = 5;
    [SerializeField, Tooltip("リロード時間")] float reloadtime02 = 2;
    [SerializeField, Tooltip("拡散角度")] float diffusionAngle = 45;
    [SerializeField, Tooltip("拡散弾数")] int diffusionBullet = 16;
    [SerializeField, Tooltip("攻撃力")] float power02 = 1;
    float attackIntervalCountdown02;
    float reloadTimeCountdown02;
    float ammoCountdown02;

    //=================================================================================
    //攻撃03
    //=================================================================================
    [Header("拡散爆発弾攻撃")]
    [SerializeField, Tooltip("EnemyDirectionBullet")] GameObject bulletPrefab03 = default;
    [SerializeField, Tooltip("攻撃間隔")] float attackInterval03 = 1;
    [SerializeField, Tooltip("弾のスピード")] float bulletSpeed03 = 10;

    [SerializeField, Tooltip("撃てる弾数")] float ammo03 = 1;
    [SerializeField, Tooltip("リロード時間")] float reloadtime03 = 5;

    [SerializeField, Tooltip("攻撃力")] float power03 = 1;

    float attackIntervalCountdown03;
    float reloadTimeCountdown03;
    float ammoCountdown03;

    int randomNext = 0;
    int randDesignation = 2;

    GameObject player;


    void Start()
    {
        currentState = State.Wait;

        //プレイヤーの中心値
        player = GameObject.FindGameObjectWithTag("PlayerCenter");

        //攻撃手段01の初期化
        attackIntervalCountdown01 = attackInterval01;
        reloadTimeCountdown01 = reloadtime01;
        ammoCountdown01 = ammo01;

        //攻撃手段02の初期化
        attackIntervalCountdown02 = attackInterval02;
        reloadTimeCountdown02 = reloadtime02;
        ammoCountdown02 = ammo02;

        //攻撃手段03の初期化
        attackIntervalCountdown03 = attackInterval03;
        reloadTimeCountdown03 = reloadtime03;
        ammoCountdown03 = ammo03;

    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Wait:
                Wait();
                break;

            case State.Shot01:
                Shot01();
                break;

            case State.Shot02:
                Shot02();
                break;

            case State.Shot03:
                Shot03();
                break;
            case State.Shot04:
                Shot04();
                break;

            case State.Dead:
                break;
        }
    }

    //それぞれの攻撃手段にランダムに移行
    void Wait()
    {
        int currnetRandomNext = randomNext;
        randomNext = Random.Range(0, 4);
        //前回とは違う値を出す
        while (currnetRandomNext == randomNext)
        {
            randomNext = Random.Range(0, 4);
        }


        int currentRandDesignation = randDesignation;
        randDesignation = Random.Range(0, 3);
        //前回とは違う値を出す
        while (currentRandDesignation == randDesignation)
        {
            randDesignation = Random.Range(0, 3);
        }


        if (randomNext == 0)
        {
            currentState = State.Shot01;
        }
        else if (randomNext == 1)
        {
            currentState = State.Shot02;
        }
        else if (randomNext == 2)
        {
            currentState = State.Shot03;
        }
        else
        {
            currentState = State.Shot04;
        }

    }
    //プレイヤーに近づきながら乱射
    void Shot01()
    {
        RandomShooting();
        Move1(randDesignation);
    }

    //波状に広がる弾を撃つ
    void Shot02()
    {
        WayShooting();
        Move1(randDesignation);
    }
    //拡散爆発弾
    void Shot03()
    {
        EveryDirectionShot();
        Move1(randDesignation);
    }
    //その場で乱射
    void Shot04()
    {
        RandomShooting();


        Vector3 targetPos = player.transform.position;
        targetPos.y = transform.position.y;

        //プレイヤーの方向を見る
        transform.LookAt(targetPos);
    }

    //プレイヤーに近づく移動
    void Move()
    {
        //プレイヤーの位置


        transform.position = transform.position + transform.forward * moveSpeed * Time.deltaTime;
    }
    void Move1(int designation)
    {
        Vector3 targetPos = player.transform.position;
        targetPos.y = transform.position.y;

        //プレイヤーの方向を見る
        transform.LookAt(targetPos);
        if (designation == 0)
        {
            transform.position = transform.position + transform.forward * moveSpeed * Time.deltaTime;
        }
        else if (designation == 1)
        {
            transform.position = transform.position + transform.right * moveSpeed * Time.deltaTime;
        }
        else
        {
            transform.position = transform.position + (-transform.right) * moveSpeed * Time.deltaTime;
        }
    }
    //弾を乱射する
    void RandomShooting()
    {
        //プレイヤーの位置
        Vector3 targetPos = player.transform.position;

        //弾のブレ幅
        float rand0 = Random.Range(-1.0f, 1.0f);
        float rand1 = Random.Range(-1.0f, 1.0f);
        float r = Random.Range(-diffusionR01, diffusionR01);

        Vector3 diffusion = new Vector3(rand0, rand1, 0).normalized * r;

        targetPos.x = targetPos.x + diffusion.x;

        //プレイヤーの方向
        Vector3 targetDirection = targetPos - transform.position;


        //攻撃間隔ごとに弾を撃つ
        attackIntervalCountdown01 -= Time.deltaTime;
        if (attackIntervalCountdown01 <= 0 && ammoCountdown01 > 0)
        {
            //弾の生成
            BulletPoolInstantate.Instance.InstBulletRigidbodyMove(bulletPrefab01, transform.position, Quaternion.LookRotation(targetPos), targetDirection, bulletSpeed01, power01);
            //弾消費
            ammoCountdown01--;
            attackIntervalCountdown01 = attackInterval01;

            AudioManager.Instance.PlaySE(AUDIO.SE_ENEMY_SHOOT, gameObject);
        }
        else if (ammoCountdown01 <= 0)
        {
            reloadTimeCountdown01 -= Time.deltaTime;
            if (reloadTimeCountdown01 <= 0)
            {
                ammoCountdown01 = ammo01;
                reloadTimeCountdown01 = reloadtime01;
                currentState = State.Wait;
            }

        }
    }
    void WayShooting()
    {
        Vector3 targetPos = player.transform.position;
        //targetPos.y = transform.position.y;

        //プレイヤーの方向を見る
        transform.LookAt(targetPos);

        attackIntervalCountdown02 -= Time.deltaTime;
        if (attackIntervalCountdown02 <= 0 && ammoCountdown02 > 0)
        {
            WayShot();
            //弾消費
            ammoCountdown02--;
            attackIntervalCountdown02 = attackInterval02;

            AudioManager.Instance.PlaySE(AUDIO.SE_ENEMY_SHOOT2, gameObject, 0.6f);
        }
        else if (ammoCountdown02 <= 0)
        {
            reloadTimeCountdown02 -= Time.deltaTime;
            if (reloadTimeCountdown02 <= 0)
            {
                ammoCountdown02 = ammo02;
                reloadTimeCountdown02 = reloadtime02;
                currentState = State.Wait;

            }

        }
    }
    //波状に広がる弾
    void WayShot()
    {
        float m_OneShotAngle = 0;
        if (diffusionBullet - 1 > 0)
        {
            m_OneShotAngle = diffusionAngle / (diffusionBullet - 1);
        }


        for (int i = 0; i < diffusionBullet; i++)
        {
            Quaternion rotate = Quaternion.Euler(0, -diffusionAngle / 2, 0) * transform.rotation * Quaternion.Euler(0, m_OneShotAngle * i, 0);

            BulletPoolInstantate.Instance.InstBullet(bulletPrefab02, transform.position, rotate, bulletSpeed02, power02);
        }
    }
    //拡散爆発弾を撃つ
    void EveryDirectionShot()
    {
        //プレイヤーの位置
        Vector3 targetPos = player.transform.position;

        targetPos.y = transform.position.y;

        //プレイヤーの方向を見る
        transform.LookAt(targetPos);

        //プレイヤーの方向
        Vector3 targetDirection = targetPos - transform.position;
        //上に角度をつける
        targetDirection.y += 4;


        //攻撃間隔ごとに弾を撃つ
        attackIntervalCountdown03 -= Time.deltaTime;
        if (attackIntervalCountdown03 <= 0 && ammoCountdown03 > 0)
        {
            BulletPoolInstantate.Instance.InstBulletRigidbodyMove(bulletPrefab03, transform.position, Quaternion.LookRotation(targetPos), targetDirection, bulletSpeed03, power03);
            //弾消費
            ammoCountdown03--;
            attackIntervalCountdown03 = attackInterval03;

            AudioManager.Instance.PlaySE(AUDIO.SE_ENEMY_SHOOT, gameObject);
        }
        else if (ammoCountdown03 <= 0)
        {
            reloadTimeCountdown03 -= Time.deltaTime;
            if (reloadTimeCountdown03 <= 0)
            {
                ammoCountdown03 = ammo03;
                reloadTimeCountdown03 = reloadtime03;
                currentState = State.Wait;
            }

        }
    }
}
