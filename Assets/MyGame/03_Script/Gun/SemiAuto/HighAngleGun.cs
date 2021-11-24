using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighAngleGun : SemiAutoGun
{
    [SerializeField, Tooltip("拡散角度")] float diffusionAngle = 45;
    [SerializeField, Tooltip("拡散弾数")] int diffusionBullet = 16;
    protected override void PlayShotSound()
    {
        AudioManager.Instance.PlaySE(AUDIO.SE_HANDLING_B_FS92_9MM_COCKING_MOTION_CHAMBER_ROUND_SAFETY_ON_FULL_MAGAZINE_MONO, gameObject, 0.3f, 0.1f);
        AudioManager.Instance.PlaySE(AUDIO.SE_FIREARM_SHOTGUN, gameObject);
    }
    protected override void Shot()
    {

        int layerMaskInt = layerInt;

        //弾のブレ幅
        float rand0 = Random.Range(-1.0f, 1.0f);
        float rand1 = Random.Range(-1.0f, 1.0f);
        float r = Random.Range(-diffusionR, diffusionR);

        Vector3 diffusion = new Vector3(rand0, rand1, 0).normalized * r;

        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2);

        Ray ray;
        ray = Camera.main.ScreenPointToRay(center + diffusion);
        //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 4, false);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000.0f, layerMaskInt))
        {
        }
        Vector3 shotDirection = hit.point - muzzle.position;


        Quaternion rotation = Quaternion.LookRotation(shotDirection);

        BulletPoolInstantate.Instance.InstBulletRigidbodyMove(bullet, muzzle.position, rotation, shotDirection, speed, power);
    }

    void WayShot()
    {
        float m_OneShotAngle = 0;
        if (diffusionBullet - 1 > 0)
        {
            m_OneShotAngle = diffusionAngle / (diffusionBullet - 1);
        }

        int layerMaskInt = layerInt;

        //弾のブレ幅
        float rand0 = Random.Range(-1.0f, 1.0f);
        float rand1 = Random.Range(-1.0f, 1.0f);
        float r = Random.Range(-diffusionR, diffusionR);

        Vector3 diffusion = new Vector3(rand0, rand1, 0).normalized * r;

        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2);
        Ray ray;
        ray = Camera.main.ScreenPointToRay(center + diffusion);
        //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 4, false);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000.0f, layerMaskInt))
        {
        }
        Vector3 shotDirection = hit.point - muzzle.position;
        Quaternion rotation = Quaternion.LookRotation(shotDirection);
        rotation.x = 0;
        rotation.z = 0;
        for (int i = 0; i < diffusionBullet; i++)
        {
            //Quaternion rotate = Quaternion.Euler(0, -diffusionAngle / 2, 0) * rotation * Quaternion.Euler(0, m_OneShotAngle * i, 0);
            Quaternion rotate = Quaternion.Euler(-diffusionAngle / 2, 0, 0) * rotation * Quaternion.Euler(m_OneShotAngle * i, 0, 0);

            BulletPoolInstantate.Instance.InstBullet(bullet, muzzle.position, rotate, speed, power);
        }
    }
}
