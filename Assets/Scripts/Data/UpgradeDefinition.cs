using UnityEngine;

public enum UpgradeRarity
{
    Common,
    Uncommon,
    Rare,
    Epic
}

/// <summary>
/// 업그레이드 데이터 정의(ScriptableObject).
/// 밸런싱을 에디터에서 데이터로 조정할 수 있게 분리한다.
/// </summary>
[CreateAssetMenu(menuName = "Game/Upgrade Definition")]
public class UpgradeDefinition : ScriptableObject
{
    [SerializeField]
    private string uniqueId;        // 중복 없는 ID(저장/동기화 기준)

    [SerializeField]
    private string displayName;     // UI에 표시할 이름

    [SerializeField]
    private UpgradeRarity rarity;   // 희귀도(가중치 롤에 사용)

    [SerializeField]
    private int maxLevel = 5;       // 최대 레벨

    [Header("Effects (per level)")]
    [SerializeField]
    private float addDamage = 0.0f;        // 대미지 가산(%)은 별도 규칙에서 처리 가능

    [SerializeField]
    private float mulDamage = 0.0f;        // 대미지 승수(예: 0.1 = +10%)

    [SerializeField]
    private float attackSpeed = 0.0f;      // 공속 승수(0.15 = +15%)

    [SerializeField]
    private int projectileCount = 0;       // 발사체 추가 수

    [SerializeField]
    private float projectileCooldown = 0.0f;

    [SerializeField]
    private float moveSpeed = 0.0f;        // 이속 승수

    [SerializeField]
    private float maxHealth = 0.0f;        // 최대 체력 가산

    [SerializeField]
    private float regenPerSec = 0.0f;      // 초당 회복

    [SerializeField]
    private float pickupRange = 0.0f;      // 경험치 흡수 범위 가산

    [SerializeField]
    private int bulletPierceDelta = 0;  // 총알의 관통 회수.

    [SerializeField]
    private float bulletDamageDelta = 0.0f; // 총알 대미지.

    [SerializeField]
    private int bladeCountDelta = 0;    // 칼날 개수.

    [SerializeField]
    private float bladeDamageDelta = 0.0f;  // 칼날의 대미지.

    [SerializeField]
    private float bladeRotationSpeedDegDelta = 0.0f;

    [SerializeField]
    private float dashDurationSec = 0.0f;

    [SerializeField]
    private float dashDamage = 0.0f;

    [SerializeField]
    private float dashSpeed = 0.0f;

    [SerializeField]
    private float boomerangDamage = 0.0f;

    [SerializeField]
    private float boomerangMaxDistance = 0.0f;

    [SerializeField]
    private float boomerangSpeed = 0.0f;

    // ----- Getter들 -----

    public string GetId()
    {
        return uniqueId;
    }

    public string GetDisplayName()
    {
        return displayName;
    }

    public UpgradeRarity GetRarity()
    {
        return rarity;
    }

    public int GetMaxLevel()
    {
        return Mathf.Max(1, maxLevel);
    }

    public float GetAddDamage()
    {
        return addDamage;
    }

    public float GetMulDamage()
    {
        return mulDamage;
    }

    public float GetAttackSpeed()
    {
        return attackSpeed;
    }

    public int GetProjectileCount()
    {
        return projectileCount;
    }

    public float GetProjectileCooldown()
    {
        return projectileCooldown;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetRegenPerSec()
    {
        return regenPerSec;
    }

    public float GetPickupRange()
    {
        return pickupRange;
    }

    public int GetBulletPerceDelta()
    {
        return bulletPierceDelta;
    }

    public float GetBulletDamageDelta()
    {
        return bulletDamageDelta;
    }

    public int GetBladeCountDelta()
    {
        return bladeCountDelta;
    }

    public float GetBladeDamageDelta()
    {
        return bladeDamageDelta;
    }

    public float GetBladeRotationSpeedDegDelta()
    {
        return bladeRotationSpeedDegDelta;
    }

    public float GetDashDurationSec()
    {
        return dashDurationSec;
    }

    public float GetDashDamage()
    {
        return dashDamage;
    }

    public float GetDashSpeed()
    {
        return dashSpeed;
    }

    public float GetBoomerangDamage()
    {
        return boomerangDamage;
    }

    public float GetBoomerangMaxDistance()
    {
        return boomerangMaxDistance;
    }

    public float GetBoomerangSpeed()
    {
        return boomerangSpeed;
    }
}
