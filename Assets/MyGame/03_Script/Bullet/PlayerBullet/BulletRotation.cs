using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//任意軸を中心に回転
public class BulletRotation : MonoBehaviour
{
    [SerializeField] float speed = default;

    enum RotatePoint
    {
        x,
        y,
        z
    }
    [SerializeField] RotatePoint rotatePoint = default;

    void Update()
    {
        if (Time.timeScale == 0) return;
        if (rotatePoint == RotatePoint.x)
        {
            transform.Rotate(new Vector3(speed, 0, 0));
        }
        else if (rotatePoint == RotatePoint.y)
        {
            transform.Rotate(new Vector3(0, speed, 0));
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, speed));
        }

    }
}
