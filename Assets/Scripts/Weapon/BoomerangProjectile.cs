using UnityEngine;
using System.Collections.Generic;

public class BoomerangProjectile : MonoBehaviour
{
    [SerializeField]
    private Collider2D hitCollider;

    [SerializeField]
    private float returnFinishDistance = 0.6f;  // 돌아왔다고 판단할 거리.

    private Transform owner;    // 돌아갈 대상의 트랜스폼.
    private Vector2 outDirection;   // 처음 나갈 방향.
    private float moveSpeed;
    private float damage;
    private float maxDistance;
    private float hitCooldownSec;
    private LayerMask enemyLayer;

    private Vector2 startPos;
    private bool returning;

    private Dictionary<Transform, float> lastHitTimeByTarget = new Dictionary<Transform, float>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        lastHitTimeByTarget.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if(returning == false)
        {
            UpdateOutgoing();
        }
        else
        {
            UpdateReturning();
        }
    }

    private bool CanApplyHit(float now, Transform target)
    {
        if(lastHitTimeByTarget.TryGetValue(target, out float lastTime) == true)
        {
            float delta = now - lastTime;
            if(delta < hitCooldownSec)
            {
                return false;
            }
        }

        return true;
    }

    void TryHit(Collider2D collision)
    {
        int mask = 1 << collision.gameObject.layer;
        if((mask & enemyLayer.value) == 0)
        {
            return;
        }

        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        if(enemyHealth == null)
        {
            return;
        }

        if(CanApplyHit(Time.time, enemyHealth.transform) == false)
        {
            return;
        }

        lastHitTimeByTarget[enemyHealth.transform] = Time.time;
        enemyHealth.ApplyDamage(damage);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryHit(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryHit(collision);
    }

    void UpdateReturning()
    {
        Vector2 to = owner.position - transform.position;

        float distSqr = to.sqrMagnitude;
        float finishSqr = returnFinishDistance * returnFinishDistance;
        //float distSqr = to.magnitude;
        //float finishSqr = returnFinishDistance;

        if(distSqr <= finishSqr)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 dir = to.normalized;
        Vector2 delta = dir * moveSpeed * Time.deltaTime;
        transform.position += (Vector3)delta;
    }

    void UpdateOutgoing()
    {
        Vector2 delta = outDirection * moveSpeed * Time.deltaTime;
        transform.position += (Vector3)delta;

        Vector2 diff = (Vector2)transform.position - startPos;

        float travelSqr = diff.sqrMagnitude;
        float maxSqr = maxDistance * maxDistance;

        if(travelSqr >= maxSqr)
        {
            returning = true;
        }
    }

    public void Setup(Transform owner, Vector2 direction, float speed, float dmg, float distance, float cooldownSec, LayerMask layer)
    {
        this.owner = owner;

        outDirection = direction.normalized;
        moveSpeed = speed;
        damage = dmg;
        maxDistance = distance;
        hitCooldownSec = cooldownSec;
        enemyLayer = layer;

        startPos = transform.position;
        returning = false;

        lastHitTimeByTarget.Clear();
    }
}
