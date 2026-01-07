using UnityEngine;

/// <summary>
/// 간단한 피해 종류 구분
/// </summary>
public enum DamageType
{
    Physical = 0,
    Piercing = 1
}

/// <summary>
/// 히트 순간의 정보 묶음
/// </summary>
public struct HitContext
{
    public Transform attacker;   // 가해자 변환
    public Transform target;     // 피격자 변환
    public Vector3 hitPosition;  // 피격 위치
    public float baseDamage;     // 기본 피해량
    public DamageType damageType; // 피해 종류

    public static HitContext Create(Transform attacker, Transform target, Vector3 pos, float dmg, DamageType type)
    {
        HitContext c = new HitContext();
        c.attacker = attacker; // 가해자 변환
        c.target = target; // 피격자 변환
        c.hitPosition = pos; // 피격 위치
        c.baseDamage = dmg; // 기본 피해량
        c.damageType = type; // 피해 종류
        return c;
    }
}