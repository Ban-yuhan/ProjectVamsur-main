using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float lifeTime = 3.0f;
    public float damage = 5.0f;

    private Vector2 moveDirection;

    private float elapsed;

    public bool canCrit;
    public float critChance;
    public float critMultiplier = 2.0f;
    public float knockbackForce;
    public bool applyBurn;
    public float burnDps;
    public float burnDuration;
    public bool applyFreeze;
    public float freezeSlowPercent;
    public float freezeDuration;
    public GameObject owner;

    public DamageType damageType = DamageType.Physical; // 피해 종류

    public string targetTag = "Enemy";

    [SerializeField]
    private int pierceCount = 0;

    private PooledObject pooled;

    private int remainPierce;   // 현재 남아있는 관통 카운트.

    [SerializeField]
    private PlayerWeaponStatsRuntime stats; // 무기 강화 수치를 읽기 위한 참조 변수.

    private Dictionary<Transform, float> lastTimeByTargetId;

    private void Awake()
    {
        pooled = GetComponent<PooledObject>();
    }

    private void Start()
    {
        stats = GameObject.FindAnyObjectByType<PlayerWeaponStatsRuntime>();
    }

    private void OnEnable()
    {
        remainPierce = pierceCount;

        if(lastTimeByTargetId == null)
        {
            lastTimeByTargetId = new Dictionary<Transform, float>();
        }
        else
        {
            lastTimeByTargetId.Clear();
        }

        if(stats != null)
        {
            remainPierce = stats.GetBulletPierceCount();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
        transform.position += delta;
    }

    public void SetDirection(Vector2 dir)
    {
        if(dir.sqrMagnitude > 0.0001f)
        {
            moveDirection = dir.normalized;
        }
        else
        {
            moveDirection = Vector2.right;
        }
    }

    public void Configure(float speed, float time, float dmg)
    {
        moveSpeed = speed;
        lifeTime = time;
        damage = dmg;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(targetTag) == false)
        {
            return;
        }

        if(stats == null)
        {
            return;
        }

        float damage = stats.GetBulletDamage();

        Transform attacker = transform; // 가해자 변환
        Transform target = collision.transform; // 피격자 변환
        Vector3 pos = collision.bounds.center; // 피격 위치

        HitContext context = HitContext.Create(attacker, target, pos, damage, damageType); // 히트 문맥 생성

        EventBus.PublishHit(context); // 닿음 알림 발행

        IReceivesHitContext ctxReceiver = collision.GetComponent<IReceivesHitContext>(); // 문맥 수신 가능 여부
        if (ctxReceiver != null)
        {
            ctxReceiver.SetHitContext(context); // 문맥 저장 요청
        }

        EnemyHealth damageable = collision.GetComponent<EnemyHealth>();
        if(damageable == null)
        {
            return;
        }

        //if(damageable != null)
        //{
        //    damageable.ApplyDamage(damage);
        //}

        Transform trans = damageable.transform;
        float now = Time.time;

        float cooldown = stats.GetBulletHitCooldownSec();
        
        if(lastTimeByTargetId.TryGetValue(trans, out float lastTime) == true)
        {
            float delta = now - lastTime;
            if(delta < cooldown)
            {
                return;
            }
        }

        lastTimeByTargetId[trans] = now;
        damageable.ApplyDamage(damage);

        --remainPierce;

        if (remainPierce <= 0)
        {
            if (pooled != null)
            {
                pooled.Release();
                return;
            }

            Destroy(gameObject);
        }
    }

    public void SetOwner(GameObject obj)
    {
        owner = obj;
    }

    public void SetCrit(bool enable, float chance, float multiplier)
    {
        canCrit = enable;
        critChance = Mathf.Clamp01(chance);
        critMultiplier = Mathf.Max(1.0f, multiplier);
    }

    public void SetKnockback(float force)
    {
        knockbackForce = Mathf.Max(0.0f, force);
    }

    public void SetBurn(float dps, float duration)
    {
        if (dps > 0.0f && duration > 0.0f)
        {
            applyBurn = true;
            burnDps = dps;
            burnDuration = duration;
        }
    }

    public void SetFreeze(float slowPercent, float duration)
    {
        if (slowPercent > 0.0f && duration > 0.0f)
        {
            applyFreeze = true;
            freezeSlowPercent = Mathf.Clamp01(slowPercent);
            freezeDuration = duration;
        }
    }

    /// <summary>
    /// 대미지를 반환
    /// </summary>
    public float GetDamage()
    {
        return damage;
    }

    /// <summary>
    /// 속도를 세팅
    /// </summary>
    public void SetSpeed(float v)
    {
        moveSpeed = v;
    }
}
