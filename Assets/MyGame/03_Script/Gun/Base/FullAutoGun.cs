using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

//フルオートガンの継承元
public abstract class FullAutoGun : BaseGun
{
    protected virtual void Update()
    {
        if (Time.timeScale == 0 || playerStatus.PlayerDead) return;

        //マガジンが最大じゃないときかつ、現在のマガジンの数と現在の合計弾数が一緒じゃないときにリロードボタンを押したら、リロードがオンになる
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
            if (Input.GetButton("Shot"))
            {
                isShot = true;
                magazineAmmoCountdown--;
                totalAammoCountdown--;
                firingIntervalCountdown = firingInterval;
            }
            if (Input.GetButtonUp("Shot"))
            {
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
}
