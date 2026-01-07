using System;
using UnityEngine;

/// <summary>
/// 전역 전투 알림 허브
/// </summary>
public static class EventBus
{
    public static event Action<HitContext> OnHit; // 명중 알림
    public static event Action<DamageAppliedEvent> OnDamageApplied; // 피해확정 알림
    public static event Action<DeathEvent> OnDeath; // 사망 알림

    public static void PublishHit(HitContext context)
    {
        Action<HitContext> h = OnHit; // 구독자 참조
        if (h != null)
        {
            h.Invoke(context);
        }
    }

    public static void PublishDamageApplied(DamageAppliedEvent e)
    {
        Action<DamageAppliedEvent> h = OnDamageApplied; // 구독자 참조
        if (h != null)
        {
            h.Invoke(e);
        }
    }

    public static void PublishDeath(DeathEvent e)
    {
        Action<DeathEvent> h = OnDeath; // 구독자 참조
        if (h != null)
        {
            h.Invoke(e);
        }
    }
}

/// <summary>
/// 피해확정 알림에 담기는 데이터
/// </summary>
public struct DamageAppliedEvent
{
    public HitContext context;   // 히트 문맥
    public float finalDamage;    // 최종 피해
    public float remainingHp;    // 적용 후 체력
}

/// <summary>
/// 사망 알림에 담기는 데이터
/// </summary>
public struct DeathEvent
{
    public HitContext context;   // 마지막 히트 문맥
    public Transform victim;     // 희생자 Transform
}
