using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01Move : MonoBehaviour
{
    enum State
    {
        Wait,
        Far,
        Near,
        Middle
    }
    State currentState;

    [SerializeField] float moveSpeed = default;//移動スピード

    [SerializeField, Tooltip("どの距離まで離れるか")] float leaveDistance = 20;

    [SerializeField, Tooltip("どの距離から近づくか")] float closerDistance = 30;

    GameObject player;

    Vector3 targetPos;

    int random;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentState = State.Wait;
        random = Random.Range(0, 1);

        //プレイヤーの位置
        targetPos = player.transform.position;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Far:
                Far();
                break;

            case State.Near:
                Near();
                break;

            case State.Middle:
                Middle();
                break;
        }
        //=================================================================================
        //移動
        //=================================================================================
        //プレイヤーの位置
        targetPos = player.transform.position;
        //プレイヤーと自身の距離
        float distance = Vector3.Distance(transform.position, targetPos);
        //距離がleaveDistanceより近かったら離れる
        if (leaveDistance > distance)
        {
            currentState = State.Far;
        }
        //距離がcloserDistanceより遠かったら近づく
        else if (distance > closerDistance)
        {
            currentState = State.Near;
        }
        //距離がちょうど間だったら右か左に動く
        else
        {
            currentState = State.Middle;
        }

        //上下には向かせない
        //targetPos.y = transform.position.y;

        //プレイヤーの方向を見る
        transform.LookAt(targetPos);


    }
    void Far()
    {
        transform.position = transform.position + (-transform.forward) * moveSpeed * Time.deltaTime;
        RandomChoice();
    }
    void Near()
    {
        transform.position = transform.position + transform.forward * moveSpeed * Time.deltaTime;
        RandomChoice();
    }
    void Middle()
    {
        if (random == 0)
        {
            transform.position = transform.position + transform.right * moveSpeed * Time.deltaTime;
            transform.position = transform.position + transform.forward * moveSpeed * Time.deltaTime / 4;
        }
        else
        {
            transform.position = transform.position + (-transform.right) * moveSpeed * Time.deltaTime;
            transform.position = transform.position + transform.forward * moveSpeed * Time.deltaTime / 4;
        }
    }
    //ランダムの値を再振り分け
    void RandomChoice()
    {
        random = Random.Range(0, 2);
    }
}
