using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

//シーン切り替えの処理
public class GamePlayScene : MonoBehaviour
{
    [SerializeField] FadeScene fadeScene = default;
    [SerializeField, Tooltip("EventSystem")] EventSystem eventSystem = default;

    bool oneShot = true;
    private void Start()
    {
        AudioManager.Instance.PlayBGM(AUDIO.BGM_TIMEBEND, 0.25f);
    }
    public void ChangeScene()
    {
        if (oneShot)
        {
            AudioManager.Instance.PlaySE(AUDIO.SE_CURSOR6);
            eventSystem.enabled = false;
            FadeScene instFade = Instantiate(fadeScene);

            oneShot = false;
        }

    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        UnityEngine.Application.Quit();
#endif
    }
}
