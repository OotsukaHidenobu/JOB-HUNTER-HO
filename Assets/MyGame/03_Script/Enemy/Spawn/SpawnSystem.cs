using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSystem : MonoBehaviour
{
    [SerializeField, Tooltip("出現させたい敵を入れる")] GameObject[] Enemys = default;

    [SerializeField, Tooltip("どの敵が何体出現するか")] int[] HowMany = default;

    [SerializeField, Tooltip("何秒間隔で出現するか")] float AppearanceCount = default;
    float AppearanceCount0;

    int Max;
    int MaxCount;

    void Start()
    {
        AppearanceCount0 = AppearanceCount;
        for (int i = 0; i < HowMany.Length; i++)
        {
            Max += HowMany[i];
        }

        //InvokeRepeating("Generate", 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Max <= MaxCount) return;
        AppearanceCount -= Time.deltaTime;
        if (AppearanceCount < 0)
        {
            for (int i = 0; i < Enemys.Length; i++)
            {
                Generate(i);
            }
            AppearanceCount = AppearanceCount0;
            MaxCount++;
        }
    }

    void Generate(int i)
    {
        //=================================================================================
        //敵の出現システム
        //=================================================================================
        float x_size = transform.lossyScale.x / 2 + transform.position.x;
        float x_size_minus = -transform.lossyScale.x / 2 + transform.position.x;
        float z_size = transform.lossyScale.z / 2 + transform.position.z;
        float z_size__minus = -transform.lossyScale.z / 2 + transform.position.z;
        float x = Random.Range(x_size_minus, x_size);
        float y = 1;
        float z = Random.Range(z_size__minus, z_size);
        Instantiate(Enemys[i], new Vector3(x, y, z), Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

        }
    }
}
