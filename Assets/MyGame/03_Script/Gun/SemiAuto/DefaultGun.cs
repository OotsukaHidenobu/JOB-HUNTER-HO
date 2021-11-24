using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DefaultGun : SemiAutoGun
{
    [SerializeField] Transform muzzlePos = default;
    protected override void Start()
    {
        reloadTimeCountdown = reloadtime;
        magazineAmmoCountdown = magazineAmmo;
    }
    override protected void Update()
    {
        if (Time.timeScale == 0 || playerStatus.PlayerDead) return;

        //マガジンが最大じゃないときにリロードボタンを押したら、リロードがオンになる
        if (Input.GetButtonDown("X Button") && magazineAmmoCountdown != magazineAmmo)
        {
            reloadPush = true;
        }
        //射撃間隔のカウントダウン
        firingIntervalCountdown -= Time.deltaTime;
        //射撃間隔が0になるかつ、マガジン弾数が0じゃないかつ、合計弾薬が0以上かつ、リロードされていなかったら
        if (firingIntervalCountdown <= 0 && magazineAmmoCountdown > 0 && reloadPush == false)
        {
            //射撃ボタンを押したら弾を撃つ
            if (Input.GetButtonDown("Shot"))
            {
                isShot = true;
                magazineAmmoCountdown--;
                firingIntervalCountdown = firingInterval;
            }
        }
        //マガジン弾数が0になるかリロードをすると
        else if (magazineAmmoCountdown <= 0 || reloadPush)
        {
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
                //マガジン弾薬のリセット
                magazineAmmoCountdown = magazineAmmo;
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
    override protected void FixedUpdate()
    {
        if (isShot)
        {
            Shot();
            AudioManager.Instance.PlaySE(AUDIO.SE_FIREARM_SUB_MACHINE_GUN, gameObject);

            isShot = false;
        }
        if (isReloadSound)
        {
            AudioManager.Instance.PlaySE(AUDIO.SE_RELOAD, gameObject);
            isReloadSound = false;
        }

        ammoLabelText.text = "∞/∞";
        magazineLabelText.text = magazineAmmoCountdown.ToString();
        Vector3 scale = Vector3.one;
        scale.x = Mathf.Lerp(1, 0, reloadTimeCountdown / reloadtime);
        reloadLabelSlider.transform.localScale = scale;
        haveGunLabelText.text = haveGunName;
    }

    protected override void Shot()
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
        Vector3 shotDirection = hit.point - muzzlePos.position;
        Quaternion rotatin = Quaternion.LookRotation(shotDirection);
        BulletPoolInstantate.Instance.InstBulletRigidbodyMove(bullet, muzzlePos.position, rotatin, shotDirection, speed, power);

    }
}
