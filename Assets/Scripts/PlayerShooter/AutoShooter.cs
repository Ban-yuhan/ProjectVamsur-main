using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    public float attackCooldown = 0.5f;
    public GameObject projectilePrefab;

    public float projectileSpeed = 10.0f;
    public float projectileLifeTime = 3.0f;
    public float damage = 5.0f;
    public int projectileCount = 1;

    public float targetSearchRadius = 8.0f;
    public LayerMask targetLayerMask;
    public GameManager gameManager;

    public Transform firePoint;

    private float cooldownTimer;
    private float rotatingAngle;

    private void Awake()
    {
        cooldownTimer = 0.0f;
        rotatingAngle = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        bool canFire = false;
        if(gameManager != null)
        {
            canFire = gameManager.IsPlaying();
        }

        if(canFire == false)
        {
            return;
        }

        cooldownTimer -= Time.deltaTime;
        if(cooldownTimer <= 0.0f)
        {
            FireOneShot();
            cooldownTimer = attackCooldown;
        }
    }

    void FireOneShot()
    {
        Vector3 spawnPos = firePoint.position;

        Vector2 dir = GetAimDirection();
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        int count = Mathf.Max(1, projectileCount);
        float spread = 10.0f;
        float start = baseAngle - spread * (count - 1) * 0.5f;

        for(int i=0; i<count; ++i)
        {
            float angle = start + spread * i;
            Vector2 shotDir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            if (projectilePrefab != null)
            {
                GameObject obj = Instantiate(projectilePrefab, spawnPos, Quaternion.Euler(0.0f, 0.0f, angle));
                Projectile proj = obj.GetComponent<Projectile>();
                if (proj != null)
                {
                    proj.SetDirection(shotDir);
                    proj.Configure(projectileSpeed, projectileLifeTime, damage);
                }
            }
        }
    }

    Vector2 GetAimDirection()
    {
        Transform nearest = FindNearestTarget();
        if(nearest != null)
        {
            Vector2 toTarget = nearest.position - transform.position;
            return toTarget.normalized;
        }

        rotatingAngle -= 90.0f * Time.deltaTime;
        float rad = rotatingAngle * Mathf.Deg2Rad;  // 각도를 라디안 값으로 변경.
        Vector2 fallback = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        return fallback.normalized;
    }

    Transform FindNearestTarget()
    {
        Collider2D[] hits;

        hits = Physics2D.OverlapCircleAll(transform.position, targetSearchRadius, targetLayerMask);
        if(hits == null || hits.Length == 0)
        {
            return null;
        }

        float bestDist = float.MaxValue;
        Transform best = null;

        for(int i=0; i<hits.Length; ++i)
        {
            Transform t = hits[i].transform;
            //float dist = (t.position - transform.position).sqrMagnitude; 
            float dist = Vector2.Distance(t.position, transform.position);
            if(dist < bestDist)
            {
                bestDist = dist;
                best = t;
            }
        }

        return best;
    }

    public float GetAttackCooldown()
    {
        return attackCooldown;
    }

    public void SetAttackCooldown(float cooldown)
    {
        attackCooldown = cooldown;
    }

    public float GetDamage()
    {
        return damage;
    }

    public void SetDamage(float value)
    {
        damage = value;
    }

    public float GetProjectileSpeed()
    {
        return projectileSpeed;
    }

    public void SetProjectileSpeed(float speed)
    {
        projectileSpeed = speed;
    }

    public int GetProjectileCount()
    {
        return projectileCount;
    }

    public void SetProjectileCount(int count)
    {
        projectileCount = count;
    }
}
