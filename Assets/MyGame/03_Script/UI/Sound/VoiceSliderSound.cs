using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceSliderSound : MonoBehaviour
{
    float startValue;

    bool onShot = true;

    public void Sound()
    {
        if (onShot)
        {
            onShot = false;
            return;
        }
        AudioManager.Instance.PlayVoice(AUDIO.Voice_V0004, Camera.main.gameObject, 0.6f);
    }
}
