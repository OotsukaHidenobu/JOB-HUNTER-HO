using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

//選ばれたテキストのアルファ値をあげる
public class SelectHighlightText : MonoBehaviour
{
    [SerializeField, Tooltip("EventSystem")] EventSystem eventSystem = default;

    [SerializeField, Tooltip("一つ目のテキスト")] TextMeshProUGUI TextPre1 = default;
    [SerializeField, Tooltip("一つ目のアンダーライン")] Image underLine1 = default;
    [SerializeField, Tooltip("一つ目のボタンの名前")] string TextStr1 = default;
    [SerializeField, Tooltip("二つ目のテキスト")] TextMeshProUGUI TextPre2 = default;
    [SerializeField, Tooltip("二つ目のアンダーライン")] Image underLine2 = default;
    [SerializeField, Tooltip("二つ目のボタンの名前")] string TextStr2 = default;

    [SerializeField, Tooltip("三つ目のテキスト")] TextMeshProUGUI TextPre3 = default;
    [SerializeField, Tooltip("三つ目のアンダーライン")] Image underLine3 = default;


    //選ばれているオブジェクトの格納
    GameObject selectedObj;

    void Start()
    {
        underLine1.transform.localScale = new Vector3(0, 1, 1);
        underLine2.transform.localScale = new Vector3(0, 1, 1);
        underLine3.transform.localScale = new Vector3(0, 1, 1);
        TextPre3.color = new Color(1, 1, 1, 0.3529412f);
    }

    void Update()
    {
        if (eventSystem.currentSelectedGameObject == null) return;
        //現在選ばれている
        selectedObj = eventSystem.currentSelectedGameObject.gameObject;

        //一つ目が選ばれていたら
        if (selectedObj.name == TextStr1)
        {
            TextPre1.color = new Color(1, 1, 1, 1);
            TextPre2.color = new Color(1, 1, 1, 0.3529412f);


            Vector3 scale = underLine1.transform.localScale;
            scale.x = Mathf.Lerp(scale.x, 1f, 20f * Time.unscaledDeltaTime);
            underLine1.transform.localScale = scale;
            Vector3 scale2 = underLine2.transform.localScale;
            scale2.x = Mathf.Lerp(scale2.x, 0f, 20f * Time.unscaledDeltaTime);
            underLine2.transform.localScale = scale2;
        }
        //二つ目が選ばれていたら
        else if (selectedObj.name == TextStr2)
        {
            TextPre2.color = new Color(1, 1, 1, 1);
            TextPre1.color = new Color(1, 1, 1, 0.3529412f);
            TextPre3.color = new Color(1, 1, 1, 0.3529412f);

            Vector3 scale = underLine2.transform.localScale;
            scale.x = Mathf.Lerp(scale.x, 1f, 20f * Time.unscaledDeltaTime);
            underLine2.transform.localScale = scale;
            Vector3 scale2 = underLine1.transform.localScale;
            scale2.x = Mathf.Lerp(scale2.x, 0f, 20f * Time.unscaledDeltaTime);
            underLine1.transform.localScale = scale2;
            underLine3.transform.localScale = scale2;
        }
        else
        {
            TextPre3.color = new Color(1, 1, 1, 1);
            TextPre2.color = new Color(1, 1, 1, 0.3529412f);

            Vector3 scale = underLine3.transform.localScale;
            scale.x = Mathf.Lerp(scale.x, 1f, 20f * Time.unscaledDeltaTime);
            underLine3.transform.localScale = scale;
            Vector3 scale2 = underLine1.transform.localScale;
            scale2.x = Mathf.Lerp(scale2.x, 0f, 20f * Time.unscaledDeltaTime);
            underLine2.transform.localScale = scale2;
        }
    }
}
