using UnityEngine;
using System.Reflection;

/// <summary>
/// 일반 적을 엘리트로 승격한다.
/// - 체력 배수, 접촉 대미지 배수 적용
/// - 점화/빙결 면역 태그 기록, 진행 중 상태 제거 시도
/// - 시각 하이라이트 적용
/// </summary>
public class EliteModifier : MonoBehaviour
{
    [Header("Stat Multipliers")]
    [SerializeField]
    private float healthMultiplier = 3.0f;          // 최대 체력 배수

    [SerializeField]
    private float touchDamageMultiplier = 1.5f;     // 접촉 대미지 배수

    [SerializeField]
    private bool preserveCurrentHealthRatio = true; // 현재 체력 비율 유지 여부

    [Header("Immunities")]
    [SerializeField]
    private bool immuneBurn = false;                // 점화 면역

    [SerializeField]
    private bool immuneFreeze = false;              // 빙결 면역

    [Header("Visuals")]
    [SerializeField]
    private Color rimColor = new Color(1.0f, 0.6f, 0.2f); // 하이라이트 색

    [SerializeField]
    private string rimPowerProperty = "_RimPower";        // 머티리얼 속성명(있을 때만 사용)

    [SerializeField]
    private float rimPower = 2.2f;                        // 림 파워 값

    private void Start()
    {
        // 순서: 수치 → 면역/상태 → 시각
        ApplyHealthMultiplier();
        ApplyTouchDamageMultiplier();
        ApplyImmunitiesAndPurgeStatuses();
        ApplyVisualHighlight();
    }

    public void Init()
    {
        // 순서: 수치 → 면역/상태 → 시각
        ApplyHealthMultiplier();
        ApplyTouchDamageMultiplier();
        ApplyImmunitiesAndPurgeStatuses();
        ApplyVisualHighlight();
    }

    /// <summary>
    /// EnemyHealth가 있으면 최대 체력에 배수를 적용하고,
    /// 옵션에 따라 현재 체력도 비율대로 보정한다.
    /// </summary>
    private void ApplyHealthMultiplier()
    {
        EnemyHealth hp = GetComponent<EnemyHealth>();
        if (hp != null)
        {
            float prevMax = hp.GetMaxHealth();
            float prevCur = hp.GetCurrentHealth();

            if (prevMax > 0.0f)
            {
                float newMax = prevMax * healthMultiplier;
                hp.SetMaxHealth(newMax);

                if (preserveCurrentHealthRatio == true)
                {
                    float ratio = 0.0f;
                    if (prevMax > 0.0f)
                    {
                        ratio = prevCur / prevMax;
                    }

                    float newCur = Mathf.Clamp(newMax * ratio, 0.0f, newMax);
                    hp.SetCurrentHealth(newCur);
                }
            }
        }
    }

    /// <summary>
    /// DamageOnContact류 컴포넌트가 있으면 내부 대미지에 배수를 적용한다.
    /// 우선 Getter/Setter, 없으면 damage/baseDamage 필드를 탐색한다.
    /// </summary>
    private void ApplyTouchDamageMultiplier()
    {
        Component damageOnContact = GetComponentWithName("DamageOnContact");
        if (damageOnContact == null)
        {
            return;
        }

        MethodInfo setter = damageOnContact.GetType().GetMethod("SetDamage", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo getter = damageOnContact.GetType().GetMethod("GetDamage", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (setter != null && getter != null)
        {
            object curObj = getter.Invoke(damageOnContact, null);
            float cur = (curObj is float) ? (float)curObj : 0.0f;
            float after = cur * touchDamageMultiplier;
            setter.Invoke(damageOnContact, new object[] { after });
            return;
        }

        FieldInfo fld = damageOnContact.GetType().GetField("damage", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (fld == null)
        {
            fld = damageOnContact.GetType().GetField("baseDamage", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        if (fld != null)
        {
            if (fld.FieldType == typeof(float))
            {
                float cur = (float)fld.GetValue(damageOnContact);
                float after = cur * touchDamageMultiplier;
                fld.SetValue(damageOnContact, after);
                return;
            }
        }

        FieldInfo[] all = damageOnContact.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        for (int i = 0; i < all.Length; i = i + 1)
        {
            FieldInfo fi = all[i];
            if (fi.FieldType == typeof(float))
            {
                string name = fi.Name.ToLower();
                if (name.Contains("damage") == true)
                {
                    float cur = (float)fi.GetValue(damageOnContact);
                    float after = cur * touchDamageMultiplier;
                    fi.SetValue(damageOnContact, after);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// 면역 태그를 기록하고, StatusEffectHost가 있으면 진행 중인 점화/빙결을 제거한다.
    /// </summary>
    private void ApplyImmunitiesAndPurgeStatuses()
    {
        StatusImmunityTag tag = GetComponent<StatusImmunityTag>();
        if (tag == null)
        {
            tag = gameObject.AddComponent<StatusImmunityTag>();
        }
        tag.SetImmuneBurn(immuneBurn);
        tag.SetImmuneFreeze(immuneFreeze);

        Component statusHost = GetComponentWithName("StatusEffectHost");
        if (statusHost != null)
        {
            TrySetFloatField(statusHost, "burnDps", 0.0f);
            TrySetFloatField(statusHost, "burnRemain", 0.0f);
            TrySetFloatField(statusHost, "freezeRemain", 0.0f);

            MethodInfo clearSlow = statusHost.GetType().GetMethod("ClearSlow", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (clearSlow != null)
            {
                clearSlow.Invoke(statusHost, null);
            }
        }
    }

    /// <summary>
    /// 시각 하이라이트(색상 틴트, 머티리얼 림 파워)를 적용한다.
    /// </summary>
    public void ApplyVisualHighlight()
    {
        //SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = rimColor;

            Material mat = sr.material;
            if (mat != null)
            {
                if (string.IsNullOrEmpty(rimPowerProperty) == false)
                {
                    if (mat.HasProperty(rimPowerProperty) == true)
                    {
                        mat.SetFloat(rimPowerProperty, rimPower);
                    }
                }
            }
        }
    }

    // ---------- Helper ----------

    private Component GetComponentWithName(string typeName)
    {
        Component[] all = GetComponents<Component>();
        for (int i = 0; i < all.Length; i = i + 1)
        {
            Component c = all[i];
            if (c != null)
            {
                if (c.GetType().Name == typeName)
                {
                    return c;
                }
            }
        }
        return null;
    }

    private void TrySetFloatField(object target, string fieldName, float value)
    {
        if (target == null)
        {
            return;
        }

        FieldInfo fi = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (fi != null)
        {
            if (fi.FieldType == typeof(float))
            {
                fi.SetValue(target, value);
            }
        }
    }
}