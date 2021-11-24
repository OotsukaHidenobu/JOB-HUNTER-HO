using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//マップ表示
public class MapDisplay : MonoBehaviour
{
    [SerializeField] GameObject map = default;

    void Update()
    {
        if (Input.GetButtonDown("Option Button"))
        {
            map.SetActive(!map.activeSelf);
        }
        // if (Input.GetButtonUp("Option Button"))
        // {
        //     map.SetActive(false);
        // }
    }
}
