using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveTest : MonoBehaviour
{
    [SerializeField]
    private float time = 1f; // 再生時間
    [SerializeField]
    private float waitTime = 1f; // 再生までの待ち時間

    private Material material = null;
    private int _Width = 0;
    private int _Cutoff = 0;

    private float duration = 0f; // 残時間
    private float halfTime = 0f; // 再生時間の半分

    void Start()
    {
        material = GetComponentInChildren<Renderer>().material;
        _Width = Shader.PropertyToID("_Width");
        _Cutoff = Shader.PropertyToID("_CutOff");

        if (material != null)
        {
            material.SetFloat(_Cutoff, 1f);
            material.SetFloat(_Width, 1f);
        }
        halfTime = time / 4f * 3f; // 半分といいつつ4/3にしているのは、見た目の調整のため
        duration = time;
    }

    void Update()
    {
        float delta = Time.deltaTime;

        // 待ち時間
        waitTime -= delta;
        if (waitTime > 0f)
            return;

        duration -= delta;
        if (duration < 0f) duration = 0f;

        // しきい値のアニメーション（再生時間の上半分の時間で1～0に推移）
        float cutoff = (duration - halfTime) / halfTime;
        if (cutoff < 0f) cutoff = 0f;

        // 幅のアニメーション（再生時間の下半分の時間で1～0に推移）
        float width = (halfTime - duration) / halfTime;
        if (width < 0f) width = 0f;
        width = 1f - width;

        // シェーダーに値を渡す
        if (material != null)
        {
            material.SetFloat(_Cutoff, cutoff);
            material.SetFloat(_Width, width);
        }
    }
}
