using UnityEngine;
using System.Collections;

/// <summary>
/// 보스 전방으로 직선 휩쓸기(돌진) 공격.
/// 텔레그래프 동안 방향을 잠시 고정한 뒤, Execute에서 실제 판정.
/// </summary>
public class SweepPattern : AttackPatternBase
{
    [SerializeField]
    private float range = 6.0f;              // 휩쓸기 길이

    [SerializeField]
    private float width = 1.2f;              // 폭(사다리꼴/직사각 대용)

    [SerializeField]
    private float damage = 12.0f;

    [SerializeField]
    private float knockback = 6.0f;

    [SerializeField]
    private LayerMask targetMask;

    private Vector2 cachedDir;               // 텔레그래프 시 고정되는 방향

    protected override IEnumerator Prepare()
    {
        // 플레이어 방향 고정
        Transform player = FindPlayer();
        if (player != null)
        {
            Vector2 to = player.position - transform.position;
            if (to.sqrMagnitude > 0.0001f)
            {
                cachedDir = to.normalized;
            }
        }
        else
        {
            cachedDir = Vector2.right;
        }

        if (telegraph != null)
        {
            telegraph.ShowSweep(transform.position, cachedDir, range, width);
        }

        float t = 0.0f;
        while (t < prepareTime)
        {
            if (gameManager != null)
            {
                if (gameManager.IsPlaying() == false)
                {
                    yield return null;
                    continue;
                }
            }
            t = t + Time.deltaTime;
            yield return null;
        }

        if (telegraph != null)
        {
            telegraph.Hide();
        }
    }

    protected override IEnumerator Execute()
    {
        // 간단한 충돌: 폭이 width 인 직사각 형태를 OverlapBox로 판정
        Vector2 center = (Vector2)transform.position + cachedDir * (range * 0.5f);
        float angle = Mathf.Atan2(cachedDir.y, cachedDir.x) * Mathf.Rad2Deg;

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, new Vector2(range, width), angle, targetMask);
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
            hs.DoHitstop(0.06f);
        }

        CameraShaker shaker = FindAnyObjectByType<CameraShaker>();
        if (shaker != null)
        {
            shaker.Shake(0.18f, 0.15f);
        }

        yield return null;
    }

    private Transform FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            return p.transform;
        }
        else
        {
            return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (cachedDir.sqrMagnitude <= 0.0001f)
        {
            cachedDir = Vector2.right;
        }
        Vector2 center = (Vector2)transform.position + cachedDir * (range * 0.5f);
        float angle = Mathf.Atan2(cachedDir.y, cachedDir.x) * Mathf.Rad2Deg;

        Gizmos.color = Color.magenta;
        // 대략적 시각화(회전된 박스는 정확 시각화가 어렵지만 참조용)
        Gizmos.DrawWireCube(center, new Vector3(range, width, 0.1f));
    }
}
