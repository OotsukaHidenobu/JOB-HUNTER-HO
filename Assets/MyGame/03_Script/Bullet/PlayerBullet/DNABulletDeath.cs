using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNABulletDeath : MonoBehaviour
{
    void Update()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf == false)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
