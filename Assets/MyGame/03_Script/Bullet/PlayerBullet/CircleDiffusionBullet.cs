using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDiffusionBullet : MonoBehaviour
{
    [SerializeField] GameObject bullet = default;

    [SerializeField] float speed = 100;

    [SerializeField] float shotInterval = default;
    [SerializeField] float power = 1;

    GameObject bullets;

    float rand0 = default;
    float rand1 = default;
    float rand2 = default;

    Vector3 diffusion;

    RaycastHit hit;

    Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2);

    Ray ray;

    float shotTimer = default;

    bool isShot = false;

    void Start()
    {
        shotTimer = shotInterval;

    }

    // Update is called once per frame
    void Update()
    {
        shotTimer -= Time.deltaTime;
        if (shotTimer < 0)
        {
            isShot = true;
            shotTimer = shotInterval;

        }
        if (isShot)
        {
            CircleDiffusionBulletShot();
            AudioManager.Instance.PlaySE(AUDIO.SE_FIREARM_RTS_MACHINE_GUN_MODEL_01_FIRE_SINGLE_RR2_MONO, gameObject);
        }
    }

    void CircleDiffusionBulletShot()
    {

        isShot = false;

        int layerMask = ~(1 << 10);

        for (int i = 0; i < 3; i++)
        {
            //弾のブレ幅
            rand0 = Random.Range(-1.0f, 1.0f);
            rand1 = Random.Range(-1.0f, 0.0f);
            rand2 = Random.Range(-1.0f, 1.0f);

            diffusion = new Vector3(rand0, rand1, rand2).normalized * 30;

            ray = new Ray(transform.position, diffusion);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 4, false);

            if (Physics.Raycast(ray, out hit, 1000.0f, layerMask))
            {
            }
            Vector3 shotDirection = hit.point - transform.position;
            Quaternion rotatin = Quaternion.LookRotation(shotDirection);
            BulletPoolInstantate.Instance.InstBulletRigidbodyMove(bullet, transform.position, rotatin, shotDirection, speed, power);
        }


    }
}
