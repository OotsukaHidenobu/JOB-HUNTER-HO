using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletChange : MonoBehaviour
{
    [SerializeField, Tooltip("弾が格納されるオブジェクト")] GameObject bullets = default;

    GameWeaponKind[] gameWeaponKinds;
    List<string> list = new List<string>();

    //部位チェンジボタンを押したか
    bool pushArrow;
    //現在が何番目の武器か
    int equipment;
    int currentGameWeaponKinds;

    //武器を所持したら増える
    [HideInInspector] public int getCount;
    int currentGetCount;
    public List<string> AllWeaponStr { get; private set; }

    void Start()
    {
        equipment = 0;

        gameWeaponKinds = bullets.GetComponentsInChildren<GameWeaponKind>(true);

        currentGameWeaponKinds = gameWeaponKinds.Length;
        currentGetCount = getCount;

        AllWeaponName();
        AllWeaponStr = list;
    }

    void Update()
    {
        if (Time.timeScale == 0) return;


        //武器が増えたら
        if (currentGetCount != getCount)
        {
            gameWeaponKinds = bullets.GetComponentsInChildren<GameWeaponKind>(true);
            AllWeaponName();

            AllWeaponStr = list;

            currentGameWeaponKinds = gameWeaponKinds.Length;
            currentGetCount = getCount;

            GetChangeWeapon();
        }

        if (currentGameWeaponKinds <= 1) return;
        //右矢印が押されたら
        if (GetX_ArrowRDown())
        {
            ChangeWeaponR();
        }
        //左矢印が押されたら
        if (GetX_ArrowLDown())
        {
            ChangeWeaponL();
        }

        if (Input.GetKeyDown("2"))
        {
            ChangeWeaponR();
        }
        if (Input.GetKeyDown("1"))
        {
            ChangeWeaponL();
        }

    }

    //所持している武器の種類の名前をリストに登録
    public void AllWeaponName()
    {
        list.Clear();
        foreach (GameWeaponKind kind in gameWeaponKinds)
        {
            list.Add(kind.weaponKind.ToString());
        }
    }
    //所持した武器に変更
    void GetChangeWeapon()
    {
        equipment = currentGameWeaponKinds - 1;

        for (int i = 0; i < gameWeaponKinds.Length; i++)
        {
            if (i == equipment)
            {
                gameWeaponKinds[i].gameObject.SetActive(true);
            }
            else
            {
                gameWeaponKinds[i].gameObject.SetActive(false);
            }
        }
    }

    //武器の切り替え
    void ChangeWeaponR()
    {
        equipment++;
        if (equipment >= gameWeaponKinds.Length)
        {
            equipment = 0;
        }
        //　武器を切り替え
        for (int i = 0; i < gameWeaponKinds.Length; i++)
        {
            if (i == equipment && gameWeaponKinds.Length > 1)
            {
                gameWeaponKinds[i].gameObject.SetActive(true);
                AudioManager.Instance.PlaySE(AUDIO.SE_CHANGE, 0.4f);
            }
            else
            {

                gameWeaponKinds[i].gameObject.SetActive(false);
            }
        }
    }

    //武器を逆に切り替え
    void ChangeWeaponL()
    {
        equipment--;
        if (equipment < 0)
        {
            equipment = gameWeaponKinds.Length - 1;
        }
        //　武器を切り替え
        for (int i = 0; i < gameWeaponKinds.Length; i++)
        {
            if (i == equipment && gameWeaponKinds.Length > 1)
            {
                gameWeaponKinds[i].gameObject.SetActive(true);
                AudioManager.Instance.PlaySE(AUDIO.SE_CHANGE, 0.4f);
            }
            else
            {
                gameWeaponKinds[i].gameObject.SetActive(false);
            }
        }
    }

    //パッドの右十字ボタンが押されたら
    bool GetX_ArrowRDown()
    {
        if (Input.GetAxisRaw("X_Arrow") == 1)
        {
            if (pushArrow == false)
            {
                pushArrow = true;
                return true;
            }
        }

        if (Input.GetAxisRaw("X_Arrow") == 0)
        {
            pushArrow = false;
        }
        return false;
    }

    ////パッドの左十字ボタンが押されたら
    bool GetX_ArrowLDown()
    {
        if (Input.GetAxisRaw("X_Arrow") == -1)
        {
            if (pushArrow == false)
            {
                pushArrow = true;
                return true;
            }
        }

        if (Input.GetAxisRaw("X_Arrow") == 0)
        {
            pushArrow = false;
        }
        return false;
    }
}
