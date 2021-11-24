using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class PlayerStatus : MonoBehaviour
{
    float hp;
    [SerializeField, Tooltip("無敵時間")] float invincible = default;

    [SerializeField, Tooltip("healthBarHUDTester")] HealthBarHUDTester healthBarHUDTester = default;

    [SerializeField, Tooltip("PlayerDamageVol")] PostProcessVolume playerDamageVol = default;
    private PostProcessProfile postProcessDamageProfile;
    Vignette damageVignette;
    [SerializeField, Tooltip("PlayerHealVol")] PostProcessVolume playerHealVol = default;

    [SerializeField, Tooltip("HealthMaxUI")] RectTransform healthMaxUI = default;
    [SerializeField, Tooltip("BossSpawnControl")] BossSpawnControl bossSpawnControl = default;

    [SerializeField, Tooltip("LifeGauge")] Image LifeGauge = default;

    [SerializeField, Tooltip("HaveWeapon")] GameObject haveWeapon = default;
    PostProcessProfile postProcessHealProfile;
    Vignette healVignette;
    float healCount;
    float invincibleCount;
    bool invincibleOn = false;
    bool healOn = false;

    PlayerStatsHP playerStatsHP;

    Animator animator;
    Rigidbody[] ragdollRigidbodies;

    Rigidbody rb;


    //プレイヤーが死んだかどうか
    public bool PlayerDead { get; set; }

    //弾丸回復アイテムを拾ったか
    public bool AmmoRecover { get; set; }

    //プレイヤーがダメージを受けたか
    public bool PlayerDamage { get; set; }

    //弾薬回復アイテムのオブジェクト
    public GameObject AmmoItem { get; set; }

    public event Func<bool> OnGetAmmo;

    void Start()
    {
        PlayerDead = false;

        invincibleCount = 0;

        AmmoRecover = false;

        PlayerDamage = false;

        playerDamageVol.enabled = false;
        postProcessDamageProfile = playerDamageVol.profile;
        playerHealVol.enabled = false;
        postProcessHealProfile = playerHealVol.profile;

        playerStatsHP = gameObject.GetComponent<PlayerStatsHP>();
        hp = playerStatsHP.Health;

        damageVignette = postProcessDamageProfile.GetSetting<Vignette>();
        healVignette = postProcessHealProfile.GetSetting<Vignette>();

        animator = GetComponent<Animator>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        rb = gameObject.GetComponent<Rigidbody>();

        healthMaxUI.localScale = Vector3.zero;
    }

    void Update()
    {
        if (PlayerDead) return;

        if (invincibleOn)
        {
            invincibleCount -= Time.deltaTime;
            damageVignette.roundness.Override(Mathf.Lerp(0, 1, invincibleCount / invincible));
        }

        if (invincibleCount <= 0)
        {
            playerDamageVol.enabled = false;
            invincibleOn = false;
        }

        if (healOn)
        {
            healCount -= Time.deltaTime;
            healVignette.roundness.Override(Mathf.Lerp(0, 1, healCount));
        }
        if (healCount <= 0)
        {
            healCount = 1;
            playerHealVol.enabled = false;
            healOn = false;
        }

        if (hp <= 0)
        {
            PlayerDead = true;
            AudioManager.Instance.PlayVoice(AUDIO.Voice_V0019, gameObject, 0.6f);
            SetRagdoll();
        }

        //ボス撃破
        if (bossSpawnControl.GameClear)
        {
            gameObject.GetComponent<Player>().enabled = false;
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            gameObject.GetComponent<Jump>().enabled = false;
            GameObject.Find("Main Camera").GetComponent<ThirdPersonOrbitCamBasic>().enabled = false;
            haveWeapon.SetActive(false);
            return;
        }

    }
    //ラグドール化
    void SetRagdoll()
    {
        foreach (Rigidbody rigidbody in ragdollRigidbodies)
        {
            rigidbody.isKinematic = false;
            animator.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //ダメージを受けた時の処理
        if (other.gameObject.CompareTag("EnemyBullet") && invincibleCount <= 0 && bossSpawnControl.GameClear == false || other.gameObject.CompareTag("EnemyAndBullet") && invincibleCount <= 0 && bossSpawnControl.GameClear == false)
        {
            //パワーの値だけHPを減らす
            float power = other.gameObject.GetComponent<OffensivePower>().Power;
            //ダメージを受けたか
            PlayerDamage = true;
            //ダメージ用のポストプロセスを起動
            playerDamageVol.enabled = true;

            hp -= power;
            healthBarHUDTester.Hurt(power);

            int rand = UnityEngine.Random.Range(0, 2);
            if (hp > 0)
            {
                if (rand == 0)
                {
                    AudioManager.Instance.PlayVoice(AUDIO.Voice_V0016, gameObject, 0.6f);
                }
                else
                {
                    AudioManager.Instance.PlayVoice(AUDIO.Voice_V0021, gameObject, 0.6f);
                }
            }


            //ダメージSE
            AudioManager.Instance.PlaySE(AUDIO.SE_DAMAGE);
            LifeGauge.fillAmount = hp / playerStatsHP.MaxHealth;

            //カウントリセット
            invincibleCount = invincible;
            invincibleOn = true;
        }

        //回復アイテムを取得
        if (other.gameObject.CompareTag("HealItem") && hp != playerStatsHP.MaxHealth)
        {
            float heal = 1;
            AudioManager.Instance.PlaySE(AUDIO.SE_HEAL);
            healthBarHUDTester.Heal(heal);
            hp = playerStatsHP.Health;
            healOn = true;
            playerHealVol.enabled = true;
            LifeGauge.fillAmount = hp / playerStatsHP.MaxHealth;
            Destroy(other.gameObject);
            return;
        }
        //体力が最大なら
        if (other.gameObject.CompareTag("HealItem") && hp == playerStatsHP.MaxHealth)
        {
            Vector3 offset = new Vector3(0, 0.5f, 0);

            healthMaxUI.localScale = Vector3.one;
            healthMaxUI.transform.position = other.transform.position + offset;
            AudioManager.Instance.PlaySE(AUDIO.SE_FULL, other.gameObject);
            StartCoroutine(DelayFalse(healthMaxUI));
        }
        //弾薬回復アイテムを取得
        if (other.gameObject.CompareTag("AmmoItem"))
        {
            // AmmoRecover = true;
            AmmoItem = other.gameObject;
            if (OnGetAmmo != null)
            {
                if (OnGetAmmo())
                {
                    Destroy(other.gameObject);
                }
            }
        }

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("EnemyBullet") && invincibleCount <= 0 && bossSpawnControl.GameClear == false || other.gameObject.CompareTag("EnemyAndBullet") && invincibleCount <= 0 && bossSpawnControl.GameClear == false)
        {
            //パワーの値だけHPを減らす
            float power = other.gameObject.GetComponent<OffensivePower>().Power;
            //ダメージを受けたか
            PlayerDamage = true;
            //ダメージ用のポストプロセスを起動
            playerDamageVol.enabled = true;

            hp -= power;
            healthBarHUDTester.Hurt(power);

            int rand = UnityEngine.Random.Range(0, 2);
            if (hp > 0)
            {
                if (rand == 0)
                {
                    AudioManager.Instance.PlayVoice(AUDIO.Voice_V0016, gameObject, 0.6f);
                }
                else
                {
                    AudioManager.Instance.PlayVoice(AUDIO.Voice_V0021, gameObject, 0.6f);
                }
            }

            //ダメージSE
            AudioManager.Instance.PlaySE(AUDIO.SE_DAMAGE);

            LifeGauge.fillAmount = hp / playerStatsHP.MaxHealth;

            //カウントリセット
            invincibleCount = invincible;
            invincibleOn = true;
        }
    }
    IEnumerator DelayFalse(RectTransform rect)
    {
        //1.5秒後に非Active
        yield return new WaitForSeconds(1.5f);
        rect.localScale = Vector3.zero;
    }
}
