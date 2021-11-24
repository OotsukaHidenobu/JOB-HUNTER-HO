using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollEmission : MonoBehaviour
{
    [SerializeField]
    Vector2 speed = default;

    private float time;

    private Renderer _renderer;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        time += Time.deltaTime;

        _renderer.material.SetFloat("_ScrollX", time * speed.x);
        _renderer.material.SetFloat("_ScrollY", time * speed.y);
    }
}
