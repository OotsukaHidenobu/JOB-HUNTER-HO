using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class MeteorGun : FullAutoGun
{
    [SerializeField, Tooltip("弾のぶれる縦の角度")] float diffusionAngleVertical = 15;
    [SerializeField, Tooltip("弾のぶれる横の角度")] float diffusionAngleHorizontal = 15;
    [SerializeField, Tooltip("変化のカーブ")] AnimationCurve curve = default;
    [SerializeField, Tooltip("何秒で最大になるか")] float accelerationDuration = 5;

    protected override void Shot()
    {

        int layerMaskInt = layerInt;

        //弾のブレ幅
        float rand0 = Random.Range(-1.0f, 1.0f);
        float rand1 = Random.Range(-1.0f, 1.0f);
        float r = Random.Range(-diffusionR, diffusionR);

        Vector3 diffusion = new Vector3(rand0, rand1, 0).normalized * r;
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(center + diffusion);

        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 4, false);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000.0f, layerMaskInt))
        {
            //Vector3 shotForward = Vector3.Scale((hit.point - transform.position), new Vector3(1, 1, 1)).normalized;
            //// hit.point が正面方向へRayをとばした際の接触座標.
            //bullets.GetComponent<Rigidbody>().velocity = shotForward * speed;
        }

        float rand2 = Random.Range(-diffusionAngleVertical, diffusionAngleVertical);
        float rand3 = Random.Range(-diffusionAngleHorizontal, diffusionAngleHorizontal);
        float rand4 = Random.Range(-diffusionAngleVertical, diffusionAngleVertical);
        Vector3 launchPoint = muzzle.position + new Vector3(rand2, rand3, rand4);
        Vector3 endPosition = hit.point + new Vector3(rand2, rand3, rand4);
        Vector3 shotDirection = endPosition - launchPoint;
        Quaternion rotatin = Quaternion.LookRotation(shotDirection);

        BulletPoolInstantate.Instance.InstBulletAcceleration(bullet, launchPoint, rotatin, speed, accelerationDuration, curve, power);

    }
}
