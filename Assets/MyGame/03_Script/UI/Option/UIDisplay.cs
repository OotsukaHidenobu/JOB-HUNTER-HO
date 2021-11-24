using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//UIを表示させるためのクラス
public class UIDisplay : MonoBehaviour
{
    [SerializeField, Tooltip("表示させたいUI")] GameObject[] selectMenu = default;

    //表示
    public void Display(int num)
    {
        //表示させたいUIをアクティブに
        selectMenu[num].SetActive(true);
        //DOTweeによる拡大
        selectMenu[num].transform.DOScale(1, 0.2f).SetEase(Ease.OutQuad).SetUpdate(true);

        AudioManager.Instance.PlaySE(AUDIO.SE_UI_DECISION);

        //このスクリプトをアタッチしているオブジェクトを非アクティブに
        this.gameObject.SetActive(false);
    }

    //確認用Panel表示
    public void ConfirmationDisplay(int num)
    {
        //表示させたいUIをアクティブに
        selectMenu[num].SetActive(true);
        //DOTweeによる拡大
        selectMenu[num].transform.DOScale(0.37f, 0.2f).SetEase(Ease.OutQuad).SetUpdate(true);

        AudioManager.Instance.PlaySE(AUDIO.SE_UI_DECISION);

        //このスクリプトをアタッチしているオブジェクトを非アクティブに
        this.gameObject.SetActive(false);
    }
}
