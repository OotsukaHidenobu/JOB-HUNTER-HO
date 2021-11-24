using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Jump : MonoBehaviour
{
    //public const int MAX_JUMP_COUNT = 2;    // ジャンプできる回数。 
    [SerializeField] float jumpForce = 5.0f;
    [SerializeField] float airForce = 5f;

    private int jumpCount = 0;
    bool isJump = false;
    public bool isAir = false;

    [HideInInspector]
    public bool airing = false;

    Rigidbody rb;

    Animator animator;
    PlayerStatus playerStatus;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        playerStatus = gameObject.GetComponent<PlayerStatus>();
        animator = gameObject.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (playerStatus.PlayerDead || Time.timeScale == 0) return;
        if (jumpCount == 0 && Input.GetButtonDown("Jump"))
        {
            isJump = true;
        }
        if (jumpCount == 1 && Input.GetButtonDown("Jump"))
        {
            isAir = true;
        }

        //回避中
        if (jumpCount == 2)
        {
            airing = true;
        }
    }
    private void FixedUpdate()
    {
        if (isJump)
        {
            AudioManager.Instance.PlayVoice(AUDIO.Voice_V0001, gameObject, 0.6f);
            AudioManager.Instance.PlaySE(AUDIO.SE_FOOTSTEP_TRAINERS_ASPHALT_RUN_RR2_MONO, gameObject, 0.8f);
            // 速度をクリアして2回目のジャンプも1回目と同じ挙動にする。 
            rb.velocity = Vector3.zero;

            // ジャンプさせる。 
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // ジャンプ回数をカウント。 
            jumpCount++;

            animator.SetBool("Jump", true);
            animator.SetBool("Idle", false);

            // ジャンプを許可する。 
            isJump = false;
        }
        if (isAir)
        {
            AudioManager.Instance.PlayVoice(AUDIO.Voice_V0004, gameObject, 0.6f);
            AudioManager.Instance.PlaySE(AUDIO.SE_SE_SWING4, gameObject);
            //A・Dキー、←→キーで横移動
            float x = Input.GetAxisRaw("Horizontal");

            //W・Sキー、↑↓キーで前後移動
            float z = Input.GetAxisRaw("Vertical");

            Vector3 caringForward = new Vector3(x, 0, z);


            rb.velocity = Vector3.zero;


            if (Mathf.Abs(caringForward.x) <= 0.2 && Mathf.Abs(caringForward.z) <= 0.2)
            {
                rb.AddForce(transform.forward * airForce, ForceMode.Impulse);
                animator.SetFloat("RollX", 0);
                animator.SetFloat("RollY", 1);
            }
            else
            {
                rb.AddForce((transform.forward * z + transform.right * x).normalized * airForce, ForceMode.Impulse);

                animator.SetFloat("RollX", x);
                animator.SetFloat("RollY", z);
            }

            jumpCount++;

            animator.SetBool("Roll", true);

            isAir = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 地面との当たり。 
        if (collision.gameObject.CompareTag("Floor"))
        {
            jumpCount = 0;
            animator.SetBool("Jump", false);
            animator.SetBool("Idle", true);
            animator.SetBool("Roll", false);
            airing = false;
            rb.velocity = Vector3.zero;
        }
    }
}
