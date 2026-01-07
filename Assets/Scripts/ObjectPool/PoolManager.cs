using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 여러 ObjectPool을 묶어 키로 관리한다
/// Spawn/Release를 간단히 호출할 수 있게 해준다.
/// </summary>
public class PoolManager : MonoBehaviour
{
    [SerializeField]
    private List<ObjectPool> pools = new List<ObjectPool>();

    private readonly Dictionary<string, ObjectPool> map = new Dictionary<string, ObjectPool>();

    private void Awake()
    {
        BuildMap();
    }

    /// <summary>
    /// 등록된 풀을 키로 맵에 올린다
    /// </summary>
    private void BuildMap()
    {
        map.Clear();
        for (int i = 0; i < pools.Count; ++i)
        {
            ObjectPool p = pools[i];
            if (p == null)
            {
                continue;
            }

            string key = p.GetKey();
            if (string.IsNullOrEmpty(key) == true)
            {
                continue;
            }

            if (map.ContainsKey(key) == false)
            {
                map.Add(key, p);
            }
            else
            {
                Debug.LogWarning("PoolManager: duplicated key = " + key);
            }
        }
    }

    /// <summary>
    /// 키를 이용해서 오브젝트 하나를 꺼낸다
    /// </summary>
    public PooledObject Spawn(string key, Vector3 pos, Quaternion rot)
    {
        ObjectPool p;
        if (map.TryGetValue(key, out p) == false)
        {
            Debug.LogWarning("PoolManager: no pool for key = " + key);
            return null;
        }

        PooledObject obj = p.Spawn();
        if (obj != null)
        {
            Transform t = obj.transform;
            t.position = pos;
            t.rotation = rot;
        }

        return obj;
    }

    /// <summary>
    /// 오브젝트를 반환
    /// </summary>
    public void Release(PooledObject obj)
    {
        if (obj == null)
        {
            return;
        }

        ObjectPool pool = obj.GetComponentInParent<ObjectPool>();
        if (pool != null)
        {
            pool.Release(obj);
            return;
        }

        // 만약 소유 풀을 못찾았을 경우 그냥 비활성화 처리
        obj.gameObject.SetActive(false);
    }
}
