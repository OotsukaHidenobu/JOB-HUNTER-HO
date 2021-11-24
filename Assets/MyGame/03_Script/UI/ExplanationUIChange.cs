using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//操作説明のUIを切り替える
public class ExplanationUIChange : MonoBehaviour
{
    bool oneShot1 = true;
    bool oneShot2 = true;

    float timeCount = 0.5f;
    void Update()
    {
        if (timeCount > 0)
        {
            timeCount -= Time.deltaTime;
            return;
        }


        if (CurrentDevice.anyKey)
        {
            if (oneShot1)
            {
                AudioManager.Instance.PlaySE(AUDIO.SE_UI_SCI_FI, gameObject, 0.3f, 0, 50);
                oneShot1 = false;
                oneShot2 = true;
            }
            transform.Find("PadImage").gameObject.SetActive(false);
            transform.Find("PadImage").gameObject.GetComponent<Animator>().enabled = false;
            transform.Find("KeyImage").gameObject.SetActive(true);
            transform.Find("KeyImage").gameObject.GetComponent<Animator>().enabled = true;


            transform.Find("Background1").gameObject.SetActive(true);
            transform.Find("Background1").gameObject.GetComponent<Animator>().enabled = true;
            transform.Find("Background2").gameObject.SetActive(false);
            transform.Find("Background2").gameObject.GetComponent<Animator>().enabled = false;

        }
        else
        {
            if (oneShot2)
            {
                AudioManager.Instance.PlaySE(AUDIO.SE_UI_SCI_FI, gameObject, 0.3f, 0, 50);
                oneShot1 = true;
                oneShot2 = false;
            }
            transform.Find("PadImage").gameObject.SetActive(true);
            transform.Find("PadImage").gameObject.GetComponent<Animator>().enabled = true;
            transform.Find("KeyImage").gameObject.SetActive(false);
            transform.Find("KeyImage").gameObject.GetComponent<Animator>().enabled = false;

            transform.Find("Background1").gameObject.SetActive(false);
            transform.Find("Background1").gameObject.GetComponent<Animator>().enabled = false;
            transform.Find("Background2").gameObject.SetActive(true);
            transform.Find("Background2").gameObject.GetComponent<Animator>().enabled = true;
        }
    }
}
