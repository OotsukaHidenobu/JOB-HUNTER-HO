using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSystem : MonoBehaviour
{
    [SerializeField, Tooltip("ドロップさせたい武器を全て入れる")] GameObject[] weapons = default;
    [SerializeField, Tooltip("回復アイテムを入れる")] GameObject healItem = default;
    [SerializeField, Tooltip("弾薬回復アイテム")] GameObject ammoItem = default;
    [SerializeField, Tooltip("武器のドロップ確率")] const int weaponPercent = 60;
    [SerializeField, Tooltip("回復アイテムのドロップ確率")] const int healPercent = 60;
    [SerializeField, Tooltip("弾薬回復アイテムのドロップ確率")] const int ammoPercent = 20;

    //どの武器を選択するか
    int rand = 0;

    /// <summary>
    /// ドロップ処理
    /// </summary>
    /// <param name="pos">出現させる場所</param>
    /// <param name="weaponP">武器のドロップ確率</param>
    /// <param name="healP">回復アイテムのドロップ確率</param>
    /// <param name="ammoP">弾薬回復アイテムのドロップ確率</param>
    public void Drop(Vector3 pos, int weaponP = weaponPercent, int healP = healPercent, int ammoP = ammoPercent)
    {
        //=================================================================================
        //武器のドロップ
        //=================================================================================

        //武器に使う確率
        int percent1 = Random.Range(0, 100);
        rand = Random.Range(0, weapons.Length);
        Vector3 offset0 = new Vector3(0, -0.5f, 0);
        if (percent1 <= weaponP && weapons.Length >= 1)
        {
            Instantiate(weapons[rand], pos + offset0, Quaternion.identity);

            List<GameObject> list = new List<GameObject>(weapons);
            list.Remove(weapons[rand]);
            list.Remove(null);
            weapons = list.ToArray();
        }

        //=================================================================================
        //回復アイテムのドロップ
        //=================================================================================

        //回復アイテムに使う確率
        int percent2 = Random.Range(0, 100);

        Vector3 offset1 = new Vector3(5, -0.5f, 5);
        if (percent2 <= healP)
        {
            Instantiate(healItem, pos + offset1, Quaternion.Euler(-90, 0, 0));
        }

        //=================================================================================
        //弾薬回復アイテムのドロップ
        //=================================================================================

        //弾薬回復アイテムに使う確率
        int percent3 = Random.Range(0, 100);

        Vector3 offset2 = new Vector3(-5, -0.5f, 5);
        if (percent3 <= ammoP)
        {
            Instantiate(ammoItem, pos + offset2, Quaternion.identity);
        }
    }
}