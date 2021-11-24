using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapUI : MonoBehaviour
{
    [SerializeField] float countSpeed = default;
    float alpha;
    RawImage rawImage;
    bool check = false;
    public float maxAlpha = 0.7f;
    public bool CheckProperty { get; set; }
    void Start()
    {
        CheckProperty = check;
        rawImage = gameObject.GetComponent<RawImage>();
    }
    void Update()
    {

        if (CheckProperty && alpha >= 0)
        {
            alpha -= Time.deltaTime * countSpeed;
        }
        else if (CheckProperty == false && alpha <= maxAlpha)
        {
            alpha += Time.deltaTime * countSpeed;
        }
        alpha = Mathf.Clamp(alpha, 0, maxAlpha);
        rawImage.color = new Color(1, 1, 1, alpha);
    }
}
