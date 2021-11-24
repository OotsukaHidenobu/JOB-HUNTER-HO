using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//ポーズ画面の表示制御
public class PauseScreen : MonoBehaviour
{
    [SerializeField, Tooltip("PauseUI")] GameObject pauseScreen = default;

    [SerializeField, Tooltip("ポーズ画面の内部")] GameObject[] pauseDetails = default;

    [SerializeField, Tooltip("PlayerStatus")] PlayerStatus playerStatus = default;

    [SerializeField, Tooltip("SpawnAreaBoss")] BossSpawnControl bossSpawn = default;

    [SerializeField, Tooltip("EventSystem")] EventSystem eventSystem = default;

    bool onShot = false;
    void Update()
    {
        if (playerStatus.PlayerDead || bossSpawn.GameClear || eventSystem.enabled == false) return;
        //ポーズ画面を開く
        if (Input.GetButtonDown("Start Button"))
        {
            OpenPause();
            AudioManager.Instance.PlaySE(AUDIO.SE_UI_DECISION);
        }

        //ポーズ画面を閉じる
        if (pauseScreen.activeInHierarchy == false && onShot)
        {
            ExitPause();
            AudioManager.Instance.PlaySE(AUDIO.SE_UI_CANCEL);
            onShot = false;
        }
    }

    //ポーズ画面を開く処理
    void OpenPause()
    {
        Time.timeScale = 0;
        pauseDetails[0].SetActive(true);
        pauseDetails[0].GetComponent<RectTransform>().localScale = Vector3.one;
        pauseScreen.SetActive(!pauseScreen.activeSelf);


        onShot = true;
    }

    //ポーズ画面を閉じる処理
    public void ExitPause()
    {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);

        foreach (GameObject i in pauseDetails)
        {
            i.SetActive(false);
            i.GetComponent<RectTransform>().localScale = Vector3.zero;
        }
    }
}
