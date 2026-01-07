using System.Collections.Generic;
using UnityEngine;

public class DashDamagePulse : MonoBehaviour
{
    [SerializeField] private PlayerDash playerDash;
    [SerializeField] private float radius = 2.0f;
    [SerializeField] private float damage = 5.0f;
    [SerializeField] private float hitCooldownSec = 0.15f;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] private int bufferSize = 32;

    [SerializeField] private PlayerWeaponStatsRuntime stats;

    private Collider2D[] results;
    private Dictionary<Transform, float> lastHitTimeByTarget = new Dictionary<Transform, float>();

    private void Awake()
    {
        lastHitTimeByTarget.Clear();

        if(playerDash != null)
        {
            playerDash.OnDashStarted += HandleDashStarted;
        }
    }

    void OnDestroy()
    {
        if (playerDash != null)
        {
            playerDash.OnDashStarted -= HandleDashStarted;
        }
    }

    void HandleDashStarted()
    {
        ApplyPulseDamage();
    }

    // 같은 대상에게 대미지를 줄 수 있는지 여부.
    bool ShouldApplyHit(float now, Transform target)
    {
        if(lastHitTimeByTarget.TryGetValue(target, out float lastTime) == true)
        {
            float delta = now - lastTime;

            if(delta < hitCooldownSec)
            {
                return false;
            }
        }

        return true;
    }

    void ApplyPulseDamage()
    {
        if(stats == null)
        {
            return;
        }

        results = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);

        if(results == null || results.Length == 0)
        {
            return;
        }

        damage = stats.GetDashDamage();

        float now = Time.time;

        for(int i=0; i<results.Length; ++i)
        {
            Collider2D c = results[i];

            EnemyHealth enemyHealth = c.GetComponent<EnemyHealth>();

            if (enemyHealth == null)
            {
                continue;
            }

            if(ShouldApplyHit(now, enemyHealth.transform) == false)
            {
                continue;
            }

            lastHitTimeByTarget[enemyHealth.transform] = now;

            enemyHealth.ApplyDamage(damage);
        }
    }
}
