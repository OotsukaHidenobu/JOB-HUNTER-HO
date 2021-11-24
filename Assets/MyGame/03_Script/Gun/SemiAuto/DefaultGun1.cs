using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DefaultGun1 : MonoBehaviour
{
    // 弾丸発射点
    [SerializeField] Transform muzzle = default;
    [SerializeField, Tooltip("弾のプレハブ")] protected GameObject bullet = default;


    [SerializeField, Tooltip("拡散半径")] float diffusionR = 0;
    [SerializeField, Tooltip("弾丸の速度")] float speed = 100;

    [SerializeField, Tooltip("射撃間隔(秒)")] float firingInterval = 0.3f;
    [SerializeField, Tooltip("リロード時間(秒)")] float reloadtime = 0.8f;
    [SerializeField, Tooltip("マガジン弾数(何発でリロードしなかければいけないか)")] int magazineAmmo = 6;

    [SerializeField] float power = 1;

    //合計弾数テキスト
    //GameObject totalAmmoLabel;
    //TextMeshProUGUI totalAmmoLabelText;
    //現在の弾数テキスト
    GameObject ammoLabel;
    TextMeshProUGUI ammoLabelText;
    //マガジンの弾数テキスト
    GameObject magazineLabel;
    TextMeshProUGUI magazineLabelText;
    //リロード時間のUI
    GameObject reloadLabel;
    Image reloadLabelSlider;
    Image[] reloadChildImage;

    //弾薬マックス用のUI
    RectTransform ammoMaxUI;

    //持っている武器の名前
    GameObject haveGunLabel;
    TextMeshProUGUI haveGunLabelText;
    protected string haveGunString;

    PlayerStatus playerStatus;

    Animator animator;

    //カウントダウン用変数
    float firingIntervalCountdown;
    float reloadTimeCountdown;
    float magazinAmmoCountdown;

    //リロードボタンが押されたか
    bool reloadPush = false;

    //リロード用のトリガー
    bool reloadSound = true;

    //レイヤーマスク用のint
    int layerInt = 1 << 0 | 1 << 18 | 1 << 20 | 1 << 21;

    private void Awake()
    {
        //muzzle = GameObject.FindWithTag("Muzzle").transform;

        firingIntervalCountdown = 0;
        reloadTimeCountdown = reloadtime;
        magazinAmmoCountdown = magazineAmmo;

        //totalAmmoLabel = GameObject.Find("TotalAmmo").gameObject;
        //totalAmmoLabelText = totalAmmoLabel.GetComponent<Text>();
        ammoLabel = GameObject.Find("Ammo").gameObject;
        ammoLabelText = ammoLabel.GetComponent<TextMeshProUGUI>();
        magazineLabel = GameObject.Find("Magazine").gameObject;
        magazineLabelText = magazineLabel.GetComponent<TextMeshProUGUI>();
        reloadLabel = GameObject.Find("Reload").gameObject;
        reloadLabelSlider = reloadLabel.transform.Find("Fill").GetComponent<Image>();
        reloadChildImage = reloadLabel.GetComponentsInChildren<Image>();

        haveGunLabel = GameObject.Find("GunName").gameObject;
        haveGunLabelText = haveGunLabel.GetComponent<TextMeshProUGUI>();

        ammoMaxUI = GameObject.Find("AmmoMaxUI").GetComponent<RectTransform>();

        playerStatus = GameObject.Find("01_kohaku_B").GetComponent<PlayerStatus>();
        animator = GameObject.Find("01_kohaku_B").GetComponent<Animator>();

        haveGunString = "HAND GUN";
        //totalAmmoLabelText.text = "/∞";
    }
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0 || playerStatus.PlayerDead) return;
        //弾薬がマックスのUI表示
        if (playerStatus.AmmoRecover)
        {
            playerStatus.AmmoRecover = false;
            ammoMaxUI.localScale = Vector3.one;
            Vector3 offset = new Vector3(0, 0.5f, 0);
            ammoMaxUI.transform.position = playerStatus.AmmoItem.transform.position + offset;
            AudioManager.Instance.PlaySE(AUDIO.SE_FULL, gameObject);
        }
        //マガジンが最大じゃないときにリロードボタンを押したら、リロードがオンになる
        if (Input.GetButtonDown("X Button") && magazinAmmoCountdown != magazineAmmo)
        {
            reloadPush = true;
        }
        //射撃間隔のカウントダウン
        firingIntervalCountdown -= Time.deltaTime;
        //射撃間隔が0になるかつ、マガジン弾数が0じゃないかつ、合計弾薬が0以上かつ、リロードされていなかったら
        if (firingIntervalCountdown <= 0 && magazinAmmoCountdown > 0 && reloadPush == false)
        {
            //射撃ボタンを押したら弾を撃つ
            if (Input.GetButtonDown("Shot"))
            {
                HandGunShot();
                //撃つ音を鳴らす
                AudioManager.Instance.PlaySE(AUDIO.SE_FIREARM_SUB_MACHINE_GUN, gameObject);
                magazinAmmoCountdown--;
                firingIntervalCountdown = firingInterval;
            }
        }
        //マガジン弾数が0になるかリロードをすると
        else if (magazinAmmoCountdown <= 0 || reloadPush)
        {
            //リロードボタン用のマガジン弾数を0
            magazinAmmoCountdown = 0;
            //リロード時間のカウントダウン
            reloadTimeCountdown -= Time.deltaTime;

            if (reloadSound)
            {
                //リロードの音を鳴らす
                AudioManager.Instance.PlaySE(AUDIO.SE_RELOAD, gameObject);
                reloadSound = false;
                //リロードアニメーション起動
                animator.SetBool("Reload", true);
                //リロードアニメーションの時間
                float reloadAnimationTime = 3.18f;
                //リロード速度調整
                float reloadSpeed = reloadAnimationTime / reloadtime;
                animator.SetFloat("Speed", reloadSpeed);
            }

            //リロード時間経ったら
            if (reloadTimeCountdown <= 0)
            {
                //マガジン弾薬のリセット
                magazinAmmoCountdown = magazineAmmo;
                //リロード時間のリセット
                reloadTimeCountdown = reloadtime;
                //リロードのOFF
                reloadPush = false;
                //リロード時のサウンドトリガーをON
                reloadSound = true;
                animator.SetBool("Reload", false);

            }
        }

        if (reloadSound)
        {
            //magazineLabel.SetActive(true);
            magazineLabelText.enabled = true;
            //reloadLabel.SetActive(false);
            reloadLabel.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
        }
        else
        {
            //magazineLabel.SetActive(false);
            magazineLabelText.enabled = false;
            // reloadLabel.SetActive(true);
            reloadLabel.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }

    }
    private void OnDisable()
    {
        animator.SetBool("Reload", false);
        reloadTimeCountdown = reloadtime;
    }

    private void OnEnable()
    {
        reloadSound = true;
    }

    private void FixedUpdate()
    {
        //totalAmmoLabelText.text = "/∞";
        //ammoLabelText.text = "∞";
        ammoLabelText.text = "∞/∞";
        magazineLabelText.text = magazinAmmoCountdown.ToString();
        Vector3 scale = Vector3.one;
        scale.x = Mathf.Lerp(1, 0, reloadTimeCountdown / reloadtime);
        reloadLabelSlider.transform.localScale = scale;
        haveGunLabelText.text = haveGunString;

    }

    void HandGunShot()
    {
        //animator.SetBool("shot", true);

        Vector3 force;

        int layerMaskInt = layerInt;
        //弾のブレ幅
        float rand0 = Random.Range(-1.0f, 1.0f);
        float rand1 = Random.Range(-1.0f, 1.0f);
        float r = Random.Range(-diffusionR, diffusionR);

        Vector3 diffusion = new Vector3(rand0, rand1, 0).normalized * r;

        force = this.gameObject.transform.forward * speed;
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(center + diffusion);

        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 4, false);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000.0f, layerMaskInt))
        {
            //Vector3 shotForward = Vector3.Scale((hit.point - transform.position), new Vector3(1, 1, 1)).normalized;
            //// hit.point が正面方向へRayをとばした際の接触座標.
            //bullets.GetComponent<Rigidbody>().velocity = shotForward * speed;
        }
        Vector3 shotDirection = hit.point - muzzle.position;
        Quaternion rotatin = Quaternion.LookRotation(shotDirection);
        BulletPoolInstantate.Instance.InstBulletRigidbodyMove(bullet, muzzle.position, rotatin, shotDirection, speed, power);

    }
}
