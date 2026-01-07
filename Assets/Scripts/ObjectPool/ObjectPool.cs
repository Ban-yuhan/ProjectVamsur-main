using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 프리팹을 관리하는 오브젝트 풀 스크립트
/// </summary>
public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private string poolKey = "Projectile"; // PoolManager에서 찾을 때 쓰는 키

    [SerializeField]
    private PooledObject prefab; // 프리팹

    [SerializeField]
    private int warmCount = 30; // 미리 만들어 놓을 개수

    private readonly Queue<PooledObject> queue = new Queue<PooledObject>();
    private int totalCreated;

    /// <summary>
    /// PoolManager가 키로 이풀을 등록할 때 사용하기 위한 함수
    /// </summary>
    public string GetKey()
    {
        return poolKey;
    }

    private void Awake()
    {
        Warm(warmCount);
    }

    /// <summary>
    /// 인자로 받은 카운트만큼 총알을 생성
    /// </summary>
    public void Warm(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            CreateOneInactive();
        }
    }

    /// <summary>
    /// Queue에서 총알을 꺼내오는 함수
    /// </summary>
    public PooledObject Spawn()
    {
        PooledObject obj = null;

        if (queue.Count > 0)
        {
            obj = queue.Dequeue();
        }
        else
        {
            obj = CreateOneInactive();
        }

        if (obj != null)
        {
            obj.gameObject.SetActive(true);
            obj.OnSpawned(this);
        }

        return obj;
    }

    /// <summary>
    /// 사용이 끝난 총알을 반환
    /// </summary>
    public void Release(PooledObject pooled)
    {
        if (pooled == null)
        {
            return;
        }

        pooled.gameObject.SetActive(false);
        queue.Enqueue(pooled);
    }

    /// <summary>
    /// 현재 사용가능한 총알의 수를 반환
    /// </summary>
    public int GetAvailableCount()
    {
        return queue.Count;
    }

    /// <summary>
    /// 미리 생성된 총알의 카운트를 반환
    /// </summary>
    public int GetTotalCreated()
    {
        return totalCreated;
    }

    /// <summary>
    /// 미리 총알을 만들어 두는 함수
    /// </summary>
    private PooledObject CreateOneInactive()
    {
        if (prefab == null)
        {
            Debug.LogWarning("ObjectPool: prefab is null.");
            return null;
        }

        PooledObject obj = Instantiate(prefab, transform);
        ++totalCreated;
        obj.gameObject.SetActive(false);
        queue.Enqueue(obj);
        return obj;
    }
}
