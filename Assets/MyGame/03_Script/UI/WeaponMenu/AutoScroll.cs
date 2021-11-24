using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//自動でスクロールさせる
public class AutoScroll : MonoBehaviour
{
    ScrollRect ScrollRect;

    void Start()
    {
        ScrollRect = GetComponent<ScrollRect>();
    }

    public void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            //もっとも上のUIより現在選択されているUIが上の位置になったら上にスクロール
            if (EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().position.y >= 269.3f)
            {
                ScrollRect.verticalNormalizedPosition = ScrollRect.verticalNormalizedPosition + 0.1f;

            }
            //もっとも下のUIより現在選択されているUIが下の位置になったら下にスクロール
            else if (EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().position.y <= 68.5f)
            {
                ScrollRect.verticalNormalizedPosition = ScrollRect.verticalNormalizedPosition - 0.1f;
            }
        }
    }
}