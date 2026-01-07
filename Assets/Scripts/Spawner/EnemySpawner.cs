using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private SpawnDifficultyScaler scaler;

    [SerializeField]
    private List<GameObject> enemyPrefabs = new List<GameObject>();

    private float spawnBudget;  // 누적 스폰 포인트.(초당 스폰 양을 시간에 곱해 축적)
    private int aliveEnemies;

    private void Awake()
    {
        if(gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        if(playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if(playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }

        if(scaler == null)
        {
            scaler = GetComponent<SpawnDifficultyScaler>();
        }

        spawnBudget = 0.0f;
        aliveEnemies = 0;
    }

    // Update is called once per frame
    void Update()
    {
        bool canSpawn = false;
        if(gameManager != null)
        {
            canSpawn = gameManager.IsPlaying();
        }

        if(canSpawn == false)
        {
            return;
        }

        if(playerTransform == null || scaler == null)
        {
            return;
        }

        float baseRate = scaler.GetBaseSpawnRatePerSecond();
        float growthPerMin = scaler.GetSpawnRateGrowthPerMinute();

        float elapsed = gameManager.GetElapsedPlayTime();
        float elapsedMinutes = elapsed / 60.0f;

        float effectiveRate = baseRate + growthPerMin * elapsedMinutes; // 초당 스폰 양.

        float add = effectiveRate * Time.deltaTime;
        spawnBudget += add;

        int cap = scaler.GetMaxAliveEnemies();
        if(aliveEnemies >= cap)
        {
            return;
        }

        while(spawnBudget >= 1.0f && aliveEnemies < cap)
        {
            bool success = SpawnOneEnemy();
            if(success == true)
            {
                --spawnBudget;
                ++aliveEnemies;
            }
            else
            {
                break;
            }
        }
    }

    bool SpawnOneEnemy()
    {
        if(enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            return false;
        }

        int index = Random.Range(0, enemyPrefabs.Count);
        GameObject prefab = enemyPrefabs[index];
        if(prefab == null)
        {
            return false;
        }

        Vector3 center = playerTransform.position;
        Vector2 spawnPos = GetRandomPointOnRing(center, scaler.GetMinSpawnRadius(), scaler.GetMaxSpawnRadius());

        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        if(enemy == null)
        {
            return false;
        }

        EnemyLifetimeHook hook = enemy.GetComponent<EnemyLifetimeHook>();
        if(hook != null)
        {
            hook.SetSpawner(this);
        }

        return true;
    }

    Vector2 GetRandomPointOnRing(Vector3 center, float minRadius, float maxRadius)
    {
        float minR = Mathf.Max(0.0f, minRadius);
        float maxR = Mathf.Max(minR + 0.1f, maxRadius);

        // 방향과 반지름을 랜덤으로 결정
        float angle = Random.Range(0.0f, 360.0f);
        float rad = angle * Mathf.Deg2Rad;

        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        float radius = Random.Range(minR, maxR);

        Vector2 pos = (Vector2)center + dir * radius;
        return pos;
    }

    public void NotifyEnemyDied()
    {
        if(aliveEnemies > 0)
        {
            --aliveEnemies;
        }
    }
}
