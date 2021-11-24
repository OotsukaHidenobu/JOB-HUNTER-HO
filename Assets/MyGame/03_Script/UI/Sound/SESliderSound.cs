using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SESliderSound : MonoBehaviour
{

    bool onShot = true;

    public void Sound()
    {
        if (onShot)
        {
            onShot = false;
            return;
        }
        AudioManager.Instance.PlaySE(AUDIO.SE_UI_SELECT);
    }
}
