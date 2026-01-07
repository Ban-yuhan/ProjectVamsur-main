using UnityEngine;

/// <summary>
/// 풀에서 꺼내 쓰는 오브젝트에 부착
/// 자신이 어느 풀에서 왔는지 기억하고, 수명/충돌/화면 이탈 시 풀로 되돌린다
/// </summary>
public class PooledObject : MonoBehaviour
{
    [SerializeField]
    private float lifeTimeSeconds = 5.0f; // 수명

    private ObjectPool ownerPool;
    private float lifeTimer;

    /// <summary>
    /// 풀에서 나올 때 소유자와 수명 주기를 설정한다
    /// </summary>
    public void OnSpawned(ObjectPool pool)
    {
        ownerPool = pool;
        lifeTimer = lifeTimeSeconds;
    }

    private void Update()
    {
        if (lifeTimeSeconds > 0.0f)
        {
            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0.0f)
            {
                Release();
            }
        }
    }

    /// <summary>
    /// 수명 주기를 세팅
    /// </summary>
    public void SetLifeTime(float seconds)
    {
        lifeTimeSeconds = seconds;
        lifeTimer = seconds;
    }

    /// <summary>
    /// 풀로 다시 반환 시 호출
    /// </summary>
    public void Release()
    {
        if (ownerPool != null)
        {
            ownerPool.Release(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
