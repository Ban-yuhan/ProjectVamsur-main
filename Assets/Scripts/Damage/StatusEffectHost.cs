using UnityEngine;

/// <summary>
/// 화상(도트 대미지)/빙결 상태이상을 관리하는 호스트.
/// - 동일 계열은 더 강한 쪽/더 긴 쪽으로 갱신한다.
/// - 이동 감속은 외부 이동 로직과 합산될 수 있으니 주의.
/// </summary>
public class StatusEffectHost : MonoBehaviour
{
    // 점화
    private float burnDps;                // 초당 피해량
    private float burnRemain;             // 남은 시간

    // 빙결
    private float freezeSlowPercent;      // 이동 속도 감속률(0~1)
    private float freezeRemain;           // 남은 시간

    // 선택: 이동 속도 조절 대상(없어도 동작)
    [SerializeField]
    private PlayerMovement playerMovement;

    [SerializeField]
    private EnemyChase enemyChase;

    // 원래 이동 속도 캐시
    private float cachedPlayerMove;
    private float cachedEnemyMove;

    private void Awake()
    {
        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
        }
        if (enemyChase == null)
        {
            enemyChase = GetComponent<EnemyChase>();
        }

        if (playerMovement != null)
        {
            cachedPlayerMove = playerMovement.GetMoveSpeed();
        }
        if (enemyChase != null)
        {
            cachedEnemyMove = enemyChase.GetMoveSpeed();
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        // 점화 Tick
        if (burnRemain > 0.0f)
        {
            burnRemain = burnRemain - dt;
            // 초당 피해량 -> 프레임당 환산
            float damage = burnDps * dt;

            IDamageable self = GetComponent<IDamageable>();
            if (self != null)
            {
                // 자기 자신에게 피해를 준다(DoT)
                self.ApplyDamage(damage);
            }

            if (burnRemain <= 0.0f)
            {
                burnDps = 0.0f;
                burnRemain = 0.0f;
            }
        }

        // 빙결 Tick
        if (freezeRemain > 0.0f)
        {
            freezeRemain = freezeRemain - dt;

            ApplySlow(freezeSlowPercent);

            if (freezeRemain <= 0.0f)
            {
                ClearSlow();
            }
        }
    }

    public void ApplyBurn(float dps, float duration)
    {
        // 더 강한 DpS 또는 더 긴 지속으로 갱신
        if (dps > burnDps)
        {
            burnDps = dps;
        }
        if (duration > burnRemain)
        {
            burnRemain = duration;
        }
    }

    public void ApplyFreeze(float slowPercent, float duration)
    {
        if (slowPercent > freezeSlowPercent)
        {
            freezeSlowPercent = Mathf.Clamp01(slowPercent);
        }
        if (duration > freezeRemain)
        {
            freezeRemain = duration;
        }
    }

    private void ApplySlow(float slowPercent)
    {
        if (playerMovement != null)
        {
            float baseSpeed = cachedPlayerMove > 0.0f ? cachedPlayerMove : playerMovement.GetMoveSpeed();
            float slowed = baseSpeed * (1.0f - slowPercent);
            playerMovement.SetMoveSpeed(slowed);
        }
        if (enemyChase != null)
        {
            float baseSpeed = cachedEnemyMove > 0.0f ? cachedEnemyMove : enemyChase.GetMoveSpeed();
            float slowed = baseSpeed * (1.0f - slowPercent);
            enemyChase.SetMoveSpeed(slowed);
        }
    }

    private void ClearSlow()
    {
        if (playerMovement != null)
        {
            float baseSpeed = cachedPlayerMove > 0.0f ? cachedPlayerMove : playerMovement.GetMoveSpeed();
            playerMovement.SetMoveSpeed(baseSpeed);
        }

        if (enemyChase != null)
        {
            float baseSpeed = cachedEnemyMove > 0.0f ? cachedEnemyMove : enemyChase.GetMoveSpeed();
            enemyChase.SetMoveSpeed(baseSpeed);
        }
        freezeSlowPercent = 0.0f;
    }
}
