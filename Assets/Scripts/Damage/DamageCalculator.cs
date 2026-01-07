using UnityEngine;

/// <summary>
/// 최종 대미지 산출기:
/// 1) 크리 판정 -> 2) 방어력 감쇄(완만형) -> 3) 클램프
/// </summary>
public static class DamageCalculator
{
    public static float ComputeFinalDamage(DamageContext ctx, DamageableStats targetStats, out bool didCrit)
    {
        // 1) 크리 판정
        didCrit = false;
        float working = ctx.baseDamage;

        if (ctx.canCrit == true)
        {
            float r = Random.value; // 0~1
            if (r < ctx.critChance)
            {
                didCrit = true;
                working = working * ctx.critMultiplier;
            }
        }

        // 2) 방어력 감쇄(완만형): reduction = def / (def + 100)
        float defense = 0.0f;
        float cap = 0.8f; // 80% 최대 감쇄
        if (targetStats != null)
        {
            defense = Mathf.Max(0.0f, targetStats.GetDefense());
            cap = Mathf.Clamp01(targetStats.GetReductionCap());
        }

        float reduction = 0.0f;
        if (defense > 0.0f)
        {
            reduction = defense / (defense + 100.0f);
        }
        reduction = Mathf.Min(reduction, cap);

        working = working * (1.0f - reduction);

        // 3) 하한/상한 (과도한 소수점/음수 방지)
        if (working < 0.0f)
        {
            working = 0.0f;
        }

        return working;
    }
}
