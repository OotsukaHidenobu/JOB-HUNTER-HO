using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveControl : MonoBehaviour
{
    //スポナーのステート
    public enum SpawnState { SPAWNING,WAITING,COUNTING};

    [System.Serializable]
    public class Wave
    {
        public string name;         //名前
        public Transform enemy;   //出したい敵
        public int count;           //何体出現させるか
        public float delay;         //何秒間隔で出現させるか
    }
    // Waveの設定を登録
    public Wave[] waves;

    // 次のWave
    private int nextWave = 0;

    //何秒後に開始するか
    [SerializeField] float timeBetweenWaves = 5;
    private float waveCountdown;

    //敵のサーチ間隔の時間
    private float serchCountdown = 1;

    //初めのステート
    private SpawnState state = SpawnState.COUNTING;

    //スポーンエリアにプレイヤーが入ったかどうか
    private bool stert = false;

    private void Start()
    {
        waveCountdown = timeBetweenWaves;
    }

    private void Update()
    {
        //エリアに入らないと始まらないようにする
        if (stert == false) return;
        //ステータスがWAITINGの時
        if(state == SpawnState.WAITING)
        {
            //敵が全て倒されたらWaveCompletedを呼ぶ
            if (!EnemyIsAlive())
            {
                WaveCompleted();
            }
            else
            {
                return;
            }
        }

        //カウントが0になると敵を出現させる
        if(waveCountdown <= 0)
        {
            if(state != SpawnState.SPAWNING)
            {
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
        }
    }

    //敵が全て倒された時の処理
    void WaveCompleted()
    {
        print("Wave Comp!");

        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        //最終Waveが終わったら
        if(nextWave + 1 > waves.Length - 1)
        {
            //最終Waveの敵を全て倒すと終了
            Destroy(this.gameObject);
            //nextWave = 0;
            //print("All Waves Comp! Looping...");
        }
        //次のWaveがある時
        else
        {
            nextWave++;
        }

        
    }

    //敵がいるかどうかを判定する
    bool EnemyIsAlive()
    {
        serchCountdown -= Time.deltaTime;
        if(serchCountdown <= 0)
        {
            serchCountdown = 1;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }
        return true;
    }

    //1Waveの処理
    IEnumerator SpawnWave(Wave _wave)
    {
        print("Spawn Wave:" + _wave.name);
        state = SpawnState.SPAWNING;

        for (int i = 0; i < _wave.count; i++)
        {
            SpawnEnemy(_wave.enemy);
            yield return new WaitForSeconds(_wave.delay);
        }

        state = SpawnState.WAITING;

        yield break;
    }

    //敵を出現させる処理
    void SpawnEnemy(Transform _enemy)
    {
        print("Spawning Enemy" + _enemy.name);

        float x_size = transform.lossyScale.x / 2 + transform.position.x;
        float x_size_minus = -transform.lossyScale.x / 2 + transform.position.x;
        float z_size = transform.lossyScale.z / 2 + transform.position.z;
        float z_size__minus = -transform.lossyScale.z / 2 + transform.position.z;
        float x = Random.Range(x_size_minus, x_size);
        float y = 1;
        float z = Random.Range(z_size__minus, z_size);
        
        Instantiate(_enemy, new Vector3(x, y, z), Quaternion.identity);
    }

    //Playerが範囲内に入ったらWaveの処理を開始する
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            stert = true;
        }
    }

}
