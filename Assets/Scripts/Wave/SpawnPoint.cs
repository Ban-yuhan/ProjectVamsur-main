using UnityEngine;

/// <summary>
/// 적을 소환하는 스폰 포인트.
/// - 안전 반경(safeRadius) 내에서 플레이어와의 최소 거리 조건을 만족하도록
///   약간의 위치 노이즈와 재시도를 수행한다.
/// - 풀링 전 단계이며, Instantiate 사용. 추후 오브젝트 풀로 교체 예정.
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    private float safeRadius = 1.0f; // 소환 시 반경 내 랜덤 오프셋의 최대 크기

    [SerializeField]
    private int positionRetry = 5;   // 안전 위치 재시도 횟수

    /// <summary>
    /// 요청된 적 프리팹을 스폰 포인트 부근에 소환한다.
    /// </summary>
    /// <param name="enemyPrefab">소환할 적 프리팹(필수)</param>
    /// <param name="minDistanceFromPlayer">플레이어와 최소 이격 거리</param>
    /// <returns>생성된 적의 GameObject, 실패 시 null</returns>
    /// <remarks>
    /// 부작용: 씬에 적 인스턴스를 추가한다.
    /// 예외: enemyPrefab이 null이면 아무 것도 하지 않는다.
    /// 성능: Instantiate 호출은 비용이 크다. 추후 풀링 전환 권장.
    /// </remarks>
    public GameObject Spawn(GameObject enemyPrefab, float minDistanceFromPlayer)
    {
        if (enemyPrefab == null)
        {
            return null;
        }

        Vector3 basePos = transform.position;
        Vector3 candidate = basePos;

        for (int i = 0; i < positionRetry; i = i + 1)
        {
            Vector2 jitter = Random.insideUnitCircle * safeRadius;
            candidate = basePos + new Vector3(jitter.x, jitter.y, 0.0f);

            if (IsFarFromPlayer(candidate, minDistanceFromPlayer) == true)
            {
                break;
            }
        }

        GameObject obj = GameObject.Instantiate(enemyPrefab, candidate, Quaternion.identity);
        return obj;
    }

    /// <summary>
    /// 특정 위치가 플레이어와 최소 이격 거리 이상인지 확인한다.
    /// </summary>
    /// <param name="pos">검사 위치</param>
    /// <param name="minDistance">최소 거리</param>
    /// <returns>충족하면 true, 아니면 false</returns>
    /// <remarks>
    /// 부작용 없음.
    /// 예외 없음.
    /// 성능: 단순 거리 비교 O(1).
    /// </remarks>
    private bool IsFarFromPlayer(Vector3 pos, float minDistance)
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p == null)
        {
            return true;
        }

        float d = Vector3.Distance(pos, p.transform.position);
        if (d >= minDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
