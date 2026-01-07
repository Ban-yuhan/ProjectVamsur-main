using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 웨이브 전개를 총괄하는 디렉터.
/// - 시간 기반 카운트다운 + 킬 조건 보조로 다음 웨이브를 시작한다.
/// - 진행 시간에 따라 스폰량/엘리트 확률/적 배수 등을 스케일링한다.
/// - UI(HUD/배너/로그)와 신호를 주고 받는다.
/// </summary>
public class WaveDirector : MonoBehaviour
{
    [Header("Spawn Config")]
    [SerializeField]
    private List<SpawnPoint> spawnPoints = new List<SpawnPoint>(); // 스폰 포인트들

    [SerializeField]
    private List<GameObject> enemyPrefabs = new List<GameObject>(); // 스폰할 적 종류

    [SerializeField]
    private float minPlayerDistance = 3.0f; // 플레이어와 최소 이격

    [Header("Wave Timing")]
    [SerializeField]
    private float timeBetweenWaves = 15.0f; // 웨이브 간 시간(초)

    [SerializeField]
    private int killsRequired = 10;         // 보조 킬 조건(선택적)

    [SerializeField]
    private int initialSpawnCount = 6;      // 첫 웨이브 스폰 수

    [SerializeField]
    private float spawnBurstInterval = 0.2f;// 같은 웨이브 내 연속 스폰 간격

    [Header("Scaling")]
    [SerializeField]
    private float spawnCountPerMinute = 3.0f;    // 분당 스폰 증가량

    [SerializeField]
    private float eliteChancePerMinute = 0.05f;  // 분당 엘리트 확률 증가

    [SerializeField]
    private float enemyHealthMultPerMinute = 0.10f; // 분당 체력 배수 증가

    [SerializeField]
    private float eliteChanceMax = 0.5f;         // 엘리트 확률 상한

    [Header("UI Hooks")]
    [SerializeField]
    private WaveHUD waveHud;                // 남은 시간/웨이브 표시

    [SerializeField]
    private BannerAnnouncer banner;         // 경고 배너

    [SerializeField]
    private EventLogUI eventLog;            // 이벤트 로그

    [SerializeField]
    private AliveRegistry aliveRegistry;

    [SerializeField]
    private SafeSpawnRingProvider safeSpawnRingProvider;

    [SerializeField]
    private int[] enemyPrefabWeight;

    private GameManager gameManager;
    private int currentWave;
    private float nextWaveTimer;            // 다음 웨이브까지 남은 시간
    private int sessionKills;               // 세션 누적 킬
    private float sessionTime;              // 세션 경과 시간(초)

    private bool isSpawning;                // 현재 웨이브 스폰 중 여부

    /// <summary>
    /// 초기화: 참조 획득 및 타이머 설정.
    /// </summary>
    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        currentWave = 0;
        sessionKills = 0;
        sessionTime = 0.0f;
        nextWaveTimer = timeBetweenWaves;
        isSpawning = false;

        if (waveHud != null)
        {
            waveHud.SetWave(currentWave);
            waveHud.SetTimer(nextWaveTimer);
        }
    }

    /// <summary>
    /// 프레임 업데이트: Playing 상태에서만 진행도 업데이트 및 웨이브 타이머 감소.
    /// </summary>
    private void Update()
    {
        if (gameManager != null)
        {
            if (gameManager.IsPlaying() == false)
            {
                return;
            }
        }

        float dt = Time.deltaTime;
        sessionTime += dt;

        if (isSpawning == false)
        {
            nextWaveTimer -= dt;

            if (waveHud != null)
            {
                waveHud.SetTimer(Mathf.Max(0.0f, nextWaveTimer));
            }

            if (nextWaveTimer <= 0.0f)
            {
                StartCoroutine(StartNextWave());
            }
            else
            {
                if (killsRequired > 0)
                {
                    // 보조 킬 조건 충족 시 웨이브 앞당김(선택 룰)
                    if (sessionKills >= (currentWave * killsRequired))
                    {
                        StartCoroutine(StartNextWave());
                    }
                }
            }
        }
    }

    /// <summary>
    /// 외부(EnemyDeath 등)에서 호출: 킬 카운트 증가.
    /// </summary>
    public void ReportKill()
    {
        ++sessionKills;
        if (eventLog != null)
        {
            eventLog.Append("Enemy defeated. Total: " + sessionKills);
        }
    }

    /// <summary>
    /// 다음 웨이브를 시작한다. UI 갱신과 스폰 실행을 포함한다.
    /// </summary>
    /// <returns>코루틴 핸들</returns>
    /// <remarks>
    /// 적 스폰, UI 배너/로그 출력.
    /// 스폰 포인트나 적 목록이 비었으면 조용히 중단.
    /// 스폰은 Instantiate로 비용이 크다. burst 간격으로 분산.
    /// </remarks>
    private IEnumerator StartNextWave()
    {
        if (isSpawning == true)
        {
            yield break;
        }

        isSpawning = true;
        currentWave = currentWave + 1;

        if (waveHud != null)
        {
            waveHud.SetWave(currentWave);
        }

        if (banner != null)
        {
            banner.Show("Wave " + currentWave + " Begins!", 1.5f);
        }

        if (eventLog != null)
        {
            eventLog.Append("Wave " + currentWave + " started.");
        }

        // 진행도 기반 스케일 계산
        float minutes = sessionTime / 60.0f;
        int spawnCount = initialSpawnCount + Mathf.RoundToInt(minutes * spawnCountPerMinute);
        float eliteChance = Mathf.Clamp01(minutes * eliteChancePerMinute);
        if (eliteChance > eliteChanceMax)
        {
            eliteChance = eliteChanceMax;
        }
        float healthMult = 1.0f + minutes * enemyHealthMultPerMinute;

        // 스폰 실행
        for (int i = 0; i < spawnCount; i = i + 1)
        {
            //SpawnPoint sp = PickSpawnPoint();
            Vector3 pos = Vector3.zero;
            bool didSpawn = safeSpawnRingProvider.TrySample(out pos);
            if(didSpawn == false)
            {
                continue;
            }

            GameObject prefab = PickEnemyPrefab();

            //if (sp != null && prefab != null)
            if(prefab != null)
            {
                //GameObject mob = sp.Spawn(prefab, minPlayerDistance);
                GameObject mob = Instantiate(prefab, pos, Quaternion.identity);

                if (mob != null)
                {
                    // 체력 배수 반영(옵션): EnemyHealth가 있으면 최대 체력 증가
                    EnemyHealth hp = mob.GetComponent<EnemyHealth>();
                    if (hp != null)
                    {
                        float max = hp.GetMaxHealth();
                        if (max > 0.0f)
                        {
                            hp.SetMaxHealth(max * healthMult);
                        }
                    }

                    // 엘리트 부여
                    EliteRoller.TryApplyElite(mob, eliteChance, 2.0f, 1.5f);

                    if(aliveRegistry != null)
                    {
                        aliveRegistry.Register(mob);
                    }
                }
            }

            // burst 간격
            float t = spawnBurstInterval;
            while (t > 0.0f)
            {
                if (gameManager != null)
                {
                    if (gameManager.IsPlaying() == false)
                    {
                        yield return null;
                        continue;
                    }
                }
                t -= Time.deltaTime;
                yield return null;
            }
        }

        // 다음 웨이브 타이머 재설정
        nextWaveTimer = timeBetweenWaves;
        isSpawning = false;
        yield break;
    }

    /// <summary>
    /// 스폰 포인트에서 하나를 선택한다(랜덤)
    /// </summary>
    /// <returns>선택된 SpawnPoint 또는 null</returns>
    private SpawnPoint PickSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            return null;
        }

        int idx = Random.Range(0, spawnPoints.Count);
        return spawnPoints[idx];
    }

    /// <summary>
    /// 적 프리팹에서 하나를 선택한다(균등 랜덤).
    /// </summary>
    /// <returns>선택된 프리팹 또는 null</returns>
    private GameObject PickEnemyPrefab()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            return null;
        }

        if(enemyPrefabWeight != null && enemyPrefabWeight.Length > 0)
        {
            int sumWeight = 0;
            for (int i=0; i<enemyPrefabWeight.Length; ++i)
            {
                // 가중치의 합을 누적.
                sumWeight += enemyPrefabWeight[i];
            }

            // 1~가중치의 합 사이에서 값을 무작위로 선택.
            int weight = Random.Range(1, sumWeight+1);
            Debug.Log("weight = " + weight);

            int selectWeight = 0;
            for (int i = 0; i < enemyPrefabWeight.Length; ++i)
            {
                selectWeight += enemyPrefabWeight[i];

                // 무작위로 선택된 값이 각 가중치에 걸리는지 비교.
                if(weight <= selectWeight)
                {
                    Debug.Log("Select Index = " + i);
                    return enemyPrefabs[i];
                }
            }
        }

        int idx = Random.Range(0, enemyPrefabs.Count);
        return enemyPrefabs[idx];
    }
}
