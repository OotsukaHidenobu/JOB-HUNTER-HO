using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthMaxUI : MonoBehaviour
{
    RectTransform healthMaxUI;

    private void Start()
    {
        healthMaxUI = GameObject.Find("HealthMaxUI").GetComponent<RectTransform>();
    }
    private void Update()
    {
        StartCoroutine(DelayFalse(healthMaxUI));
    }
    IEnumerator DelayFalse(RectTransform rect)
    {
        //1.5秒後に非Active
        yield return new WaitForSeconds(1.5f);
        rect.localScale = Vector3.zero;
    }
}
