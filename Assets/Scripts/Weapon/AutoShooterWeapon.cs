using System.Collections.Generic;
using UnityEngine;

public class AutoShooterWeapon : WeaponBase
{
    [SerializeField]
    private float targetSearchRadius = 8.0f;

    [SerializeField]
    private LayerMask targetLayerMask;

    [SerializeField]
    private float spreadDegrees = 10.0f;

    private float rotatingAngle;

    protected override void Awake()
    {
        base.Awake();
        rotatingAngle = 0.0f;
    }

    protected override void AcquireFireDirections(out List<Vector2> directions)
    {
        directions = new List<Vector2>();

        Vector2 aim = GetAimDirection();
        if(aim.sqrMagnitude <= 0.0001f)
        {
            aim = Vector2.right;
        }

        int count = GetProjectileCount();
        float baseAngle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg;
        if(count <= 1)
        {
            directions.Add(aim.normalized);
            return;
        }

        float start = baseAngle - spreadDegrees * (count - 1) * 0.5f;

        for(int i=0; i<count; ++i)
        {
            float angle = start + spreadDegrees * i;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            directions.Add(dir.normalized);
        }
    }

    Vector2 GetAimDirection()
    {
        Transform nearest = FindNearestTarget();
        if (nearest != null)
        {
            Vector2 toTarget = nearest.position - transform.position;
            return toTarget.normalized;
        }

        rotatingAngle -= 90.0f * Time.deltaTime;
        float rad = rotatingAngle * Mathf.Deg2Rad;  // 각도를 라디안 값으로 변경.
        Vector2 fallback = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        return fallback.normalized;
    }

    Transform FindNearestTarget()
    {
        Collider2D[] hits;

        hits = Physics2D.OverlapCircleAll(transform.position, targetSearchRadius, targetLayerMask);
        if (hits == null || hits.Length == 0)
        {
            return null;
        }

        float bestDist = float.MaxValue;
        Transform best = null;

        for (int i = 0; i < hits.Length; ++i)
        {
            Transform t = hits[i].transform;
            //float dist = (t.position - transform.position).sqrMagnitude; 
            float dist = Vector2.Distance(t.position, transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = t;
            }
        }

        return best;
    }
}
