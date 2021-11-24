using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SensitivityControl : MonoBehaviour
{
    public static float sensitivity = 2.4f;

    Slider sensitivitySlider;
    void Start()
    {
        sensitivitySlider = gameObject.GetComponent<Slider>();
    }

    void Update()
    {
        sensitivity = sensitivitySlider.value;
    }
}
