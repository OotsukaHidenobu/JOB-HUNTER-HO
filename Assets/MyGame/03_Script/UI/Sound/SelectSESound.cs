using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectSESound : MonoBehaviour
{
    public void Sound()
    {
        AudioManager.Instance.PlaySE(AUDIO.SE_UI_SELECT);
    }
}
