using UnityEngine;

public abstract class ZoneBase : MonoBehaviour
{
    [SerializeField]
    protected float valueToApply = 10.0f;   // 대미지/회복에 적용할 수치

    [SerializeField]
    protected float effectCooldown = 1.0f;  // 쿨타임

    [SerializeField]
    protected float effectRadius = 1.0f;    // 체크할 반경

    [SerializeField]
    protected LayerMask applyMask;  // 대상 레이어 마스크

    protected float effectTimer = 0.0f; // 타이머 체크용

    protected abstract void ApplyEffect();

    protected void Update()
    {
        if(effectTimer < effectCooldown)
        {
            effectTimer += Time.deltaTime;

            if(effectTimer >= effectCooldown)
            {
                ApplyEffect();
                effectTimer = 0.0f;
            }
        }
    }
}
