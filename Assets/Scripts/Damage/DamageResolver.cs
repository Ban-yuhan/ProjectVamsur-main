using UnityEngine;

/// <summary>
/// 최종 피해 계산 순수 함수
/// </summary>
public static class DamageResolver
{
    /// <summary>
    /// 히트 문맥과 대상 저항, 전달된 피해값을 바탕으로 최종 피해를 계산한다
    /// </summary>
    public static float ComputeFinalDamage(HitContext context, float targetResistance, float incomingDamage)
    {
        float resistance = targetResistance; // 대상 저항
        if (context.damageType == DamageType.Piercing)
        {
            resistance *= 0.5f; // 관통 피해는 저항 절반만 적용
        }

        float reduced = incomingDamage * (1.0f - resistance); // 저항 적용 결과
        if (reduced < 0.0f)
        {
            reduced = 0.0f; // 음수 방지
        }

        return reduced;
    }
}
