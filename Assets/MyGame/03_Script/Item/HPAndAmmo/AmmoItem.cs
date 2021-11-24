using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoItem : MonoBehaviour
{
    RectTransform ammoMaxUI;

    private void Start()
    {
        ammoMaxUI = GameObject.Find("AmmoMaxUI").GetComponent<RectTransform>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DelayFalse(ammoMaxUI));
        }

    }
    IEnumerator DelayFalse(RectTransform rect)
    {
        //1.5秒後に非Active
        yield return new WaitForSeconds(1.5f);
        rect.localScale = Vector3.zero;
    }

    private void OnDestroy()
    {
        ammoMaxUI.localScale = Vector3.zero;
    }
}
