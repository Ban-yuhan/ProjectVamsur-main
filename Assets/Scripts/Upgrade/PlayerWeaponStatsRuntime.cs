using UnityEngine;

/// <summary>
/// 무기의 강화 수치를 한 곳에 모아 관리한다.
/// 업그레이드 카드에서의 선택 결과는 이 스크립트의 함수를 호출해서 들어오게 한다.
/// 무기 스크립트는 이 값을 읽어서 동작한다.
/// </summary>
public class PlayerWeaponStatsRuntime : MonoBehaviour
{
    [SerializeField] private int bulletPierceCount = 0; // 총알이 추가로 관통할 수 있는 회수.
    [SerializeField] private float bulletDamage = 1.0f; // 총알 대미지.
    [SerializeField] private float bulletHitCooldownSec = 0.15f;    // 대미지를 주는 쿨타임.

    [SerializeField] private int bladeCount = 3;    // 주변을 도는 칼날의 개수.
    [SerializeField] private float bladeRadius = 2.0f;  // 칼날의 반경.
    [SerializeField] private float bladeRotationSpeedDeg = 180.0f;  // 칼날의 초당 회전 각도.
    [SerializeField] private float bladeDamage = 1.0f;  // 칼날의 대미지.
    [SerializeField] private float bladeHitCooldownSec = 0.2f;  // 대미지를 주는 쿨타임.

    [SerializeField] private float dashDurationSec = 0.2f;  // 대쉬 지속시간.
    [SerializeField] private float dashDamage = 5.0f;   // 대쉬 대미지.
    [SerializeField] private float dashSpeed = 15.0f;   // 대쉬 속도.

    [SerializeField] private float boomerangDamage = 5.0f;  // 부메랑 대미지.
    [SerializeField] private float boomerangMaxDistance = 6.0f; // 부메랑 사정거리.
    [SerializeField] private float boomerangSpeed = 10.0f;  // 부메랑 속도.

    public int GetBulletPierceCount()
    {
        return bulletPierceCount;
    }

    public float GetBulletDamage()
    {
        return bulletDamage;
    }

    public float GetBulletHitCooldownSec()
    {
        return bulletHitCooldownSec;
    }

    public int GetBladeCount()
    {
        return bladeCount;
    }

    public float GetBladeRadius()
    {
        return bladeRadius;
    }

    public float GetBladeRotationSpeedDeg()
    {
        return bladeRotationSpeedDeg;
    }

    public float GetBladeDamage()
    {
        return bladeDamage;
    }

    public float GetBladeHitCooldownSec()
    {
        return bladeHitCooldownSec;
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

    /// <summary>
    /// 관통 회수와 대미지를 더한다.
    /// </summary>
    /// <param name="pierceDelta"></param>
    /// <param name="damageDelta"></param>
    public void AddPierceBulletUpgrade(int pierceDelta, float damageDelta)
    {
        bulletPierceCount += pierceDelta;
        bulletDamage += damageDelta;
    }

    /// <summary>
    /// 회전 칼날의 개수, 대미지, 회전 속도 값을 더한다.
    /// </summary>
    /// <param name="countDelta"></param>
    /// <param name="damageDelta"></param>
    /// <param name="rotationSpeedDelta"></param>
    public void AddOrbitBladesUpgrade(int countDelta, float damageDelta, float rotationSpeedDelta)
    {
        bladeCount += countDelta;
        bladeDamage += damageDelta;
        bladeRotationSpeedDeg += rotationSpeedDelta;
    }

    public void AddDashUpgrade(float durationSec, float damage, float speed)
    {
        dashDurationSec += durationSec;
        dashDamage += damage;
        dashSpeed += speed;
    }

    public void AddBoomerangUpgrade(float damage, float maxDistance, float speed)
    {
        boomerangDamage += damage;
        boomerangMaxDistance += maxDistance;
        boomerangSpeed += speed;
    }
}
