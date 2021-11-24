using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//アイテムを拾う際の表示
public class GatherUI : MonoBehaviour
{
    private RectTransform myRectTfm;

    void Start()
    {
        myRectTfm = GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector3 cameraTra = Camera.main.transform.position;
        cameraTra.y = transform.position.y;
        // 自身の向きをカメラに向ける
        myRectTfm.LookAt(cameraTra);
    }
}
