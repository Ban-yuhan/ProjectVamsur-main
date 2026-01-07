using UnityEngine;

/// <summary>
/// 대상의 상태이상 면역 정보를 보관하는 태그.
/// Host/Router가 이 값을 참조해 적용 여부를 판단한다.
/// </summary>
public class StatusImmunityTag : MonoBehaviour
{
    [SerializeField]
    private bool immuneBurn;

    [SerializeField]
    private bool immuneFreeze;

    public bool IsImmuneBurn()
    {
        return immuneBurn;
    }

    public bool IsImmuneFreeze()
    {
        return immuneFreeze;
    }

    public void SetImmuneBurn(bool value)
    {
        immuneBurn = value;
    }

    public void SetImmuneFreeze(bool value)
    {
        immuneFreeze = value;
    }
}