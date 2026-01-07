using UnityEngine;

/// <summary>
/// DamageContext를 받아 최종 대미지를 계산하고,
/// 대상의 IDamageable에 전달하며 상태/넉백까지 처리하는 허브.
/// - 대상에 DamageableStats/StatusEffectHost가 없어도 안전하게 동작.
/// - 역호환: 대상에 Router만 붙이면 기존 Projectile도 그대로 작동(폴백 경로 유지).
/// </summary>
public class DamageRouter : MonoBehaviour
{
    private DamageableStats stats;            // 방어/저항
    private StatusEffectHost statusHost;      // 상태이상 관리자
    private Rigidbody2D body;                 // 넉백 임펄스(플레이어는 PlayerMovement 경유 권장)

    private void Awake()
    {
        stats = GetComponent<DamageableStats>();
        statusHost = GetComponent<StatusEffectHost>();
        body = GetComponent<Rigidbody2D>();
    }

    public void Receive(DamageContext ctx)
    {
        // 최종 대미지 계산
        bool didCrit = false;
        float finalDamage = DamageCalculator.ComputeFinalDamage(ctx, stats, out didCrit);

        // 피해 적용(필수)
        IDamageable d = GetComponent<IDamageable>();
        if (d != null)
        {
            d.ApplyDamage(finalDamage);
        }

        // 상태이상 적용(선택)
        if (statusHost != null)
        {
            if (ctx.applyBurn == true)
            {
                statusHost.ApplyBurn(ctx.burnDps, ctx.burnDuration);
            }
            if (ctx.applyFreeze == true)
            {
                statusHost.ApplyFreeze(ctx.freezeSlowPercent, ctx.freezeDuration);
            }
        }

        // 넉백(선택)
        if (ctx.knockbackForce > 0.0f)
        {
            Vector2 dir = Vector2.zero;
            if (ctx.attacker != null)
            {
                dir = (transform.position - ctx.attacker.transform.position).normalized;
            }
            else
            {
                dir = Random.insideUnitCircle.normalized;
            }

            // PlayerMovement 경유(있으면)
            PlayerHealth player = GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.ApplyKnockback(dir * ctx.knockbackForce);
                return;
            }

            // 일반 임펄스
            if (body != null)
            {
                body.AddForce(dir * ctx.knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}
