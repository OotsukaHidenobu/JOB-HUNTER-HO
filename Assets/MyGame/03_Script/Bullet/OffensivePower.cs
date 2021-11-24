using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//攻撃力
public class OffensivePower : MonoBehaviour
{
    public float power = default;
    public float Power { get; private set; }
    void Start()
    {
        Power = power;
    }

    private void Update()
    {
        Power = power;
    }
}
