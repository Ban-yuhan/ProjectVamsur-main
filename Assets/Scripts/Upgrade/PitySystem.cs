using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 보장 장치(운이 너무 없을 때 한 칸 보장).
/// 최근 여러 번의 뽑기에서 좋은 등급이 전혀 안 나오면,
/// 이번 뽑기에서 "한 칸"의 등급을 올려 준다.
/// </summary>
public class PitySystem : MonoBehaviour
{
    [SerializeField]
    private int rarePityThreshold = 6;

    [SerializeField]
    private int epicPityThreshold = 12;

    private int sinceLastRare;
    private int sinceLastEpic;

    /// <summary>
    /// 이번 뽑기가 끝난 뒤, 실제로 나온 등급 목록을 알려주면 카운터를 갱신한다.
    /// </summary>
    /// <param name="finalRarities">확정된 옵션들의 등급</param>
    public void NotifyFinalRarities(List<UpgradeRarity> finalRarities)
    {
        bool anyRareOrMore = false;
        bool anyEpic = false;

        for (int i = 0; i < finalRarities.Count; ++i)
        {
            UpgradeRarity r = finalRarities[i];
            if (r == UpgradeRarity.Rare || r == UpgradeRarity.Epic)
            {
                anyRareOrMore = true;
            }
            if (r == UpgradeRarity.Epic)
            {
                anyEpic = true;
            }
        }

        if (anyRareOrMore == true)
        {
            sinceLastRare = 0;
        }
        else
        {
            ++sinceLastRare;
        }

        if (anyEpic == true)
        {
            sinceLastEpic = 0;
        }
        else
        {
            ++sinceLastEpic;
        }
    }

    /// <summary>
    /// 뽑기 전에 현재 등급 배열을 받아, 필요하면 "한 칸"의 등급을 올려준다.
    /// </summary>
    /// <param name="rarities">이번 드래프트의 등급 배열</param>
    /// <remarks>
    /// 부작용: 배열 내용이 바뀐다.
    /// 예외 없음.
    /// </remarks>
    public void ApplyPityIfNeeded(List<UpgradeRarity> rarities)
    {
        if (rarities == null)
        {
            return;
        }

        if (sinceLastEpic >= epicPityThreshold)
        {
            PromoteOneSlot(rarities, UpgradeRarity.Epic);
            sinceLastEpic = 0;
            return;
        }

        if (sinceLastRare >= rarePityThreshold)
        {
            PromoteOneSlot(rarities, UpgradeRarity.Rare);
            sinceLastRare = 0;
        }
    }

    /// <summary>
    /// 등급을 "한 칸" 올려준다. 가능한 한 낮은 등급 칸을 올린다.
    /// </summary>
    private void PromoteOneSlot(List<UpgradeRarity> rarities, UpgradeRarity target)
    {
        int lowestIndex = -1;
        int lowestRank = 999;

        for (int i = 0; i < rarities.Count; ++i)
        {
            int rank = RankOf(rarities[i]);
            if (rank < lowestRank)
            {
                lowestRank = rank;
                lowestIndex = i;
            }
        }

        if (lowestIndex >= 0)
        {
            if (target == UpgradeRarity.Epic)
            {
                rarities[lowestIndex] = UpgradeRarity.Epic;
            }
            else if (target == UpgradeRarity.Rare)
            {
                if (rarities[lowestIndex] == UpgradeRarity.Common)
                {
                    rarities[lowestIndex] = UpgradeRarity.Rare;
                }
                else if (rarities[lowestIndex] == UpgradeRarity.Uncommon)
                {
                    rarities[lowestIndex] = UpgradeRarity.Rare;
                }
            }
        }
    }

    /// <summary>
    /// 등급을 숫자로 바꿔 우열을 비교하기 쉽게 만든다(낮을수록 낮은 등급).
    /// </summary>
    private int RankOf(UpgradeRarity r)
    {
        if (r == UpgradeRarity.Common)
        {
            return 0;
        }
        if (r == UpgradeRarity.Uncommon)
        {
            return 1;
        }
        if (r == UpgradeRarity.Rare)
        {
            return 2;
        }
        return 3;
    }
}
