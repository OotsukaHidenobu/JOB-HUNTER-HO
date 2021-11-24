using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : MonoBehaviour
{
    public bool Touch { get; set; }
    private void Awake()
    {
        Touch = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Touch = true;
        }
    }
}
