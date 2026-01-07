using UnityEngine;

public class BoomerangWeapon : MonoBehaviour
{
    [SerializeField]
    private Transform owner;

    [SerializeField]
    private GameObject boomerangPrefab;

    [SerializeField]
    private float attackCooldown = 1.0f;

    [SerializeField]
    private float damage = 5.0f;

    [SerializeField]
    private float projectileSpeed = 10.0f;

    [SerializeField]
    private float projectileCount = 1;

    [SerializeField]
    private float maxDistance = 6.0f;

    [SerializeField]
    private float hitCooldownSec = 0.15f;

    [SerializeField]
    private LayerMask enemyLayer;

    [SerializeField]
    private float aimRange = 8.0f;

    [SerializeField]
    private float spreadDeg = 16.0f;    // 발사 방향의 퍼짐 각도.

    [SerializeField]
    private PlayerWeaponStatsRuntime stats;

    private float attackTimer;

    private void Reset()
    {
        owner = transform;
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;

        if(attackTimer >= attackCooldown)
        {
            attackTimer -= attackCooldown;

            FireBoomerang();
        }
    }

    Vector2 Rotate(Vector2 v, float deg)
    {
        // 각도를 라디안 값으로 변환.
        float rad = deg * Mathf.Deg2Rad;

        // 회전에 필요한 값 계산.
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        // 회전 공식 계산.
        float x = (v.x * cos) - (v.y * sin);
        float y = (v.x * sin) + (v.y * sin);

        // 회전 결과 벡터 생성.
        Vector2 r = new Vector2(x, y).normalized;

        return r;
    }

    Vector2 FindAimDirection()
    {
        Vector2 origin = owner.position;

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, aimRange, enemyLayer);

        if(hits == null || hits.Length <= 0)
        {
            return Vector2.zero;
        }

        Transform bestTarget = null;    // 가장 가까운 적의 트랜스폼을 저장할 변수.
        float bestSqr = 0.0f;   // 가장 가까운 거리의 제곱값을 저장할 변수.

        for(int i=0; i<hits.Length; ++i)
        {
            Collider2D c = hits[i];

            Vector2 targetPos = c.bounds.center;
            Vector2 to = targetPos - origin;

            float sqr = to.sqrMagnitude;

            if(bestTarget == null)
            {
                bestTarget = c.transform;
                bestSqr = sqr;
            }
            else
            {
                if(sqr < bestSqr)
                {
                    bestTarget = c.transform;
                    bestSqr = sqr;
                }
            }
        }

        if(bestTarget == null)
        {
            return Vector2.zero;
        }

        Vector2 dir = (Vector2)bestTarget.position - origin;
        //dir = dir.normalized;

        return dir.normalized;
    }

    void SpawnOne(Vector2 dir)
    {
        if(stats == null)
        {
            return;
        }

        Vector3 spawnPos = owner.position;

        GameObject obj = Instantiate(boomerangPrefab, spawnPos, Quaternion.identity);
        if(obj == null)
        {
            return;
        }

        BoomerangProjectile proj = obj.GetComponent<BoomerangProjectile>();
        if(proj == null)
        {
            Destroy(obj);
            return;
        }

        damage = stats.GetBoomerangDamage();
        maxDistance = stats.GetBoomerangMaxDistance();
        projectileSpeed = stats.GetBoomerangSpeed();

        proj.Setup(owner, dir, projectileSpeed, damage, maxDistance, hitCooldownSec, enemyLayer);
    }

    void FireBoomerang()
    {
        if(boomerangPrefab == null)
        {
            return;
        }

        Vector2 mainDir = FindAimDirection();

        if(mainDir == Vector2.zero)
        {
            return;
        }

        float startDeg = (spreadDeg * 0.5f) * (projectileCount - 1);

        for(int i=0; i<projectileCount; ++i)
        {
            float offsetDeg = startDeg + (spreadDeg * i);
            Vector2 dir = Rotate(mainDir, offsetDeg);

            SpawnOne(dir);
        }
    }
}
