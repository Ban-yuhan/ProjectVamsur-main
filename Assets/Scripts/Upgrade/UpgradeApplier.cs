using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

/// <summary>
/// 업그레이드 적용 유틸. 한 레벨의 효과를 실제 시스템에 반영한다.
/// 모든 적용은 이곳을 경유해 일원화하여, 밸런스/디버깅을 단순화한다.
/// </summary>
public static class UpgradeApplier
{
    /// <summary>
    /// 업그레이드 정의의 한 레벨 효과를 적용한다.
    /// </summary>
    /// <param name="player">플레이어 루트</param>
    /// <param name="def">업그레이드 정의</param>
    /// <remarks>
    /// 부작용: 플레이어 스탯/무기 컴포넌트가 수정될 수 있다.
    /// 예외: player 또는 def가 null이면 아무 것도 하지 않는다.
    /// 성능: O(1).
    /// </remarks>
    public static void ApplyOneLevel(GameObject player, UpgradeDefinition def)
    {
        if (player == null)
        {
            return;
        }

        if (def == null)
        {
            return;
        }

        // 예시:
        WeaponManager weapon = player.GetComponent<WeaponManager>();

        if (weapon != null)
        {
            if(def.GetProjectileCooldown() > 0.0f)
            {
                weapon.ReduceCooldownAll(def.GetProjectileCooldown());
            }

            if (def.GetAttackSpeed() != 0.0f)
            {
                weapon.AddProjectileSpeedAll(def.GetAttackSpeed());
            }

            if (def.GetAddDamage() != 0.0f)
            {
                weapon.AddDamageAll(def.GetAddDamage());
            }

            if (def.GetProjectileCount() != 0)
            {
                weapon.AddProjectileCountAll(def.GetProjectileCount());
            }
        }

        PlayerWeaponStatsRuntime stats = GameObject.FindAnyObjectByType<PlayerWeaponStatsRuntime>();

        if(stats != null)
        {
            int bulletPierceDelta = def.GetBulletPerceDelta();
            float bulletDamageDelta = def.GetBulletDamageDelta();
            stats.AddPierceBulletUpgrade(bulletPierceDelta, bulletDamageDelta);

            int bladeCountDelta = def.GetBladeCountDelta();
            float bladeDamageDelta = def.GetBladeDamageDelta();
            float bladeRotationSpeedDelta = def.GetBladeRotationSpeedDegDelta();
            stats.AddOrbitBladesUpgrade(bladeCountDelta, bladeDamageDelta, bladeRotationSpeedDelta);

            float dashDurationSec = def.GetDashDurationSec();
            float dashDamage = def.GetDashDamage();
            float dashSpeed = def.GetDashSpeed();
            stats.AddDashUpgrade(dashDurationSec, dashDamage, dashSpeed);

            float boomerangDamage = def.GetBoomerangDamage();
            float boomerangMaxDistance = def.GetBoomerangMaxDistance();
            float boomerangSpeed = def.GetBoomerangSpeed();
            stats.AddBoomerangUpgrade(boomerangDamage, boomerangMaxDistance, boomerangSpeed);
        }
    }
}
