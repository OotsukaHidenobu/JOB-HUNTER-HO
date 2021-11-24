using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//ボタンをアクティブにする
public class ActivateButton : MonoBehaviour
{

    [SerializeField, Tooltip("最初にフォーカスするゲームオブジェクト")] private GameObject firstSelect = default;
    [SerializeField] EventSystem eventSystem = default;

    GameObject selectedObj;
    private void Start()
    {
        //最初にイベントシステムで選択状態にするオブジェクトを選択
        EventSystem.current.SetSelectedGameObject(firstSelect);
    }
    private void OnEnable()
    {
        //前回の選択状態を選択
        EventSystem.current.SetSelectedGameObject(selectedObj);
    }
    private void Update()
    {
        if (eventSystem.currentSelectedGameObject == null) return;
        //前回アクティブになっていたオブジェクトの保存
        selectedObj = eventSystem.currentSelectedGameObject.gameObject;
    }

}
