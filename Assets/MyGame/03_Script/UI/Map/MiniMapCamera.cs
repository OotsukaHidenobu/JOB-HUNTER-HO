using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    [SerializeField, Tooltip("プレイヤー")] Transform playerPrefab = default;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position = new Vector3(playerPrefab.position.x, 100, playerPrefab.position.z);
    }
}
