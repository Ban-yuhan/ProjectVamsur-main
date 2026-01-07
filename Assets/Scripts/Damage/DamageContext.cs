using UnityEngine;

/// <summary>
/// 한 번의 공격(히트)에 대한 모든 전투 파라미터를 담는 컨텍스트.
/// Router가 이를 받아 최종 대미지 계산과 상태이상 적용을 수행한다.
/// </summary>
public struct DamageContext
{
    // 기본 대미지(무기/투사체에서 설정)
    public float baseDamage;                   // 크리/방어 계산의 입력

    // 크리티컬
    public bool canCrit;                       // 크리티컬 가능 여부
    public float critChance;                   // 0.0~1.0
    public float critMultiplier;               // 예: 2.0 = 200%

    // 넉백
    public float knockbackForce;               // 임펄스 강도(0이면 미사용)

    // 상태이상(점화/빙결)
    public bool applyBurn;
    public float burnDps;
    public float burnDuration;

    public bool applyFreeze;
    public float freezeSlowPercent;            // 0.0~1.0 (예: 0.4 = 40% 감속)
    public float freezeDuration;

    // 메타
    public GameObject attacker;                // 가해자(넉백 방향/소유권 추적용)
}
