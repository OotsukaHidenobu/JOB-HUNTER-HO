using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TitleScene : MonoBehaviour
{
    [SerializeField] FadeScene fadeScene = default;
    [SerializeField, Tooltip("EventSystem")] EventSystem eventSystem = default;

    bool oneShot = true;
    private void Start()
    {
        Time.timeScale = 1;
        AudioManager.Instance.PlayBGM(AUDIO.BGM_TITLE, 0.5f);

        Cursor.visible = false;
        //カーソルのロック
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void ChangeScene()
    {
        if (oneShot)
        {
            AudioManager.Instance.PlaySE(AUDIO.SE_ENTER, 0.3f, 0, 1.3f);
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
