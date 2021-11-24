using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//速度を設定できる前方向に進む弾
public class Bullet : MonoBehaviour
{
    [HideInInspector] public float speed;

    //初めのポジション
    private Vector3 currentPosition;

    float theta = 0;
    private void Start()
    {
        currentPosition = transform.position;
    }
    private void Update()
    {
        if (gameObject.name == "EnemyBullet")
        {
            this.transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }

        //インボリュート曲線で広がる挙動
        else if (gameObject.name == "InvoluteBullet")
        {
            //広がりの度合
            float degree = 0.2f;
            theta += Time.deltaTime * speed;
            transform.position = InvoluteOfCircle(degree, theta) + currentPosition;
        }
    }
    private void OnEnable()
    {
        //現在のポジション
        currentPosition = transform.position;
    }
    private void OnDisable()
    {
        theta = 0;
    }
    private float InvoluteOfCircleX(float a, float theta)
    {
        return a * (Mathf.Cos(theta) + theta * Mathf.Sin(theta));
    }
    private float InvoluteOfCircleZ(float a, float theta)
    {
        return a * (Mathf.Sin(theta) - theta * Mathf.Cos(theta));
    }

    private Vector3 InvoluteOfCircle(float a, float theta)
    {
        return new Vector3(
            InvoluteOfCircleX(a, theta), 0,
            InvoluteOfCircleZ(a, theta)
        );
    }
}
