using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AwakeSensitivity : MonoBehaviour
{
    Slider sensitivitySlider;
    private void Awake()
    {
        sensitivitySlider = gameObject.GetComponent<Slider>();
        sensitivitySlider.value = SensitivityControl.sensitivity;
    }
}
