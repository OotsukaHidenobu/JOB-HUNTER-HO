using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//シーン切り替え時にフェード
public class FadeScene : MonoBehaviour
{
    [SerializeField, Tooltip("遷移したいシーン名")] string scene = default;

    [SerializeField, Tooltip("FadeUI")] GameObject fadeUI = default;

    [SerializeField, Tooltip("フェードアウトインターバル")] float fadeOutInterval = 3;
    [SerializeField, Tooltip("フェードインインターバル")] float fadeInInterval = 3;
    [SerializeField, Tooltip("フェードアウトカラー")] Color fadeOutColor = Color.black;
    [SerializeField, Tooltip("フェードインカラー")] Color fadeInColor = Color.black;

    [SerializeField, Tooltip("フェードアウトのルール画像")] Sprite fadeOutSourceImage = default;
    [SerializeField, Tooltip("フェードインのルール画像")] Sprite fadeInSourceImage = default;

    Color fadeColor;

    float fadeAlpha = 0;

    GameObject instFadeUI;
    bool isFading = false;
    Image fadePanel;

    Material material;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        instFadeUI = Instantiate(fadeUI);

        DontDestroyOnLoad(instFadeUI);

        fadePanel = instFadeUI.GetComponent<Image>();

        fadePanel.sprite = fadeOutSourceImage;

        material = fadePanel.material;

        AudioManager.Instance.FadeOutBGM(fadeOutInterval);

    }
    void Start()
    {

        StartCoroutine(TransScene(scene));
    }
    private IEnumerator TransScene(string scene)
    {
        this.isFading = true;
        //フェードアウト
        float time = 0;
        while (time <= fadeOutInterval)
        {
            fadeColor = fadeOutColor;
            this.fadeAlpha = Mathf.Lerp(0f, 1f, time / fadeOutInterval);
            time += Time.unscaledDeltaTime;
            yield return 0;
        }
        //シーン切替 .
        SceneManager.LoadScene(scene);
        //スプライトの変更
        fadePanel.sprite = fadeInSourceImage;
        //フェードイン
        time = 0;
        while (time <= fadeInInterval)
        {
            fadeColor = fadeInColor;
            this.fadeAlpha = Mathf.Lerp(1f, 0f, time / fadeInInterval);
            time += Time.unscaledDeltaTime;
            if (Input.GetButtonDown("Submit"))
            {
                Destroy(instFadeUI);
                Destroy(this.gameObject);
            }
            yield return 0;
        }
        //削除
        Destroy(instFadeUI);
        Destroy(this.gameObject);
    }
    private void Update()
    {
        if (this.isFading)
        {
            //色と透明度を更新
            material.SetColor("_Color", this.fadeColor);
            material.SetFloat("_Alpha", this.fadeAlpha);
        }
    }
}
