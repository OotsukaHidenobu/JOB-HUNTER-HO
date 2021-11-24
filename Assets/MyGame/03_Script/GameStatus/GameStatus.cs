using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStatus : MonoBehaviour
{
    [SerializeField, Tooltip("ゲームオーバーUI")] GameObject gameOverUI = default;
    [SerializeField, Tooltip("プレイヤー")] GameObject player = default;

    [SerializeField] FadeScene fadeScene = default;

    [SerializeField] FloorAnimation floorAnimation = default;
    PlayerStatus playerStatus;

    bool onShot = true;

    void Start()
    {
        playerStatus = player.GetComponent<PlayerStatus>();

        // FindObjectOfType<FloorAnimation>().SetMode(FloorAnimation.Mode.Safe);
        floorAnimation.SetMode(FloorAnimation.Mode.Safe);
    }

    void Update()
    {
        if (playerStatus.PlayerDead)
        {
            if (onShot)
            {
                gameOverUI.SetActive(true);
                FadeScene instFade = Instantiate(fadeScene);
                onShot = false;
            }
        }
    }
}
