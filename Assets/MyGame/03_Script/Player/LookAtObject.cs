using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    Animator animator;
    Vector3 targetPos;

    public GameObject targetObject;       // テスト用のオブジェクト（敵）

    void Start()
    {
        this.animator = GetComponent<Animator>();
        this.targetPos = targetObject.transform.position;
    }

    void Update()
    {
        this.targetPos = targetObject.transform.position;
    }

    // IK Passにチェックをつけると使える。
    private void OnAnimatorIK(int layerIndex)
    {
        this.animator.SetLookAtWeight(1f, 0.3f, 1f, 0f, 0.5f);     // LookAtの調整
        this.animator.SetLookAtPosition(this.targetPos);          // ターゲットの方向を向くよ
    }
}
