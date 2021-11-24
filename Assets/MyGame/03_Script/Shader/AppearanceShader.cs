using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearanceShader : MonoBehaviour
{

    public float spawnEffectTime = 4;
    public float pause = 3;
    public AnimationCurve fadeIn;

    ParticleSystem ps;
    float timer = 0;

    Material Material;
    void Start()
    {
        Material = GetComponent<Renderer>().material;
        Material.SetFloat("_Progress", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < spawnEffectTime + pause)
        {
            timer += Time.deltaTime;
        }
        else
        {
            //ps.Play();
            timer = 10;
        }

        Material.SetFloat("_Progress", fadeIn.Evaluate(Mathf.InverseLerp(0, spawnEffectTime, timer)));
        if (fadeIn.Evaluate(Mathf.InverseLerp(0, spawnEffectTime, timer)) >= 1)
        {
            Material.SetFloat("_Edge", 0);
        }
    }
}
