using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : SemiAutoGun
{
    [SerializeField, Tooltip("拡散数")] int diffusionBullets = 10;
    protected override void PlayShotSound()
    {
        AudioManager.Instance.PlaySE(AUDIO.SE_HANDLING_B_FS92_9MM_COCKING_MOTION_CHAMBER_ROUND_SAFETY_ON_FULL_MAGAZINE_MONO, gameObject, 0.3f, 0.1f);
        AudioManager.Instance.PlaySE(AUDIO.SE_FIREARM_SHOTGUN, gameObject);
    }
    protected override void Shot()
    {

        int layerMaskInt = layerInt;

        for (int i = 0; i < diffusionBullets; i++)
        {
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
            Quaternion rotatin = Quaternion.LookRotation(shotDirection);

            BulletPoolInstantate.Instance.InstBulletRigidbodyMove(bullet, muzzle.position, rotatin, shotDirection, speed, power);
        }
    }
}
