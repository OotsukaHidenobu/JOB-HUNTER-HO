using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveControlVarious : MonoBehaviour
{
    //スポナーのステート
    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    [System.Serializable]
    public class Wave
    {
        public string name;         //名前
        public Transform[] enemy;   //出したい敵の順番で入れる
        public float delay;         //何秒間隔で出現させるか
    }
    // Waveの設定を登録
    [SerializeField, Tooltip("ウェイブ数")] Wave[] waves = default;

    // 次のWave
    private int nextWave = 0;

    //何秒後に開始するか
    [SerializeField, Tooltip("何秒間隔でウェイブが開始するか")] float timeBetweenWaves = 5;
    private float waveCountdown;

    //ドロップシステム
    [SerializeField, Tooltip("DropSystemの参照")] GameObject dropSystem = default;
    [SerializeField, Tooltip("武器のドロップ確率")] int weaponPercent = 80;
    [SerializeField, Tooltip("回復アイテムのドロップ確率")] int healPercent = 20;
    [SerializeField, Tooltip("弾薬回復アイテムのドロップ確率")] int ammoPercent = 20;

    [SerializeField, Tooltip("消したいマップのプレハブ")] GameObject[] deleteMapPrefab = default;
    [SerializeField, Tooltip("表示したいマップのプレハブ")] GameObject[] activeMapPrefab = default;
    [SerializeField, Tooltip("MiniMapUI")] MiniMapUI miniMapUI = default;

    //敵のサーチ間隔の時間
    private float searchCountdown = 1;

    //初めのステート
    private SpawnState state = SpawnState.COUNTING;

    //スポーンエリアにプレイヤーが入ったかどうか
    private bool start = false;

    BoxCollider boxCol;


    private void Start()
    {
        //プレイヤーがエリアに入った瞬間スタート
        waveCountdown = 0;

        boxCol = gameObject.GetComponent<BoxCollider>();
    }

    private void Update()
    {
        //エリアに入らないと始まらないようにする
        if (start == false) return;
        //ステータスがWAITINGの時
        if (state == SpawnState.WAITING)
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
        if (waveCountdown <= 0)
        {
            if (state != SpawnState.SPAWNING)
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

        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        //最終Waveが終わったら
        if (nextWave + 1 > waves.Length - 1)
        {
            //アイテムドロップ
            dropSystem.GetComponent<DropSystem>().Drop(boxCol.bounds.center, weaponPercent, healPercent, ammoPercent);
            //マップの表示
            miniMapUI.CheckProperty = false;
            AudioManager.Instance.PlayBGM(AUDIO.BGM_TIMEBEND, 0.25f, 0.5f);
            //最終Waveの敵を全て倒すと終了
            Destroy(this.gameObject);

            GameObject.Find("FloorGrid").GetComponent<FloorAnimation>().SetMode(FloorAnimation.Mode.Safe);

            //nextWave = 0;
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
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0)
        {
            searchCountdown = 1;
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
        state = SpawnState.SPAWNING;

        for (int i = 0; i < _wave.enemy.Length; i++)
        {
            SpawnEnemy(_wave.enemy[i]);
            yield return new WaitForSeconds(_wave.delay);
        }

        state = SpawnState.WAITING;

        yield break;
    }

    //敵を出現させる処理
    void SpawnEnemy(Transform _enemy)
    {
        float x_size = boxCol.bounds.size.x / 2 + boxCol.bounds.center.x;
        float x_size_minus = -boxCol.bounds.size.x / 2 + boxCol.bounds.center.x;
        float z_size = boxCol.bounds.size.z / 2 + boxCol.bounds.center.z;
        float z_size_minus = -boxCol.bounds.size.z / 2 + boxCol.bounds.center.z;
        float offset = 3;
        float x = Random.Range(x_size_minus + offset, x_size - offset);
        float y = _enemy.transform.position.y / 2;
        float z = Random.Range(z_size_minus + offset, z_size - offset);

        Transform enemy = Instantiate(_enemy, new Vector3(x, y, z), Quaternion.identity);
        AudioManager.Instance.PlaySE(AUDIO.SE_CYBER10, enemy.gameObject);
    }

    //Playerが範囲内に入ったらWaveの処理を開始する
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AudioManager.Instance.PlayBGM(AUDIO.BGM_ZONE, 0.25f, 0.5f);
            start = true;
            transform.GetChild(0).gameObject.SetActive(true);

            miniMapUI.CheckProperty = true;

            //？マップの削除
            foreach (GameObject i in deleteMapPrefab)
            {
                Destroy(i);
            }
            List<GameObject> list = new List<GameObject>(activeMapPrefab);
            list.RemoveAll(item => item == null);
            activeMapPrefab = list.ToArray();
            //先のマップの表示
            foreach (GameObject i in activeMapPrefab)
            {
                i.SetActive(true);
            }

            GameObject.Find("FloorGrid").GetComponent<FloorAnimation>().SetMode(FloorAnimation.Mode.Danger);
        }
    }
}
