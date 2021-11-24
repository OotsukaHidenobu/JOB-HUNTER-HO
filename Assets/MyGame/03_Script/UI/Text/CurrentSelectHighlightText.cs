using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

//現在選ばれているボタンのテキストのアルファ値をあげる
public class CurrentSelectHighlightText : MonoBehaviour
{
    [SerializeField, Tooltip("EventSystem")] EventSystem eventSystem = default;
    // [ColorUsage(false, true)] public Color color;
    TextMeshProUGUI Text;
    //選ばれているオブジェクトの格納
    GameObject selectedObj;
    Image underLine;
    void Start()
    {
        Text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        underLine = transform.GetChild(1).GetComponent<Image>();
        underLine.transform.localScale = new Vector3(0, 1, 1);
    }

    void Update()
    {
        if (eventSystem.currentSelectedGameObject == null) return;
        //現在選ばれているオブジェクト
        selectedObj = eventSystem.currentSelectedGameObject.gameObject;

        //自分が選ばれていたら
        if (selectedObj == this.gameObject)
        {
            Text.color = new Color(1, 1, 1, 1);

            Vector3 scale = underLine.transform.localScale;
            scale.x = Mathf.Lerp(scale.x, 1f, 20f * Time.unscaledDeltaTime);
            underLine.transform.localScale = scale;
        }
        //自分以外が選ばれていたら
        else
        {
            Text.color = new Color(1, 1, 1, 0.3529412f);

            Vector3 scale = underLine.transform.localScale;
            scale.x = Mathf.Lerp(scale.x, 0f, 20f * Time.unscaledDeltaTime);
            underLine.transform.localScale = scale;
        }
    }
}
