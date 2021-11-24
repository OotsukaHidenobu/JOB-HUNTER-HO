using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//キャンセルボタンを押すとイベントシステムのキャンセル
public class CancelButton : MonoBehaviour
{
    [SerializeField, Tooltip("キャンセルボタンを押したときに表示させたいUI")] GameObject selectMenu = default;

    RectTransform rectTransform;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void Display()
    {
        selectMenu.SetActive(true);
        //現在のUIのスケールを０にして非アクティブに
        transform.DOScale(0, 0).OnComplete(() => this.gameObject.SetActive(false)).SetUpdate(true);
        AudioManager.Instance.PlaySE(AUDIO.SE_UI_CANCEL);

        gameObject.GetComponent<ActivateButton>().enabled = false;

    }
    private void OnEnable()
    {
        gameObject.GetComponent<ActivateButton>().enabled = true;
    }
}
