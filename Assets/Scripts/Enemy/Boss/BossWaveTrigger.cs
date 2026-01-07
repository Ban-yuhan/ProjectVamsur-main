using UnityEngine;

/// <summary>
/// 정해진 시각에 보스 몬스터를 스폰한다.
/// 해당 보스가 사망하면 게임 클리어 상태로 전환한다.(선택)
/// 스폰 위치는 SafeSpawnRingProvider 기능을 이용해서 지정.
/// 위의 경우가 아닐 경우 그냥 임의의 위치에 생성.
/// </summary>
public class BossWaveTrigger : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;    // 게임 상태를 전환하기 위한 참조용 변수.

    [SerializeField]
    private Transform player;   // 플레이어의 트램스폼 참조용 변수.

    [SerializeField]
    private GameObject bossPrefab;  // 보스 몬스터 프리팹.

    [SerializeField]
    private SafeSpawnRingProvider safeProvider; // 화면 영역 바깥에 소환 위치를 지정하기 위한 참조용 변수.

    [SerializeField]
    private float spawnTimeSeconds = 120.0f;    // 플레이 시작 후 보스를 소환할 시간.

    [SerializeField]
    private Vector2 fallbackOffset = new Vector2(13.0f, 0.0f);  // 보스를 소환할 임시 위치.

    [SerializeField]
    private BossWarning bossWarning;

    private bool spawned = false;   // 중복 소환을 막기 위한 변수.
    private Transform bossInstance; // 소환된 보스의 트램스폼 정보를 담을 변수.

    private void OnEnable()
    {
        spawned = false;
        bossInstance = null;
        EventBus.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        EventBus.OnDeath -= OnDeath;
    }

    // Update is called once per frame
    void Update()
    {
        if(spawned == true)
        {
            return;
        }

        if(gameManager == null)
        {
            return;
        }

        if(player == null)
        {
            return;
        }

        if(gameManager.currentState != GameState.Playing)
        {
            return;
        }

        // 게임 플레이 경과 시간을 가져온다.
        float t = gameManager.GetElapsedPlayTime();
        if(t >= spawnTimeSeconds)
        {
            TrySpawnBossOnce();
        }
    }

    /// <summary>
    /// 사망 이벤트를 받아, 사망한 대상이 보스일 경우 게임 클리어 상태로 전환 처리.
    /// </summary>
    /// <param name="deathEvent">사망 이벤트 정보를 담고 있는 구조체</param>
    void OnDeath(DeathEvent deathEvent)
    {
        if(gameManager == null)
        {
            return;
        }

        if(bossInstance == null)
        {
            return;
        }

        if(deathEvent.victim == bossInstance)
        {
            if(gameManager.currentState == GameState.Playing)
            {
                gameManager.HandleGameClear();
            }
        }
    }

    /// <summary>
    /// 보스 몬스터를 소환할 위치를 뽑는다.
    /// 기본적으로 화면 바깥 영역에서 좌표를 뽑는 처리를 하고,
    /// 그렇지 못할 경우 플레이어의 위치에서 offset 만큼 떨어진 위치를 반환한다.
    /// </summary>
    /// <returns>스폰할 위치</returns>
    Vector3 GetSpawnPosition()
    {
        if(safeProvider != null)
        {
            Vector3 p;
            bool ok = safeProvider.TrySample(out p);
            if(ok == true)
            {
                return p;
            }
        }

        Vector3 basePos = player.position;
        Vector3 offset = new Vector3(fallbackOffset.x, fallbackOffset.y, 0.0f);
        return basePos + offset;
    }

    /// <summary>
    /// 보스를 한 번만 소환하기 위한 함수.
    /// </summary>
    void TrySpawnBossOnce()
    {
        if(spawned == true)
        {
            return;
        }

        if(bossPrefab == null)
        {
            return;
        }

        Vector3 pos = GetSpawnPosition();
        GameObject go = Instantiate(bossPrefab, pos, Quaternion.identity);
        if(go != null)
        {
            bossInstance = go.transform;
            spawned = true;

            if(bossWarning != null)
            {
                bossWarning.StartWarning();
            }
        }
    }
}
