using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmachineGun : FullAutoGun
{
    protected override void Shot()
    {
        //animator.SetBool("shot", true);
        Vector3 force;

        int layerMaskInt = layerInt;

        //弾のブレ幅
        float rand0 = Random.Range(-1.0f, 1.0f);
        float rand1 = Random.Range(-1.0f, 1.0f);
        float r = Random.Range(-diffusionR, diffusionR);

        Vector3 diffusion = new Vector3(rand0, rand1, 0).normalized * r;

        force = this.gameObject.transform.forward * speed;
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
        Vector3 shotDirection = hit.point - muzzle.position;
        Quaternion rotatin = Quaternion.LookRotation(shotDirection);
        BulletPoolInstantate.Instance.InstBulletRigidbodyMove(bullet, muzzle.position, rotatin, shotDirection, speed, power);

    }
}
