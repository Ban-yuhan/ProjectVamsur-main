using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 업그레이드 옵션을 만드는 확장 버전 풀.
/// 순서: (1) 시간 그래프 점수로 등급 뽑기 -> (2) 보장 장치 적용 -> (3) 잘 어울리는 조합으로 후보 선택
/// + 이미 최대 레벨인 것은 빼고, 없으면 한 단계 낮은 등급에서 대신 고른다.
/// + 마지막으로 "항상 N장 채우기" 보강 단계로 빈 칸을 채운다.
/// </summary>
public class UpgradePoolPlus : MonoBehaviour
{
    [SerializeField]
    private List<UpgradeDefinition> allDefs = new List<UpgradeDefinition>();

    [Header("참조 컴포넌트")]
    [SerializeField]
    private RarityCurveProvider rarityCurves;

    [SerializeField]
    private PitySystem pity;

    [SerializeField]
    private SynergyWeighter synergy;

    /// <summary>
    /// 옵션 N개를 만들어 돌려준다.
    /// </summary>
    /// <param name="count">필요한 옵션 개수</param>
    /// <param name="state">현재 보유 업그레이드 상태(최대 레벨 제외에 필요)</param>
    /// <param name="elapsedSeconds">게임 시작 후 지난 시간(초)</param>
    /// <returns>뽑힌 업그레이드 목록</returns>
    /// <remarks>
    /// 부작용 없음(리스트를 새로 만들어 반환).
    /// 예외: 정의가 없으면 빈 리스트.
    /// </remarks>
    public List<UpgradeDefinition> RollOptions(int count, UpgradeRuntimeState state, float elapsedSeconds)
    {
        List<UpgradeDefinition> result = new List<UpgradeDefinition>();
        if (count <= 0)
        {
            return result;
        }

        int wC;
        int wU;
        int wR;
        int wE;

        if (rarityCurves != null)
        {
            rarityCurves.GetWeightsByTime(elapsedSeconds, out wC, out wU, out wR, out wE);
        }
        else
        {
            wC = 60;
            wU = 28;
            wR = 10;
            wE = 2;
        }

        // (1) 등급 배열 만들기
        List<UpgradeRarity> rarities = new List<UpgradeRarity>();
        for (int i = 0; i < count; ++i)
        {
            rarities.Add(PickRarity(wC, wU, wR, wE));
        }

        // (2) 보장 장치 적용
        if (pity != null)
        {
            pity.ApplyPityIfNeeded(rarities);
        }

        // (3) 시너지 반영해 각 등급에서 하나 고르기(없으면 하향 대체)
        for (int i = 0; i < rarities.Count; ++i)
        {
            UpgradeDefinition picked = PickOneByRarityWithSynergy(rarities[i], state);
            if (picked == null)
            {
                picked = TryFallbackPick(state, rarities[i]);
            }

            if (picked != null)
            {
                result.Add(picked);
            }
        }

        // (4) 항상 N장 채우기 보강
        EnsureFullOptions(result, count, state);

        // (5) 보장 장치 기록 갱신
        if (pity != null)
        {
            List<UpgradeRarity> finalR = new List<UpgradeRarity>();
            for (int i = 0; i < result.Count; ++i)
            {
                finalR.Add(result[i].GetRarity());
            }
            pity.NotifyFinalRarities(finalR);
        }

        return result;
    }

    /// <summary>
    /// 등급 점수로 하나의 등급을 고른다.
    /// </summary>
    private UpgradeRarity PickRarity(int wC, int wU, int wR, int wE)
    {
        int sum = wC + wU + wR + wE;
        int roll = Random.Range(0, sum);
        int acc = 0;

        acc += wC;
        if (roll < acc)
        {
            return UpgradeRarity.Common;
        }

        acc += wU;
        if (roll < acc)
        {
            return UpgradeRarity.Uncommon;
        }

        acc += wR;
        if (roll < acc)
        {
            return UpgradeRarity.Rare;
        }

        return UpgradeRarity.Epic;
    }

    /// <summary>
    /// 해당 등급에서 "최대 레벨이 아닌" 후보들을 모아,
    /// 잘 어울리는 조합 점수를 반영해 하나 고른다.
    /// </summary>
    private UpgradeDefinition PickOneByRarityWithSynergy(UpgradeRarity rarity, UpgradeRuntimeState state)
    {
        List<UpgradeDefinition> candidates = new List<UpgradeDefinition>();

        for (int i = 0; i < allDefs.Count; ++i)
        {
            UpgradeDefinition d = allDefs[i];
            if (d == null)
            {
                continue;
            }

            if (d.GetRarity() != rarity)
            {
                continue;
            }

            if (state != null)
            {
                if (state.IsMaxed(d) == true)
                {
                    continue;
                }
            }

            candidates.Add(d);
        }

        if (candidates.Count == 0)
        {
            return null;
        }

        if (synergy != null)
        {
            return synergy.PickWithSynergy(candidates, state);
        }
        else
        {
            int idx = Random.Range(0, candidates.Count);
            return candidates[idx];
        }
    }

    /// <summary>
    /// 이 등급에서 뽑을 게 없으면, 한 단계씩 낮춰서 대신 고른다.
    /// 마지막에도 없으면 "아무 등급이나 비맥스"에서 하나 집어온다.
    /// </summary>
    private UpgradeDefinition TryFallbackPick(UpgradeRuntimeState state, UpgradeRarity start)
    {
        UpgradeRarity[] orderEpic = { UpgradeRarity.Rare, UpgradeRarity.Uncommon, UpgradeRarity.Common };
        UpgradeRarity[] orderRare = { UpgradeRarity.Uncommon, UpgradeRarity.Common };
        UpgradeRarity[] orderUncommon = { UpgradeRarity.Common };
        UpgradeRarity[] none = { };

        UpgradeRarity[] order = none;
        if (start == UpgradeRarity.Epic)
        {
            order = orderEpic;
        }
        else if (start == UpgradeRarity.Rare)
        {
            order = orderRare;
        }
        else if (start == UpgradeRarity.Uncommon)
        {
            order = orderUncommon;
        }
        else
        {
            order = none;
        }

        for (int i = 0; i < order.Length; ++i)
        {
            UpgradeDefinition alt = PickOneByRarityWithSynergy(order[i], state);
            if (alt != null)
            {
                return alt;
            }
        }

        // 최후: 모든 희귀도에서 비맥스 아무거나
        List<UpgradeDefinition> anyPool = CollectNonMaxed(state);
        if (anyPool.Count > 0)
        {
            int anyIdx = Random.Range(0, anyPool.Count);
            return anyPool[anyIdx];
        }

        return null;
    }

    /// <summary>
    /// 모든 정의 중 "최대 레벨이 아닌" 것들을 모은다.
    /// </summary>
    private List<UpgradeDefinition> CollectNonMaxed(UpgradeRuntimeState state)
    {
        List<UpgradeDefinition> list = new List<UpgradeDefinition>();
        for (int i = 0; i < allDefs.Count; ++i)
        {
            UpgradeDefinition d = allDefs[i];
            if (d == null)
            {
                continue;
            }

            if (state != null)
            {
                if (state.IsMaxed(d) == true)
                {
                    continue;
                }
            }

            list.Add(d);
        }
        return list;
    }

    /// <summary>
    /// 결과 리스트가 부족하면 아무 희귀도든 섞어서 채운다.
    /// 중복 허용. 정말 후보가 없으면 "대체 카드"로 채운다.
    /// </summary>
    private void EnsureFullOptions(List<UpgradeDefinition> result, int desiredCount, UpgradeRuntimeState state)
    {
        if (result == null)
        {
            return;
        }

        int safety = 32;
        while (result.Count < desiredCount && safety > 0)
        {
            --safety;

            List<UpgradeDefinition> pool = CollectNonMaxed(state);
            if (pool.Count > 0)
            {
                int idx = Random.Range(0, pool.Count);
                result.Add(pool[idx]);
                continue;
            }

            UpgradeDefinition fallback = GetFallbackCard();
            if (fallback != null)
            {
                result.Add(fallback);
                continue;
            }

            break;
        }
    }

    /// <summary>
    /// 후보가 완전히 고갈되었을 때 대신 넣을 "대체 카드"를 돌려준다.
    /// 프로젝트에 없다면 null을 반환한다.
    /// </summary>
    private UpgradeDefinition GetFallbackCard()
    {
        // 선택지:
        // 1) 전용 SO "RerollCard"를 만들어 등록 후 반환
        // 2) 회복/골드/리롤 같은 특수 카드
        return null;
    }
}
