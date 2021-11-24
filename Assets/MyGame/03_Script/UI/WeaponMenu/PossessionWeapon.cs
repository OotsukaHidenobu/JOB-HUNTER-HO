using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//WeaponsMenu用のスクリプト
public class PossessionWeapon : MonoBehaviour
{

    [SerializeField, Tooltip("BulletChange")] BulletChange bulletChange = default;
    [System.Serializable]
    public class WeaponMenu
    {
        public WeaponKind name;
        public GameObject weaponButton;

    }

    [SerializeField, Tooltip("全武器の数")] WeaponMenu[] weaponMenus = default;


    //所持武器Menuのリスト
    List<string> weaponMenuNameList = new List<string>();
    //プレイヤーが持っている武器のリスト
    List<string> allWeaponStrList = new List<string>();

    //プレイヤーが持っている武器の配列
    string[] allWeaponStrArray;

    //カウント用変数
    int allWeaponStrArrayCount;
    int currentAllWeaponStrCount;
    private void Awake()
    {
        //すべての武器ボタンを非表示にする
        foreach (WeaponMenu i in weaponMenus)
        {
            i.weaponButton.SetActive(false);
            //weaponMenuNameListにWeaponMenuのnameをすべて追加
            weaponMenuNameList.Add(i.name.ToString());
        }
        //最初の配列の長さを入れる
        allWeaponStrArrayCount = 1;
        //初期武器を表示
        weaponMenus[0].weaponButton.SetActive(true);
    }


    private void OnEnable()
    {
        //現在所持している武器のリスト
        allWeaponStrList = bulletChange.AllWeaponStr;
        //配列に変換
        allWeaponStrArray = allWeaponStrList.ToArray();
        //現在の所持武器の配列の長さを入れる
        currentAllWeaponStrCount = allWeaponStrArray.Length;

        //新しく武器を所持したら
        if (allWeaponStrArrayCount != currentAllWeaponStrCount)
        {
            //Menuを開くまでに増えた武器の個数
            int weaponCountDifference = currentAllWeaponStrCount - allWeaponStrArrayCount;

            //追加された武器の表示
            for (int i = weaponCountDifference; i > 0; i--)
            {
                for (int j = 0; j < weaponMenus.Length; j++)
                {
                    if (allWeaponStrArray[currentAllWeaponStrCount - i] == weaponMenus[j].name.ToString())
                    {
                        weaponMenus[j].weaponButton.SetActive(true);
                        //Hierarchyの表示順序を最後に
                        weaponMenus[j].weaponButton.transform.SetAsLastSibling();
                    }

                }
            }
            //カウントを現在の値へ
            allWeaponStrArrayCount = currentAllWeaponStrCount;
        }
    }
}
