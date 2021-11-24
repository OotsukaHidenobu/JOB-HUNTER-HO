using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public abstract class BaseGun : MonoBehaviour
{
    [SerializeField, Tooltip("武器名")] protected string gunName = default;
    [SerializeField, Tooltip("武器の愛称")] protected string haveGunName = default;
    [SerializeField, Tooltip("弾のプレハブ")] protected GameObject bullet = default;
    [SerializeField, Tooltip("射撃間隔(秒)")] protected float firingInterval = 1;
    [SerializeField, Tooltip("リロード時間(秒)")] protected float reloadtime = 3;
    [SerializeField, Tooltip("合計弾数(全部で何発持てるか)")] protected int totalAmmo = 36;
    [SerializeField, Tooltip("マガジン弾数(何発でリロードしなかければいけないか)")] protected int magazineAmmo = 6;

    [SerializeField, Tooltip("拡散半径(ぶれ)")] protected float diffusionR = 200;
    [SerializeField, Tooltip("弾丸の速度")] protected float speed = 100;

    [SerializeField, Tooltip("攻撃力")] protected float power = 1;

    protected Transform muzzle = default;

    //弾丸回復アイテム
    protected PlayerStatus playerStatus;

    protected Animator animator;

    //現在の弾数テキスト
    protected GameObject ammoLabel;
    protected TextMeshProUGUI ammoLabelText;
    //マガジンの弾数テキスト
    protected GameObject magazineLabel;
    protected TextMeshProUGUI magazineLabelText;
    //リロード時間のUI
    protected GameObject reloadLabel;
    protected Image reloadLabelSlider;
    protected Image[] reloadChildImage;

    //武器入手時のＵＩ
    protected GameObject haveLabel;
    protected RectTransform haveLabelRect;
    protected TextMeshProUGUI haveLabelText;

    //弾薬入手時のUI
    protected GameObject haveAmmoLabel;
    protected RectTransform haveAmmoLabelRect;
    protected TextMeshProUGUI haveAmmoLabelText;

    //持っている武器の名前
    protected GameObject haveGunLabel;
    protected TextMeshProUGUI haveGunLabelText;

    //弾薬マックス用のUI
    protected RectTransform ammoMaxUI;

    //カウントダウン用変数
    protected float firingIntervalCountdown;
    protected float reloadTimeCountdown;
    protected float magazineAmmoCountdown;
    protected float totalAammoCountdown;

    //リロードボタンが押されたか
    protected bool reloadPush = false;

    protected bool reloadSound = true;

    protected bool isShot = false;

    protected bool isReloadSound = false;

    //レイヤーマスク用のint
    protected int layerInt = 1 << 0 | 1 << 18 | 1 << 20 | 1 << 21;
    protected virtual void Awake()
    {
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
    protected virtual void OnDisable()
    {
        animator.SetBool("Reload", false);
        reloadTimeCountdown = reloadtime;
        playerStatus.OnGetAmmo -= OnGetAmmo;
    }

    private void OnDestroy()
    {
        // 弾薬取得イベントの購読解除
        playerStatus.OnGetAmmo -= OnGetAmmo;
    }
    protected virtual void OnEnable()
    {
        reloadSound = true;
        // 弾薬取得イベントの登録
        if (gameObject.GetComponent<BaseGun>().isActiveAndEnabled)
        {
            playerStatus.OnGetAmmo += OnGetAmmo;
        }
    }

    protected virtual void FixedUpdate()
    {
        //弾数などのUI表示
        magazineLabelText.text = magazineAmmoCountdown.ToString();
        ammoLabelText.text = totalAammoCountdown.ToString() + "/" + totalAmmo.ToString();
        Vector3 scale = Vector3.one;
        scale.x = Mathf.Lerp(1, 0, reloadTimeCountdown / reloadtime);
        reloadLabelSlider.transform.localScale = scale;

        haveGunLabelText.text = haveGunName;

        // 発砲処理
        if (isShot)
        {
            Shot();
            PlayShotSound();
            isShot = false;
        }
        if (isReloadSound)
        {
            PlayReloadSound();
            isReloadSound = false;
        }
    }

    /// <summary>
    /// 弾薬回復アイテムを取得したときの処理。
    /// </summary>
    /// <returns>アイテムを取得するならtrue、取得しないならfalse</returns>
    protected virtual bool OnGetAmmo()
    {
        //弾薬回復アイテムを取得した際の処理
        if (totalAammoCountdown < totalAmmo)
        {
            AudioManager.Instance.PlaySE(AUDIO.SE_HANDGUN_READY, gameObject);
            totalAammoCountdown = totalAmmo;
            //UI表示
            haveAmmoLabelText.text = gunName;
            haveAmmoLabelRect.transform.DOLocalMoveY(157, 1).SetEase(Ease.OutBack).OnComplete(() =>
            {
                haveAmmoLabelRect.transform.DOLocalMoveY(450, 1).SetEase(Ease.InQuint).SetDelay(1);
            });

            return true;
        }
        //弾薬がマックスだったら
        else
        {
            ammoMaxUI.localScale = Vector3.one;
            Vector3 offset = new Vector3(0, 0.5f, 0);
            ammoMaxUI.transform.position = playerStatus.AmmoItem.transform.position + offset;
            AudioManager.Instance.PlaySE(AUDIO.SE_FULL, gameObject);
            return false;
        }
    }

    protected abstract void Shot();

    protected virtual void PlayShotSound()
    {
        AudioManager.Instance.PlaySE(AUDIO.SE_FIREARM_SUB_MACHINE_GUN, gameObject);
    }

    protected virtual void PlayReloadSound()
    {
        AudioManager.Instance.PlaySE(AUDIO.SE_RELOAD, gameObject);
    }
}
