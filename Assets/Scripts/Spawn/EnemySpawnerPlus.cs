using UnityEngine;

public class EnemySpawnerPlus : MonoBehaviour
{
    [SerializeField]
    private Transform[] spawnPoints; // 적을 찍어낼 위치 배열

    [SerializeField]
    private GameObject enemyPrefab; // 적 프리팹

    [SerializeField]
    private int maxAlive = 20; // 장면에 동시에 존재하도록 허용할 최대 수

    [SerializeField]
    private AliveRegistry aliveRegistry; // 살아 있는 수를 추적하는 레지스트리

    private int nextIndex; // 다음으로 사용할 지점의 인덱스

    private void OnEnable()
    {
        nextIndex = 0; // 시작 시 첫 지점부터 사용
    }

    public bool TrySpawnOne()
    {
        if (aliveRegistry != null)
        {
            int count = aliveRegistry.GetAliveCount(); // 현재 살아 있는 수
            if (count >= maxAlive)
            {
                return false;
            }
        }

        if (enemyPrefab == null)
        {
            return false;
        }

        if (spawnPoints == null)
        {
            return false;
        }

        if (spawnPoints.Length == 0)
        {
            return false;
        }

        Transform point = spawnPoints[nextIndex]; // 선택된 지점
        if (point == null)
        {
            return false;
        }

        ++nextIndex;
        if (nextIndex >= spawnPoints.Length)
        {
            nextIndex = 0;
        }

        GameObject go = Instantiate(enemyPrefab, point.position, point.rotation); // 프리팹 생성
        if (aliveRegistry != null)
        {
            aliveRegistry.Register(go); // 살아 있는 수에 등록
        }

        return true;
    }
}
