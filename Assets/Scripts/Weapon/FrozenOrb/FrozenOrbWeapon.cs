using System.Buffers;
using UnityEngine;

public class FrozenOrbWeapon : MonoBehaviour
{
    [SerializeField]
    private Transform owner; //구체의 기준이 되는 대상

    [SerializeField]
    private GameObject orbPrefab;

    [SerializeField]
    private float castCooldownSec = 2.0f;

    [SerializeField]
    private float aimRange = 8.0f;

    [SerializeField]
    private LayerMask enemyLayer;

    private float castTimer;

    private void Reset()
    {
        owner = transform;
    }


    private void Update()
    {
        castTimer += Time.deltaTime;

        if(castTimer >= castCooldownSec)
        {
            castTimer -= castCooldownSec;
            Cast();
        }
        
    }


    Vector2 FindAimDirection()
    {
        Vector2 origin = owner.position;

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, aimRange, enemyLayer);

        if (hits == null || hits.Length <= 0)
        {
            return Vector2.zero;
        }

        Transform bestTarget = null;    // 가장 가까운 적의 트랜스폼을 저장할 변수.
        float bestSqr = 0.0f;   // 가장 가까운 거리의 제곱값을 저장할 변수.

        for (int i = 0; i < hits.Length; ++i)
        {
            Collider2D c = hits[i];

            Vector2 targetPos = c.bounds.center;
            Vector2 to = targetPos - origin;

            float sqr = to.sqrMagnitude;

            if (bestTarget == null)
            {
                bestTarget = c.transform;
                bestSqr = sqr;
            }
            else
            {
                if (sqr < bestSqr)
                {
                    bestTarget = c.transform;
                    bestSqr = sqr;
                }
            }
        }

        if (bestTarget == null)
        {
            return Vector2.zero;
        }

        Vector2 dir = (Vector2)bestTarget.position - origin;
        //dir = dir.normalized;

        return dir.normalized;
    }


    void Cast()
    {
        if(orbPrefab == null)
        {
            return;
        }

        Vector2 dir = FindAimDirection();

        if(dir == Vector2.zero)
        {
            return;
        }

        Vector3 spawnPos = owner.position;

        GameObject obj = Instantiate(orbPrefab, spawnPos, Quaternion.identity);
        if(obj == null)
        {
            return;
        }

        FrozenOrbCore core = obj.GetComponent<FrozenOrbCore>();
        if(core == null)
        {
            Destroy(obj);
            return;
        }

        core.Setup(owner, dir);
    }

}
