using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EveryDirectionBullet : MonoBehaviour
{
    BulletPoolInstantate bulletPoolInstantate;
    [SerializeField, Tooltip("弾のスピード")] float BulletSpeed = 10.0f;
    [SerializeField, Tooltip("EnemyReflectionBullet")] GameObject BulletPrefab = default;

    [SerializeField, Tooltip("")] int HorizontalBulletCount = 12;
    [SerializeField] float MuzzleOffset = 0.5f;

    [SerializeField] float destroyTime = 3;

    float destroyTimeCount;

    private void Start()
    {
        destroyTimeCount = destroyTime;
        bulletPoolInstantate = GameObject.Find("BulletPoolInstantate").GetComponent<BulletPoolInstantate>();
    }

    private void Shot()
    {
        var angleStep = 360.0f / this.HorizontalBulletCount;
        this.ShotHorizontal(0.0f, this.HorizontalBulletCount); // 赤道方向に弾を発射
        for (var angle = angleStep; angle < 90.0f; angle += angleStep)
        {
            this.ShotHorizontal(angle, this.HorizontalBulletCount); // 北緯angle°に弾を発射
            this.ShotHorizontal(-angle, this.HorizontalBulletCount); // 南緯angle°に弾を発射
        }
    }

    private void ShotHorizontal(float angleV, int maxCount)
    {
        angleV *= Mathf.Deg2Rad; // 緯度を弧度法による角度に変換
        var cosAngleV = Mathf.Cos(angleV);
        var sinAngleV = Mathf.Sin(angleV);
        var count = (int)(cosAngleV * maxCount); // 緯度に合わせて弾数を減らす...緯線の長さは緯度のコサインに比例する
        for (var i = 0; i < count; i++)
        {
            var angleH = (2.0f * Mathf.PI * i) / count; // 弧度法による経度
            var cosAngleH = Mathf.Cos(angleH);
            var sinAngleH = Mathf.Sin(angleH);

            // 緯度、経度からVector3による方角を算出
            var x = sinAngleH * cosAngleV;
            var y = sinAngleV;
            var z = cosAngleH * cosAngleV;
            var direction = new Vector3(x, y, z); // この方角に弾を1発発射したい

            bulletPoolInstantate.InstBulletRigidbodyMove(
                this.BulletPrefab,
                this.transform.position + (direction * this.MuzzleOffset),
                Quaternion.LookRotation(direction),
                direction,
                this.BulletSpeed,
                1);
            // var bullet = Instantiate(
            //     this.BulletPrefab,
            //     this.transform.position + (direction * this.MuzzleOffset),
            //     Quaternion.LookRotation(direction));
            // var rigidbody = bullet.GetComponent<Rigidbody>();
            // //rigidbody.useGravity = false;
            // //rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            // rigidbody.velocity = direction * this.BulletSpeed;
            // //rigidbody.AddForce(direction * this.BulletSpeed, ForceMode.VelocityChange);
        }
    }

    private void Update()
    {
        destroyTimeCount -= Time.deltaTime;

        if (destroyTimeCount <= 0)
        {
            Shot();
            AudioManager.Instance.PlaySE(AUDIO.SE_EXPLOSION_DISTORTED_01_MEDIUM_STEREO, gameObject, 0.3f, 0, 100, false, false);
            Destroy(this.gameObject);
        }

    }
}
