using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossSpawnControl : MonoBehaviour
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

    [SerializeField, Tooltip("出現させたい敵の場所")] Transform spawnPosition = default;

    // 次のWave
    private int nextWave = 0;

    //何秒後に開始するか
    [SerializeField, Tooltip("何秒間隔でウェイブが開始するか")] float timeBetweenWaves = 5;
    private float waveCountdown;

    [SerializeField, Tooltip("消したいマップのプレハブ")] GameObject[] deleteMapPrefab = default;
    [SerializeField, Tooltip("表示したいマップのプレハブ")] GameObject[] activeMapPrefab = default;
    [SerializeField, Tooltip("MiniMapUI")] MiniMapUI miniMapUI = default;
    [SerializeField] FadeScene fadeScene = default;
    [SerializeField, Tooltip("プレイヤー")] GameObject player = default;
    PlayerStatus playerStatus;
    Animator animator;
    [SerializeField, Tooltip("ボス破壊後のバーチャルカメラ")] GameObject vcamDestoryAfter = default;
    //敵のサーチ間隔の時間
    private float searchCountdown = 1;

    //初めのステート
    private SpawnState state = SpawnState.COUNTING;

    //スポーンエリアにプレイヤーが入ったかどうか
    private bool start = false;

    BoxCollider boxCol;

    HPBar hpBar;

    BossState bossState;

    bool onShot = true;

    bool onShot2 = true;

    bool clearUI = false;
    float clearTimeCount = 0;

    //ゲームクリア時のUI
    GameObject gameClearUI;

    public bool GameClear { get; set; } = false;

    private void Awake()
    {
        hpBar = GameObject.Find("BossHP").GetComponent<HPBar>();
        hpBar.gameObject.SetActive(false);

        playerStatus = player.GetComponent<PlayerStatus>();
        animator = player.GetComponent<Animator>();

        gameClearUI = GameObject.Find("ClearCanvas");
    }
    private void Start()
    {
        //プレイヤーがエリアに入った瞬間スタート
        waveCountdown = 0;

        boxCol = gameObject.GetComponent<BoxCollider>();

    }

    private void Update()
    {
        //エリアに入らないと始まらないようにする
        if (start == false || playerStatus.PlayerDead) return;
        //ボスが存在しているとき
        if (bossState)
        {
            GameClear = bossState.clear;
            //ボスが破壊されたら
            if (bossState.bossDestory && onShot2)
            {
                StartCoroutine(BossDestory());
                Transform bullets = GameObject.Find("PlayerBullets").transform;
                foreach (Transform child in bullets)
                {
                    child.gameObject.SetActive(false);
                }
                onShot2 = false;
            }
        }
        if (GameClear && onShot)
        {
            GameObject.Find("UI").SetActive(false);
            GameObject.Find("SightCanvas").SetActive(false);
            onShot = false;
        }

        if (clearUI)
        {
            Image clearImage = gameClearUI.transform.GetChild(0).gameObject.GetComponent<Image>();

            clearTimeCount += Time.deltaTime;
            float alfa = Mathf.Lerp(0f, 1f, clearTimeCount / 0.5f);
            clearImage.color = new Color(clearImage.color.r, clearImage.color.g, clearImage.color.b, alfa);
        }

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
    IEnumerator BossDestory()
    {
        yield return new WaitForSeconds(1.0f);
        vcamDestoryAfter.SetActive(true);
        AudioManager.Instance.FadeOutBGM(2);
        GameObject.Find("Weapon").SetActive(false);
        yield return new WaitForSeconds(0.7f);
        AudioManager.Instance.PlaySE(AUDIO.SE_GAME_MAOUDAMASHII_9_JINGLE02, 0.2f);
        yield return new WaitForSeconds(0.2f);
        AudioManager.Instance.PlayVoice(AUDIO.Voice_V0027, Camera.main.gameObject, 0.6f);
        animator.SetBool("Win", true);


        yield return new WaitForSeconds(3);
        AudioManager.Instance.PlayBGM(AUDIO.BGM_TALK_AND_WALK);
        clearUI = true;
        gameClearUI.transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(4);
        FadeScene instFade = Instantiate(fadeScene);
    }



    //敵が全て倒された時の処理
    void WaveCompleted()
    {

        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        //最終Waveが終わったら
        if (nextWave + 1 > waves.Length - 1)
        {
            //ゲームクリア
            //GameClear = true;

            //StartCoroutine(ChangeScene(3));

            //Destroy(this.gameObject);

            GameObject.Find("FloorGrid").GetComponent<FloorAnimation>().SetMode(FloorAnimation.Mode.Safe);

            //マップの表示
            miniMapUI.CheckProperty = false;

            return;
        }
        //次のWaveがある時
        else
        {
            nextWave++;
        }


    }
    //シーン切り替え
    IEnumerator ChangeScene(float time)
    {
        AudioManager.Instance.PlaySE(AUDIO.SE_SOUNDLOGO);
        yield return new WaitForSeconds(time);
        AudioManager.Instance.FadeOutBGM(3);
        FadeScene instFade = Instantiate(fadeScene);


        //最終Waveの敵を全て倒すと終了
        Destroy(this.gameObject);
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
        //print("Spawn Wave:" + _wave.name);
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

        Instantiate(_enemy, spawnPosition.position, Quaternion.identity);

        bossState = GameObject.Find("Boss(Clone)").GetComponent<BossState>();

    }

    //Playerが範囲内に入ったらWaveの処理を開始する
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AudioManager.Instance.PlayBGM(AUDIO.BGM_BOSS, 0.25f, 0.5f);
            start = true;
            transform.GetChild(0).gameObject.SetActive(true);

            miniMapUI.CheckProperty = true;
            hpBar.gameObject.SetActive(true);

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
