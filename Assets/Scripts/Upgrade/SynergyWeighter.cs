using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 후보들 중에서 "잘 어울리는 보너스 점수"를 더해 하나를 고른다.
/// </summary>
public class SynergyWeighter : MonoBehaviour
{
    [SerializeField]
    private SynergyMatrixAsset matrix;

    /// <summary>
    /// 후보 리스트에서 하나를 점수 비율로 뽑는다.
    /// </summary>
    /// <param name="candidates">후보 업그레이드들</param>
    /// <param name="state">현재 내가 가진 업그레이드 상태</param>
    /// <returns>선택된 업그레이드 또는 null</returns>
    /// <remarks>
    /// 부작용 없음.
    /// 예외: 후보가 없으면 null.
    /// </remarks>
    public UpgradeDefinition PickWithSynergy(List<UpgradeDefinition> candidates, UpgradeRuntimeState state)
    {
        if (candidates == null || candidates.Count == 0)
        {
            return null;
        }

        int sum = 0;
        int[] weights = new int[candidates.Count];

        for (int i = 0; i < candidates.Count; ++i)
        {
            UpgradeDefinition d = candidates[i];
            if (d == null)
            {
                weights[i] = 0;
                continue;
            }

            int w = 1; // 기본 점수 1
            if (matrix != null)
            {
                w += matrix.GetBonusFor(d.GetId(), state);
            }

            if (w < 0)
            {
                w = 0;
            }

            weights[i] = w;
            sum += w;
        }

        if (sum <= 0)
        {
            int idxFallback = Random.Range(0, candidates.Count);
            return candidates[idxFallback];
        }

        int roll = Random.Range(0, sum);
        int acc = 0;

        for (int i = 0; i < candidates.Count; ++i)
        {
            acc += weights[i];
            if (roll < acc)
            {
                return candidates[i];
            }
        }

        return candidates[candidates.Count - 1];
    }
}
