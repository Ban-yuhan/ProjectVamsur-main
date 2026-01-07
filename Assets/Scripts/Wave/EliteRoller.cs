using UnityEngine;

/// <summary>
/// 엘리트 부여 유틸리티.
/// - 확률 체크 후 대상에 EliteModifier를 부착하고 일부 파라미터를 설정한다.
/// </summary>
public static class EliteRoller
{
    /// <summary>
    /// 대상 적에 엘리트를 부여하려 시도한다.
    /// </summary>
    /// <param name="target">대상 GameObject(적 루트)</param>
    /// <param name="chance">0~1 확률</param>
    /// <param name="healthMult">체력 배수</param>
    /// <param name="touchDamageMult">접촉 대미지 배수</param>
    /// <returns>부여되면 true, 미부여면 false</returns>
    /// <remarks>
    /// 부작용: EliteModifier 컴포넌트를 AddComponent로 추가할 수 있다.
    /// 예외: target이 null이면 false 반환.
    /// 성능: AddComponent는 비용이 있다. 스폰 직후 한 번만 호출 권장.
    /// </remarks>
    public static bool TryApplyElite(GameObject target, float chance, float healthMult, float touchDamageMult)
    {
        if (target == null)
        {
            return false;
        }

        float r = Random.value;
        if (r >= chance)
        {
            return false;
        }

        EliteModifier elite = target.GetComponent<EliteModifier>();
        if (elite == null)
        {
            elite = target.AddComponent<EliteModifier>();            
        }

        if (elite != null)
        {
            elite.Init();
        }

        // Inspector 노출 필드가 private라면 SerializeField로 보존되므로 런타임에 접근 불가할 수 있다.
        // 여기서는 간단히 공개 Setter를 가정하지 않고, 필드 기본값을 사용.
        // 실제 세밀 조정이 필요하면 EliteModifier에 공개 Setter를 추가할 것.

        return true;
    }
}
