using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

//セミオートガンの継承元
public class SemiAutoGunTest : MonoBehaviour
{
    [SerializeField, Tooltip("武器名")] string gunName = default;
    [SerializeField, Tooltip("武器の愛称")] string haveGunName = default;
    [SerializeField, Tooltip("弾のプレハブ")] protected GameObject bullet = default;
    [SerializeField, Tooltip("射撃間隔(秒)")] float firingInterval = 1;
    [SerializeField, Tooltip("リロード時間(秒)")] float reloadtime = 3;
    [SerializeField, Tooltip("合計弾数(全部で何発持てるか)")] protected int totalAmmo = 36;
    [SerializeField, Tooltip("マガジン弾数(何発でリロードしなかければいけないか)")] int magazineAmmo = 6;

    [SerializeField, Tooltip("拡散半径(ぶれ)")] protected float diffusionR = 200;
    [SerializeField, Tooltip("弾丸の速度")] protected float speed = 100;

    [SerializeField, Tooltip("攻撃力")] protected float power = 1;

    protected Transform muzzle = default;

    //弾丸回復アイテム
    protected PlayerStatus playerStatus;

    protected Animator animator;

    //合計弾数テキスト
    GameObject totalAmmoLabel;
    Text totalAmmoLabelText;
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

    //武器入手時のＵＩ
    GameObject haveLabel;
    RectTransform haveLabelRect;
    TextMeshProUGUI haveLabelText;

    //弾薬入手時のUI
    GameObject haveAmmoLabel;
    RectTransform haveAmmoLabelRect;
    TextMeshProUGUI haveAmmoLabelText;

    //弾薬マックス用のUI
    RectTransform ammoMaxUI;

    //持っている武器の名前
    GameObject haveGunLabel;
    TextMeshProUGUI haveGunLabelText;


    //カウントダウン用変数
    protected float firingIntervalCountdown;
    protected float reloadTimeCountdown;
    protected float magazineAmmoCountdown;
    protected float totalAammoCountdown;

    //リロードボタンが押されたか
    protected bool reloadPush = false;

    protected bool isShot = false;

    protected bool reloadSound = true;
    protected bool isReloadSound = false;

    //レイヤーマスク用のint
    protected int layerInt = 1 << 0 | 1 << 18 | 1 << 20 | 1 << 21;
    private void Awake()
    {
        // totalAmmoLabel = GameObject.Find("TotalAmmo").gameObject;
        // totalAmmoLabelText = totalAmmoLabel.GetComponent<Text>();
        ammoLabel = GameObject.Find("Ammo").gameObject;
        ammoLabelText = ammoLabel.GetComponent<TextMeshProUGUI>();
        magazineLabel = GameObject.Find("Magazine").gameObject;
        magazineLabelText = magazineLabel.GetComponent<TextMeshProUGUI>();
        reloadLabel = GameObject.Find("Reload").gameObject;
        reloadLabelSlider = reloadLabel.transform.Find("Fill").GetComponent<Image>();
        reloadChildImage = reloadLabel.GetComponentsInChildren<Image>();

        haveLabel = GameObject.Find("HaveText").gameObject;
        haveLabelRect = GameObject.Find("HaveImage").GetComponent<RectTransform>();
        haveLabelText = haveLabel.GetComponent<TextMeshProUGUI>();

        haveAmmoLabel = GameObject.Find("HaveAmmoText").gameObject;
        haveAmmoLabelRect = GameObject.Find("HaveAmmoImage").GetComponent<RectTransform>();
        haveAmmoLabelText = haveAmmoLabel.GetComponent<TextMeshProUGUI>();

        haveGunLabel = GameObject.Find("GunName").gameObject;
        haveGunLabelText = haveGunLabel.GetComponent<TextMeshProUGUI>();

        ammoMaxUI = GameObject.Find("AmmoMaxUI").GetComponent<RectTransform>();

        muzzle = GameObject.FindWithTag("Muzzle").transform;
        playerStatus = GameObject.Find("01_kohaku_B").GetComponent<PlayerStatus>();
        animator = GameObject.Find("01_kohaku_B").GetComponent<Animator>();
    }
    protected virtual void Start()
    {
        firingIntervalCountdown = 0;
        reloadTimeCountdown = reloadtime;
        magazineAmmoCountdown = magazineAmmo;
        totalAammoCountdown = totalAmmo;

        //アイテムを所持した際のＵＩ表示
        haveLabelText.text = "you got " + gunName;
        DOTween.KillAll(true);
        haveLabelRect.anchoredPosition = new Vector3(0, 450, 0);
        haveLabelRect.transform.DOLocalMoveY(157, 1).SetEase(Ease.OutBack).OnComplete(() =>
        {
            haveLabelRect.transform.DOLocalMoveY(450, 1).SetEase(Ease.InQuint).SetDelay(1);
        });

        haveAmmoLabelRect.anchoredPosition = new Vector3(0, 450, 0);
    }


    protected virtual void Update()
    {
        if (Time.timeScale == 0 || playerStatus.PlayerDead) return;

        //弾薬回復アイテムを取得した際の処理
        if (totalAammoCountdown != totalAmmo && playerStatus.AmmoRecover)
        {
            Destroy(playerStatus.AmmoItem);
            AudioManager.Instance.PlaySE(AUDIO.SE_HANDGUN_READY, gameObject);
            totalAammoCountdown = totalAmmo;
            //UI表示
            haveAmmoLabelText.text = gunName;
            haveAmmoLabelRect.transform.DOLocalMoveY(157, 1).SetEase(Ease.OutBack).OnComplete(() =>
            {
                haveAmmoLabelRect.transform.DOLocalMoveY(450, 1).SetEase(Ease.InQuint).SetDelay(1);
            });
            playerStatus.AmmoRecover = false;
        }
        //弾薬がマックスだったら
        if (totalAammoCountdown == totalAmmo && playerStatus.AmmoRecover)
        {
            playerStatus.AmmoRecover = false;
            ammoMaxUI.localScale = Vector3.one;
            Vector3 offset = new Vector3(0, 0.5f, 0);
            ammoMaxUI.transform.position = playerStatus.AmmoItem.transform.position + offset;
            AudioManager.Instance.PlaySE(AUDIO.SE_FULL, gameObject);
        }
        //マガジンが最大じゃないときにリロードボタンを押したら、リロードがオンになる
        if (Input.GetButtonDown("X Button") && magazineAmmoCountdown != magazineAmmo && totalAammoCountdown != magazineAmmoCountdown)
        {
            reloadPush = true;
        }
        //射撃間隔のカウントダウン
        firingIntervalCountdown -= Time.deltaTime;
        //射撃間隔が0になるかつ、マガジン弾数が0じゃないかつ、合計弾薬が0以上かつ、リロードされていなかったら
        if (firingIntervalCountdown <= 0 && magazineAmmoCountdown > 0 && totalAammoCountdown >= 0 && reloadPush == false)
        {
            //射撃ボタンを押したら弾を撃つ
            if (Input.GetButtonDown("Shot"))
            {
                isShot = true;
                magazineAmmoCountdown--;
                totalAammoCountdown--;
                firingIntervalCountdown = firingInterval;
            }
        }
        //マガジン弾数が0になるかリロードをすると
        else if (magazineAmmoCountdown <= 0 || reloadPush)
        {
            //合計弾数が0の時に射撃をすると
            if (totalAammoCountdown <= 0 && Input.GetButtonDown("Shot"))
            {
                AudioManager.Instance.PlaySE(AUDIO.SE_SE_GUN_KACHI01, gameObject);
            }
            if (totalAammoCountdown <= 0) return;
            //リロードボタン用のマガジン弾数を0
            magazineAmmoCountdown = 0;
            //リロード時間のカウントダウン
            reloadTimeCountdown -= Time.deltaTime;

            if (reloadSound)
            {
                reloadSound = false;
                isReloadSound = true;
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
                //合計弾数の数がマガジンの数を下回ったら
                if (totalAammoCountdown < magazineAmmo)
                {
                    //リロード後の弾の数を合計弾数にする
                    magazineAmmoCountdown = totalAammoCountdown;
                    //リロード時間のリセット
                    reloadTimeCountdown = reloadtime;
                    //リロードのOFF
                    reloadPush = false;

                    reloadSound = true;
                    isReloadSound = false;
                    animator.SetBool("Reload", false);
                }
                //通常リロード
                else
                {
                    //マガジン弾薬のリセット
                    magazineAmmoCountdown = magazineAmmo;
                    //リロード時間のリセット
                    reloadTimeCountdown = reloadtime;
                    //リロードのOFF
                    reloadPush = false;

                    reloadSound = true;
                    isReloadSound = false;
                    animator.SetBool("Reload", false);
                }

            }
        }

        //リロードによるUI表示切替
        if (reloadSound)
        {
            //非リロード時の処理
            //マガジンの描画
            magazineLabelText.enabled = true;
            // reloadLabel.SetActive(false);
            reloadLabel.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
        }
        else
        {
            //リロード中の処理
            //マガジンの非表示
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

    protected virtual void FixedUpdate()
    {
        //弾数などのUI表示
        magazineLabelText.text = magazineAmmoCountdown.ToString();
        ammoLabelText.text = totalAammoCountdown.ToString() + "/" + totalAmmo.ToString();
        // totalAmmoLabelText.text = "/" + totalAmmo.ToString();
        Vector3 scale = Vector3.one;
        scale.x = Mathf.Lerp(1, 0, reloadTimeCountdown / reloadtime);
        reloadLabelSlider.transform.localScale = scale;

        haveGunLabelText.text = haveGunName;
    }
}
