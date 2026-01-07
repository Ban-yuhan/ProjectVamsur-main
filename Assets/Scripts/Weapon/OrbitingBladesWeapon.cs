using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 플레이어 주변에 칼날 여러개를 만들고 원형으로 회전시킨다.
/// 칼날이 적과 닿으면 대미지를 준다.
/// 너무 자주 대미지를 주지 않기 위해 적별로 쿨다운을 적용.
/// </summary>
public class OrbitingBladesWeapon : MonoBehaviour
{
    [SerializeField] private PlayerWeaponStatsRuntime stats;

    [SerializeField] private GameObject bladePrefab;

    [SerializeField] private int bladeCount = 5;    // 생성할 칼날 개수.
    [SerializeField] private float radius = 2.0f;   // 칼날을 생성할 플레이어 주위 반경.
    [SerializeField] private float rotationSpeedDeg = 180.0f;   // 초당 회전 속도.(각도 단위)

    [SerializeField] private float damage = 2.0f;   // 대미지 값.
    [SerializeField] private float hitCooldownSec = 0.2f;    // 쿨다운.

    private List<Transform> blades = new List<Transform>(); // 생성한 칼날의 트랜스폼 정보를 담을 리스트.
    private Dictionary<Transform, float> lastHitTimeByTargetId = new Dictionary<Transform, float>();   // 적별로 마지막으로 명중한 시각.

    private void Awake()
    {
        blades.Clear();
        lastHitTimeByTargetId.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if(stats == null)
        {
            return;
        }

        int targetCount = stats.GetBladeCount();

        EnsureBladeCount(targetCount);

        if(blades.Count <= 0)
        {
            return;
        }

        rotationSpeedDeg = stats.GetBladeRotationSpeedDeg();
        radius = stats.GetBladeRadius();

        float baseAngle = Time.time * rotationSpeedDeg;
        float step = 360.0f / blades.Count;

        for(int i=0; i<blades.Count; ++i)
        {
            // 칼날의 시작 각도를 순서대로 구한다.
            float angleDeg = baseAngle + (step * i);

            // 구한 각도값을 라디안 값으로 변환한다.
            float angleRad = angleDeg * Mathf.Deg2Rad;

            // 칼날의 x, y 위치를 계산한다.
            float x = Mathf.Cos(angleRad) * radius;
            float y = Mathf.Sin(angleRad) * radius;

            Vector3 offset = new Vector3(x, y, 0.0f);
            blades[i].position = transform.position + offset;
        }
    }

    public void TryDealDamage(EnemyHealth enemyHealth)
    {
        if(stats == null)
        {
            return;
        }

        hitCooldownSec = stats.GetBladeHitCooldownSec();

        Transform trans = enemyHealth.transform;
        float now = Time.time;  // 현재 시간을 저장.
        float lastTime;

        bool hasTime = lastHitTimeByTargetId.TryGetValue(trans, out lastTime);
        if(hasTime == true)
        {
            if(now - lastTime < hitCooldownSec)
            {
                return;
            }
        }

        lastHitTimeByTargetId[trans] = now;

        damage = stats.GetBladeDamage();
        enemyHealth.ApplyDamage(damage);
    }

    /// <summary>
    /// 칼날의 개수를 목표 개수로 맞춘다.
    /// 부족하면 추가 생성하고, 많으면 제거한다.
    /// </summary>
    /// <param name="targetCount"></param>
    void EnsureBladeCount(int targetCount)
    {
        if(targetCount == blades.Count)
        {
            return;
        }

        if(targetCount < blades.Count)
        {
            for(int i=blades.Count-1; i>=targetCount; --i)
            {
                Transform t = blades[i];
                blades.RemoveAt(i);

                if(t != null)
                {
                    Destroy(t.gameObject);
                }
            }

            return;
        }

        int need = targetCount - blades.Count;
        for (int i = 0; i < need; ++i)
        {
            GameObject blade = Instantiate(bladePrefab, transform.position, Quaternion.identity);
            if (blade != null)
            {
                blade.transform.SetParent(transform, true);

                BladeHitReceiver receiver = blade.GetComponent<BladeHitReceiver>();
                if (receiver != null)
                {
                    receiver.Setup(this);
                }

                blades.Add(blade.transform);
            }
        }
    }
}
