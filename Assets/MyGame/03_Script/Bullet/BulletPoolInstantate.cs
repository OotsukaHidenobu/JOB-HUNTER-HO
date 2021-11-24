using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//弾をプールにして生成するシングルトン処理
public class BulletPoolInstantate : SingletonMonoBehaviour<BulletPoolInstantate>
{
    TrailRenderer trailRenderer;

    Transform bullets;
    void Start()
    {
        //弾を保持する空のオブジェクトを生成
        bullets = new GameObject("PlayerBullets").transform;
        //trailRenderer = bullet.gameObject.transform.GetChild(1).GetComponent<TrailRenderer>();
    }
    /// <summary>
    /// 弾をプールして生成
    /// </summary>
    /// <param name="bullet">弾のプレハブ</param>
    /// <param name="pos">生成したい場所</param>
    /// <param name="rotation">向かせたい方向</param>
    /// <param name="speed">弾の速度</param>
    /// <param name="power">攻撃力</param>
    /// <returns></returns>
    public GameObject InstBullet(GameObject bullet, Vector3 pos, Quaternion rotation, float speed, float power)
    {
        //アクティブでないオブジェクトをbulletsの中から探索
        foreach (Transform t in bullets)
        {
            if (!t.gameObject.activeSelf && t.name == bullet.name)
            {
                //非アクティブなオブジェクトの位置と回転を設定
                t.SetPositionAndRotation(pos, rotation);
                //速度の設定
                t.gameObject.GetComponent<Bullet>().speed = speed;
                //攻撃力の設定
                OffensivePower[] powers1 = t.gameObject.GetComponentsInChildren<OffensivePower>();
                foreach (OffensivePower pow in powers1)
                {
                    pow.power = power;
                }
                //トレイルレンダラーの初期化
                t.gameObject.transform.GetChild(1).GetComponent<TrailRenderer>().Clear();
                //アクティブにする
                t.gameObject.SetActive(true);
                return t.GetComponent<GameObject>();
            }
        }
        //非アクティブなオブジェクトがない場合新規生成
        GameObject instBullet = Instantiate(bullet, pos, rotation, bullets);
        instBullet.GetComponent<Bullet>().speed = speed;
        //攻撃力の設定
        OffensivePower[] powers = instBullet.GetComponentsInChildren<OffensivePower>();
        foreach (OffensivePower pow in powers)
        {
            pow.power = power;
        }
        instBullet.name = bullet.name;
        return instBullet;
    }

    /// <summary>
    /// Rigidbodyのvelocityで移動をさせたい弾をプールして生成
    /// </summary>
    /// <param name="bullet">弾のプレハブ</param>
    /// <param name="pos">生成したい場所</param>
    /// <param name="rotation">向かせたい方向</param>
    /// <param name="direction">動かしたい方向ベクトル</param>
    /// <param name="speed">弾の速度</param>
    /// <param name="power">攻撃力</param>
    /// <returns></returns>
    public GameObject InstBulletRigidbodyMove(GameObject bullet, Vector3 pos, Quaternion rotation, Vector3 direction, float speed, float power)
    {
        //アクティブでないオブジェクトをbulletsの中から探索
        foreach (Transform t in bullets)
        {
            if (!t.gameObject.activeSelf && t.name == bullet.name)
            {
                //非アクティブなオブジェクトの位置と回転を設定
                t.SetPositionAndRotation(pos, rotation);
                //向きと速度の設定
                t.gameObject.GetComponent<Rigidbody>().velocity = direction.normalized * speed;
                //攻撃力の設定
                OffensivePower[] powers1 = t.gameObject.GetComponentsInChildren<OffensivePower>();
                foreach (OffensivePower pow in powers1)
                {
                    pow.power = power;
                }
                //トレイルレンダラーの初期化
                t.gameObject.transform.GetChild(1).GetComponent<TrailRenderer>().Clear();
                //アクティブにする
                t.gameObject.SetActive(true);
                return t.GetComponent<GameObject>();
            }
        }
        //非アクティブなオブジェクトがない場合新規生成
        GameObject instBullet = Instantiate(bullet, pos, rotation, bullets);
        instBullet.GetComponent<Rigidbody>().velocity = direction.normalized * speed;
        //攻撃力の設定
        OffensivePower[] powers = instBullet.GetComponentsInChildren<OffensivePower>();
        foreach (OffensivePower pow in powers)
        {
            pow.power = power;
        }
        //生成した弾の名前をプレハブと同じに(Cloneをつけなくする)
        instBullet.name = bullet.name;
        return instBullet;
    }

    /// <summary>
    /// DoTweenのDOMoveで移動させたい弾をプールして生成
    /// </summary>
    /// <param name="bullet">弾のプレハブ</param>
    /// <param name="pos">生成したい場所</param>
    /// <param name="rotation">向かせたい方向</param>
    /// <param name="endPos">向かわせたい場所</param>
    /// <param name="speed">弾の速度</param>
    /// <param name="ease">DoTweenアニメーションの種類</param>
    /// <returns></returns>
    public GameObject InstBulletDOMove(GameObject bullet, Vector3 pos, Quaternion rotation, Vector3 endPos, float speed, Ease ease)
    {
        //アクティブでないオブジェクトをbulletsの中から探索
        foreach (Transform t in bullets)
        {
            if (!t.gameObject.activeSelf && t.name == bullet.name)
            {
                //非アクティブなオブジェクトの位置と回転を設定
                t.SetPositionAndRotation(pos, rotation);
                //向きと速度の設定
                t.gameObject.transform.DOMove(endPos, speed).SetEase(ease).OnComplete(() => t.gameObject.SetActive(false));
                //トレイルレンダラーの初期化
                t.gameObject.transform.GetChild(1).GetComponent<TrailRenderer>().Clear();
                //アクティブにする
                t.gameObject.SetActive(true);
                return t.GetComponent<GameObject>();
            }
        }
        //非アクティブなオブジェクトがない場合新規生成
        GameObject instBullet = Instantiate(bullet, pos, rotation, bullets);
        instBullet.transform.DOMove(endPos, speed).SetEase(ease).OnComplete(() => instBullet.SetActive(false));
        instBullet.name = bullet.name;
        return instBullet;
    }

    /// <summary>
    /// 加速させたい弾をプールして生成
    /// </summary>
    /// <param name="bullet">弾のプレハブ</param>
    /// <param name="pos">生成したい場所</param>
    /// <param name="rotation">向かせたい方向</param>
    /// <param name="maxSpeed">弾の速度</param>
    /// <param name="accelerationDuration">何秒で最大速度か</param>
    /// <param name="curve">AnimationCurve</param>
    /// <param name="power">攻撃力</param>
    /// <returns></returns>
    public GameObject InstBulletAcceleration(GameObject bullet, Vector3 pos, Quaternion rotation, float maxSpeed, float accelerationDuration, AnimationCurve curve, float power)
    {
        //アクティブでないオブジェクトをbulletsの中から探索
        foreach (Transform t in bullets)
        {
            if (!t.gameObject.activeSelf && t.name == bullet.name)
            {
                //非アクティブなオブジェクトの位置と回転を設定
                t.SetPositionAndRotation(pos, rotation);
                //向きと速度の設定
                t.gameObject.GetComponent<AccelerationBullet>().maxSpeed = maxSpeed;
                t.gameObject.GetComponent<AccelerationBullet>().accelerationDuration = accelerationDuration;
                t.gameObject.GetComponent<AccelerationBullet>().curve = curve;
                //攻撃力の設定
                OffensivePower[] powers1 = t.gameObject.GetComponentsInChildren<OffensivePower>();
                foreach (OffensivePower pow in powers1)
                {
                    pow.power = power;
                }
                //トレイルレンダラーの初期化
                t.gameObject.transform.GetChild(1).GetComponent<TrailRenderer>().Clear();
                //アクティブにする
                t.gameObject.SetActive(true);
                return t.GetComponent<GameObject>();
            }
        }
        //非アクティブなオブジェクトがない場合新規生成
        GameObject instBullet = Instantiate(bullet, pos, rotation, bullets);
        instBullet.GetComponent<AccelerationBullet>().maxSpeed = maxSpeed;
        instBullet.GetComponent<AccelerationBullet>().accelerationDuration = accelerationDuration;
        instBullet.GetComponent<AccelerationBullet>().curve = curve;
        //攻撃力の設定
        OffensivePower[] powers = instBullet.GetComponentsInChildren<OffensivePower>();
        foreach (OffensivePower pow in powers)
        {
            pow.power = power;
        }
        instBullet.name = bullet.name;
        return instBullet;
    }

    /// <summary>
    /// パーティクルをプールして生成
    /// </summary>
    /// <param name="particle">パーティクルのプレハブ</param>
    /// <param name="pos">生成したい場所</param>
    /// <param name="rotation">向かせたい方向</param>
    /// <returns></returns>
    public GameObject InstParticle(GameObject particle, Vector3 pos, Quaternion rotation)
    {
        //アクティブでないオブジェクトをbulletsの中から探索
        foreach (Transform t in bullets)
        {
            if (!t.gameObject.activeSelf && t.name == particle.name)
            {
                //非アクティブなオブジェクトの位置と回転を設定
                t.SetPositionAndRotation(pos, rotation);
                //アクティブにする
                t.gameObject.SetActive(true);
                return t.GetComponent<GameObject>();
            }
        }
        //非アクティブなオブジェクトがない場合新規生成
        GameObject instBullet = Instantiate(particle, pos, rotation, bullets);
        instBullet.name = particle.name;
        return instBullet;
    }
}
