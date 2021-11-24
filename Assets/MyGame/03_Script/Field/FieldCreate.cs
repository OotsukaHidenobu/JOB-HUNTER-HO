using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCreate : MonoBehaviour
{
    [SerializeField] GameObject[] map = default;

    int rand = 0;
    private void Awake()
    {
        rand = Random.Range(0, map.Length);

        for (int i = 0; i < map.Length; i++)
        {
            if (i == rand)
            {
                map[i].SetActive(true);
            }
            else
            {
                Destroy(map[i]);
            }
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
}
