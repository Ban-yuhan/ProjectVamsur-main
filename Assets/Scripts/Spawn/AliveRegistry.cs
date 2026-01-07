using System.Collections.Generic;
using UnityEngine;

public class AliveRegistry : MonoBehaviour
{
    private HashSet<GameObject> aliveSet = new HashSet<GameObject>(); // 살아 있는 오브젝트 모음

    public void Register(GameObject go)
    {
        if (go == null)
        {
            return;
        }
        aliveSet.Add(go); // 집합에 추가
    }

    public void Unregister(GameObject go)
    {
        if (go == null)
        {
            return;
        }
        aliveSet.Remove(go); // 집합에서 제거
    }

    public int GetAliveCount()
    {
        return aliveSet.Count; // 현재 살아 있는 수
    }
}
