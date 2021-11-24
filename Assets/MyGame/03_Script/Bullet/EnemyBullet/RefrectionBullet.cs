using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefrectionBullet : MonoBehaviour
{
    [SerializeField, Tooltip("何回バウンドできるか")] int boundCounter = 1;
    int boundCountdown;
    private Vector3 lastVelocity;//速度ベクトル
    private Rigidbody rb;//Rigidbody用

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //カウント用の値の初期化
        boundCountdown = boundCounter;
    }

    void Update()
    {
        //最後のvelocityの値を保存
        lastVelocity = rb.velocity;
        //かうんと
        if (boundCountdown < 0)
        {
            gameObject.SetActive(false);
        }
    }


    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("Floor"))//壁と当たった時
        {
            Reflection(coll);
        }
        if (coll.gameObject.CompareTag("Wall"))//破壊出来る壁に当たった時
        {
            Reflection(coll);
        }
        if (coll.gameObject.CompareTag("Player"))
        {
            //rb.isKinematic = true;
            gameObject.SetActive(false);
        }
    }

    //反射の処理
    void Reflection(Collision collision)
    {
        //反射ベクトル計算
        Vector3 refrectVec = Vector3.Reflect(lastVelocity, collision.contacts[0].normal);
        transform.rotation = Quaternion.LookRotation(refrectVec);
        rb.velocity = refrectVec;
        boundCountdown--;
    }
    private void OnDisable()
    {
        boundCountdown = boundCounter;
    }
}
