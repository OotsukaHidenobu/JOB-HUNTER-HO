using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

public class Player : MonoBehaviour
{
    //移動スピード
    [SerializeField] float speed = 2f;

    //Animatorを入れる
    private Animator animator;

    //Main Cameraを入れる
    [SerializeField] Transform cam = default;

    //Rigidbodyを入れる
    Rigidbody rb;
    //Capsule Colliderを入れる
    CapsuleCollider caps;

    PlayerStatus playerStatus;

    Jump jump;

    float x = default;
    float y = default;
    void Start()
    {
        //Animatorコンポーネントを取得
        animator = GetComponent<Animator>();

        //Rigidbodyコンポーネントを取得
        rb = GetComponent<Rigidbody>();
        //Jumpコンポーネントを取得
        jump = GetComponent<Jump>();
        //RigidbodyのConstraintsを3つともチェック入れて
        //勝手に回転しないようにする
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        playerStatus = GetComponent<PlayerStatus>();
        //CapsuleColliderコンポーネントを取得
        caps = GetComponent<CapsuleCollider>();
        //CapsuleColliderの中心の位置を決める
        caps.center = new Vector3(0, 0.76f, 0);
        //CapsuleColliderの半径を決める
        caps.radius = 0.23f;
        //CapsuleColliderの高さを決める
        caps.height = 1.6f;
    }

    // Update is called once per frame
    void Update()
    {
        //ポーズ時または死亡時は処理しない
        if (Time.timeScale == 0 || playerStatus.PlayerDead) return;

        //カメラを正面向きに向かす
        transform.rotation = Quaternion.Euler(
            new Vector3(transform.rotation.x, cam.eulerAngles.y, transform.rotation.z));

        //回避中は移動処理をしない
        if (jump.airing) return;
        //A・Dキー、←→キーで横移動
        x = Input.GetAxis("Horizontal") * speed;

        //W・Sキー、↑↓キーで前後移動
        y = Input.GetAxis("Vertical") * speed;


        //AnimatorControllerのParametersに数値を送って
        //アニメーションを出す
        animator.SetFloat("X", x / speed);
        animator.SetFloat("Y", y / speed);



        //xとzの数値に基づいて移動
        Vector3 velocity = transform.forward * y + transform.right * x;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;
    }
    private void OnDisable()
    {
        rb.velocity = Vector3.zero;
        animator.SetFloat("X", 0);
        animator.SetFloat("Y", 0);
    }
    public void PlayFootstepSE()
    {
        float pitch = 1 + Random.Range(-0.1f, 0.1f);
        if (y > 0)
        {
            AudioManager.Instance.PlaySEPitch(AUDIO.SE_FOOTSTEP_TRAINERS_ASPHALT_RUN_RR2_MONO, gameObject, 0.4f, pitch);
        }
        if (y < 0)
        {
            AudioManager.Instance.PlaySEPitch(AUDIO.SE_FOOTSTEP_TRAINERS_ASPHALT_RUN_RR2_MONO, gameObject, 0.4f, pitch);
        }
    }

    public void PlayFootstepSideSE()
    {
        float pitch = 1 + Random.Range(-0.1f, 0.1f);
        if (x > 0 && y == 0)
        {
            AudioManager.Instance.PlaySEPitch(AUDIO.SE_FOOTSTEP_TRAINERS_ASPHALT_RUN_RR2_MONO, gameObject, 0.4f, pitch);
        }
        if (x < 0 && y == 0)
        {
            AudioManager.Instance.PlaySEPitch(AUDIO.SE_FOOTSTEP_TRAINERS_ASPHALT_RUN_RR2_MONO, gameObject, 0.4f, pitch);
        }
    }

    public void WinChange()
    {
        animator.SetBool("Win2", true);
        AudioManager.Instance.PlayVoice(AUDIO.Voice_V0025B, Camera.main.gameObject, 0.6f);
    }
}