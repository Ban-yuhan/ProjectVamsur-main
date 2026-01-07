using UnityEngine;

/// <summary>
/// 적의 원거리 사격을 처리.
/// 쿨다운 관리, 플레이어가 사격 가능 범위 안에 있으면 총알 프리팹을 발사.
/// </summary>
public class EnemyShooter : MonoBehaviour
{
    [SerializeField]
    private Transform player;

    [SerializeField]
    private Transform muzzle;   // 총알을 발사할 위치.

    [SerializeField]
    private GameObject projectilePrefab;    // 총알 프리팹.

    [SerializeField]
    private float fireRange = 7.0f;

    [SerializeField]
    private float fireCooldown = 3.0f;

    [SerializeField]
    private string projectileKey = "ProjectileEnemy";

    private float cooldownTimer = 0.0f;
    private PoolManager poolManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go != null)
        {
            player = go.transform;
        }

        poolManager = GameObject.FindAnyObjectByType<PoolManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // 플레이어가 없을 경우.
        if(player == null)
        {
            return;
        }

        // 총알 프리팹이 없을 경우.
        if (projectilePrefab == null)
        {
            return;
        }

        // 총알 발사 트랜스폼이 없을 경우.
        if(muzzle == null)
        {
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);
        if(dist > fireRange)    // 플레이어와의 거리가 사격 가능 거리보다 멀 경우.
        {
            return;
        }

        if (cooldownTimer > 0.0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // 발사 가능 쿨타임이 아직 안됐을 경우.
        if (cooldownTimer > 0.0f)
        {
            return;
        }

        FireOne();
        cooldownTimer = fireCooldown;
    }

    /// <summary>
    /// 총알 발사.
    /// </summary>
    /// <returns></returns>
    void FireOne()
    {
        PooledObject projObj = poolManager.Spawn(projectileKey, muzzle.position, Quaternion.identity);
        if (projObj == null)
        {
            return;
        }

        Projectile proj = projObj.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.SetDirection((player.position - transform.position).normalized);
            proj.SetOwner(gameObject);
        }
    }
}
