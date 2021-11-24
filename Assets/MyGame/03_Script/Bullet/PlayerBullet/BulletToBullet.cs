using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletToBullet : MonoBehaviour
{
    [SerializeField] GameObject bullet = default;

    [SerializeField, Tooltip("発射間隔")] float firingInterval = 0.3f;
    [SerializeField, Tooltip("弾速")] float speed = 50;
    [SerializeField, Tooltip("発射した弾の攻撃力")] float setPower = 0.2f;

    float firingIntervalCount;
    void Start()
    {
        firingIntervalCount = firingInterval;
    }

    // Update is called once per frame
    void Update()
    {
        firingIntervalCount -= Time.deltaTime;
        if (firingIntervalCount < 0)
        {
            Shot();
            firingIntervalCount = firingInterval;
            AudioManager.Instance.PlaySE(AUDIO.SE_FIREARM_RTS_MACHINE_GUN_MODEL_01_FIRE_SINGLE_RR2_MONO, gameObject);
        }
    }

    void Shot()
    {
        int layerMaskInt = 1 << 0 | 1 << 18 | 1 << 20 | 1 << 21;
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2);

        Ray ray;
        ray = Camera.main.ScreenPointToRay(center);
        //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 4, false);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000.0f, layerMaskInt))
        {
        }
        Vector3 shotDirection = hit.point - transform.position;

        Quaternion rotatin = Quaternion.LookRotation(shotDirection);

        BulletPoolInstantate.Instance.InstBulletRigidbodyMove(bullet, transform.position, rotatin, shotDirection, speed, setPower);
    }
}
