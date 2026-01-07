using UnityEngine;

public class SpawnDifficultyScaler : MonoBehaviour
{
    [SerializeField] 
    private float baseSpawnRatePerSecond = 1.0f; // 기본 초당 스폰 양
    
    [SerializeField] 
    private float spawnRateGrowthPerminute = 0.5f;   // 분당 증가량

    [SerializeField] 
    private float minSpawnRadius = 8.0f; // 플레이어 위치로부터 최소 서리
    
    [SerializeField] 
    private float maxSpawnRadius = 14.0f;    // 플레이어 위치로부터 최대 거리

    [SerializeField] 
    private int maxAliveEnemies = 60;

    public float GetBaseSpawnRatePerSecond()
    {
        return baseSpawnRatePerSecond;
    }

    public float GetSpawnRateGrowthPerMinute()
    {
        return spawnRateGrowthPerminute;
    }

    public float GetMinSpawnRadius()
    {
        return minSpawnRadius;
    }

    public float GetMaxSpawnRadius()
    {
        return maxSpawnRadius;
    }

    public int GetMaxAliveEnemies()
    {
        return maxAliveEnemies;
    }
}
