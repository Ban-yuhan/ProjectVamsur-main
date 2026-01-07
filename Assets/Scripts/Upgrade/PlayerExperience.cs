using UnityEngine;
using System;

/// <summary>
/// 플레이어의 경험치/레벨을 관리한다.
/// 필요 경험치 곡선은 간단한 증가식으로 제공하며, 레벨업 시 이벤트를 발생시킨다.
/// UI와 드래프트는 외부(예: LevelUpDraftUI)가 이 이벤트를 구독해 처리한다.
/// </summary>
public class PlayerExperience : MonoBehaviour
{
    [SerializeField]
    private int currentLevel = 1;         // 현재 레벨(최소 1)

    [SerializeField]
    private float currentExp = 0.0f;      // 현재 경험치

    [SerializeField]
    private float baseRequired = 20.0f;   // 1 -> 2 레벨업 필요 경험치

    [SerializeField]
    private float growthFactor = 1.12f;   // 레벨업마다 필요 경험치 성장 계수

    public event Action<int> OnLevelUp;   // 레벨업 시 (새 레벨) 알림

    /// <summary>
    /// 경험치를 추가한다.
    /// </summary>
    /// <param name="amount">추가할 경험치 양(양수)</param>
    /// <remarks>
    /// 부작용: currentExp가 증가하며, 필요량을 넘으면 레벨업 처리/이벤트 발생.
    /// 예외: amount가 0 이하이면 무시한다.
    /// </remarks>
    public void AddExp(float amount)
    {
        if (amount <= 0.0f)
        {
            return;
        }

        currentExp += amount;

        // 다단 레벨업 처리
        while (currentExp >= GetRequiredExpForNextLevel())
        {
            currentExp -= GetRequiredExpForNextLevel();
            ++currentLevel;

            if (OnLevelUp != null)
            {
                OnLevelUp(currentLevel);
            }
        }
    }

    /// <summary>
    /// 다음 레벨까지 필요한 경험치를 반환한다.
    /// </summary>
    public float GetRequiredExpForNextLevel()
    {
        float req = baseRequired * Mathf.Pow(growthFactor, currentLevel - 1);
        return req;
    }

    /// <summary>
    /// 현재 레벨을 반환한다.
    /// </summary>
    public int GetLevel()
    {
        return currentLevel;
    }

    /// <summary>
    /// 현재 경험치 비율(0~1)을 반환한다.
    /// </summary>
    public float GetExpRatio()
    {
        float req = GetRequiredExpForNextLevel();
        if (req <= 0.0f)
        {
            return 0.0f;
        }

        float r = Mathf.Clamp01(currentExp / req);
        return r;
    }
}
