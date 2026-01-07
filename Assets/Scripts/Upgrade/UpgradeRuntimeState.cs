using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 런타임 업그레이드 레벨 상태를 보관한다.
/// </summary>
public class UpgradeRuntimeState : MonoBehaviour
{
    private Dictionary<string, int> levelMap = new Dictionary<string, int>(); // id -> level

    /// <summary>
    /// 현재 레벨을 반환한다. 없으면 0.
    /// </summary>
    public int GetLevel(string id)
    {
        if (string.IsNullOrEmpty(id) == true)
        {
            return 0;
        }

        if (levelMap.ContainsKey(id) == true)
        {
            return levelMap[id];
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// 레벨을 1 증가시킨다.
    /// </summary>
    public void IncreaseLevel(string id)
    {
        if (string.IsNullOrEmpty(id) == true)
        {
            return;
        }

        int cur = 0;
        if (levelMap.ContainsKey(id) == true)
        {
            cur = levelMap[id];
        }
        ++cur;
        levelMap[id] = cur;
    }

    /// <summary>
    /// 특정 업그레이드가 최대 레벨인지 확인한다.
    /// </summary>
    public bool IsMaxed(UpgradeDefinition def)
    {
        if (def == null)
        {
            return true;
        }

        int lv = GetLevel(def.GetId());
        if (lv >= def.GetMaxLevel())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
