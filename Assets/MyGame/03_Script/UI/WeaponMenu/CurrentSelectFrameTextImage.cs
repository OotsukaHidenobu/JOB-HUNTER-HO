using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class CurrentSelectFrameTextImage : MonoBehaviour
{
    [SerializeField, Tooltip("EventSystem")] EventSystem eventSystem = default;

    [SerializeField, Tooltip("TextParent")] GameObject TextParent = default;

    TextMeshProUGUI weaponDetailsText;
    TextMeshProUGUI weaponNameText;

    Color firstColor;
    //選ばれているオブジェクトの格納
    GameObject selectedObj;
    void Start()
    {
        TextParent.SetActive(true);
        weaponDetailsText = TextParent.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        weaponNameText = TextParent.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (eventSystem.currentSelectedGameObject == null) return;
        //現在選ばれているオブジェクト
        selectedObj = eventSystem.currentSelectedGameObject.gameObject;


        //一つ目が選ばれていたら
        if (selectedObj == this.gameObject)
        {
            weaponDetailsText.color = new Color(1, 1, 1, 1);
            weaponNameText.color = new Color(1, 1, 1, 1);
        }
        //二つ目が選ばれていたら
        else
        {
            weaponDetailsText.color = new Color(1, 1, 1, 0);
            weaponNameText.color = new Color(1, 1, 1, 0);
        }
    }
}
