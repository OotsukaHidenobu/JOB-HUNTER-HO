using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//アニメーションの変更
public class AnimatorChanger : MonoBehaviour
{
    Animator mainAnimator;
    void Start()
    {
        mainAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            mainAnimator.runtimeAnimatorController = (RuntimeAnimatorController)RuntimeAnimatorController.Instantiate(Resources.Load("Hand Gun Animator Controller"));
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            mainAnimator.runtimeAnimatorController = (RuntimeAnimatorController)RuntimeAnimatorController.Instantiate(Resources.Load("Shot Gun Animator Controller"));
        }
    }
}
