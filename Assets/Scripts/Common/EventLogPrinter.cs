using UnityEngine;

/// <summary>
/// 전투 알림 흐름을 콘솔로 보여주는 도우미
/// </summary>
public class EventLogPrinter : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus.OnHit += OnHit;
        EventBus.OnDamageApplied += OnDamageApplied;
        EventBus.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        EventBus.OnHit -= OnHit;
        EventBus.OnDamageApplied -= OnDamageApplied;
        EventBus.OnDeath -= OnDeath;
    }

    private void OnHit(HitContext c)
    {
        Debug.Log("닿음 알림");
    }

    private void OnDamageApplied(DamageAppliedEvent e)
    {
        Debug.Log("피해확정 알림");
    }

    private void OnDeath(DeathEvent e)
    {
        Debug.Log("사망 알림");
    }
}
