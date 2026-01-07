using UnityEngine;

/// <summary>
/// 대상(플레이어/적)의 방어/저항 수치를 보관.
/// </summary>
public class DamageableStats : MonoBehaviour
{
    [SerializeField]
    private float defense = 0.0f;                 // 방어력(완만 감쇄식 입력 값)

    [SerializeField]
    private float reductionCap = 0.8f;            // 최대 감쇄율(0~1)

    // 상태저항(오늘은 사용 X, 추후 확장용)
    [SerializeField]
    private float burnResist = 0.0f;              // 0~1
    [SerializeField]
    private float freezeResist = 0.0f;            // 0~1

    public float GetDefense()
    {
        return defense;
    }

    public void SetDefense(float value)
    {
        if (value >= 0.0f)
        {
            defense = value;
        }
    }

    public float GetReductionCap()
    {
        return reductionCap;
    }

    public void SetReductionCap(float value)
    {
        reductionCap = Mathf.Clamp01(value);
    }

    public float GetBurnResist()
    {
        return Mathf.Clamp01(burnResist);
    }

    public float GetFreezeResist()
    {
        return Mathf.Clamp01(freezeResist);
    }
}
