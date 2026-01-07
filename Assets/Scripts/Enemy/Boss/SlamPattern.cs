using UnityEngine;
using System.Collections;

/// <summary>
/// 보스 주변 원형 범위 강타.
/// </summary>
public class SlamPattern : AttackPatternBase
{
    [SerializeField]
    private float radius = 3.5f;

    [SerializeField]
    private float damage = 18.0f;

    [SerializeField]
    private float knockback = 8.0f;

    [SerializeField]
    private LayerMask targetMask;

    protected override IEnumerator Execute()
    {
        Collider2D[] hits;
        if (targetMask.value != 0)
        {
            hits = Physics2D.OverlapCircleAll(transform.position, radius, targetMask);
        }
        else
        {
            hits = Physics2D.OverlapCircleAll(transform.position, radius);
        }

        if (hits != null)
        {
            for (int i = 0; i < hits.Length; i = i + 1)
            {
                DamageRouter router = hits[i].GetComponent<DamageRouter>();
                if (router != null)
                {
                    DamageContext ctx = new DamageContext();
                    ctx.baseDamage = damage;
                    ctx.canCrit = false;
                    ctx.knockbackForce = knockback;
                    ctx.attacker = gameObject;

                    router.Receive(ctx);
                }
            }
        }

        HitstopManager hs = FindAnyObjectByType<HitstopManager>();
        if (hs != null)
        {
            hs.DoHitstop(0.08f);
        }

        CameraShaker shaker = FindAnyObjectByType<CameraShaker>();
        if (shaker != null)
        {
            shaker.Shake(0.25f, 0.2f);
        }

        yield return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
