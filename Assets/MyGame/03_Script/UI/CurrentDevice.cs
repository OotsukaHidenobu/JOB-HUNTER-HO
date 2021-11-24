using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//最新の押されたボタンがキーボードかパッドかを調べる
public class CurrentDevice : MonoBehaviour
{
    public static bool anyKey = false;

    bool oneShot1 = true;
    bool oneShot2 = true;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        if (Input.anyKeyDown && oneShot1)
        {
            anyKey = true;
            oneShot1 = false;
            oneShot2 = true;
        }
        if ((Input.GetKeyDown("joystick button 0") || Input.GetKeyDown("joystick button 1") || Input.GetKeyDown("joystick button 2") ||
        Input.GetKeyDown("joystick button 3") || Input.GetKeyDown("joystick button 4") || Input.GetKeyDown("joystick button 5") ||
        Input.GetKeyDown("joystick button 6") || Input.GetKeyDown("joystick button 7") || Input.GetKeyDown("joystick button 8") ||
        Input.GetKeyDown("joystick button 9")) && oneShot2)
        {
            anyKey = false;
            oneShot1 = true;
            oneShot2 = false;
        }
        float hori = Mathf.Abs(Input.GetAxis("PadHorizontal"));
        float vert = Mathf.Abs(Input.GetAxis("PadVertical"));
        float x_arrow = Mathf.Abs(Input.GetAxis("PadHorizontalArrow"));
        float y_arrow = Mathf.Abs(Input.GetAxis("PadVerticalArrow"));
        float r_StickX = Mathf.Abs(Input.GetAxis("R_StickX"));
        float r_StickY = Mathf.Abs(Input.GetAxis("R_StickY"));
        if ((hori > 0 || vert > 0 || x_arrow > 0 || y_arrow > 0 || r_StickX > 0 || r_StickY > 0) && oneShot2)
        {
            anyKey = false;
            oneShot1 = true;
            oneShot2 = false;
        }
    }
}
