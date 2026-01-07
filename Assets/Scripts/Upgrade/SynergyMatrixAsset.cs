using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SynergyRule
{
    public string requiresUpgradeId;   // 이 업그레이드를
    public int requiresMinLevel = 1;   // 최소 몇 레벨 이상 갖고 있으면

    public string targetUpgradeId;     // 이 업그레이드가
    public int weightBonus = 5;        // 뽑힐 때 점수를 추가로 준다
}

/// <summary>
/// "잘 어울리는 조합" 규칙 표(에셋).
/// 조건을 만족하면 목표 업그레이드의 점수를 더해 준다.
/// </summary>
[CreateAssetMenu(menuName = "Game/Synergy Matrix")]
public class SynergyMatrixAsset : ScriptableObject
{
    [SerializeField]
    private List<SynergyRule> rules = new List<SynergyRule>();

    /// <summary>
    /// 특정 업그레이드 id가 뽑힐 때, 현재 보유 상태를 보고 보너스 점수를 계산한다.
    /// </summary>
    /// <param name="upgradeId">보너스를 계산할 업그레이드 id</param>
    /// <param name="state">현재 업그레이드 보유 상태</param>
    /// <returns>가산 점수(정수, 0 이상)</returns>
    /// <remarks>
    /// 부작용 없음.
    /// 예외: state가 null이면 0.
    /// </remarks>
    public int GetBonusFor(string upgradeId, UpgradeRuntimeState state)
    {
        if (state == null)
        {
            return 0;
        }

        int bonus = 0;

        for (int i = 0; i < rules.Count; ++i)
        {
            SynergyRule r = rules[i];
            if (r == null)
            {
                continue;
            }

            if (string.IsNullOrEmpty(r.targetUpgradeId) == true)
            {
                continue;
            }

            if (r.targetUpgradeId != upgradeId)
            {
                continue;
            }

            if (string.IsNullOrEmpty(r.requiresUpgradeId) == true)
            {
                bonus += Mathf.Max(0, r.weightBonus);
                continue;
            }

            int lv = state.GetLevel(r.requiresUpgradeId);
            if (lv >= r.requiresMinLevel)
            {
                bonus += Mathf.Max(0, r.weightBonus);
            }
        }

        if (bonus < 0)
        {
            bonus = 0;
        }

        return bonus;
    }
}
