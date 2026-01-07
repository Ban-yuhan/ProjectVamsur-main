using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable, IRecover, IReceivesHitContext
{
    public float maxHealth = 100.0f;
    public SpriteRenderer spriteRenderer;
    public float hitFlashTime = 0.05f;

    public DropOnDeath drop;

    [SerializeField]
    private float currentHealth;
    private float hitFlashTimer;

    private Color originalColor;

    private WaveDirector waveDirector;

    private bool hasLastContext; // 최근 문맥 보유 여부
    private HitContext lastContext; // 최근 문맥 저장
    
    [SerializeField, Range(0.0f, 0.9f)]
    private float resistance = 0.1f; // 피해 저항

    [SerializeField]
    private AnimationController animController;

    public event Action<float, float> OnChangedHP;

    private void Awake()
    {
        currentHealth = maxHealth;

        if(spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        waveDirector = GameObject.FindAnyObjectByType<WaveDirector>();

        hasLastContext = false; // 문맥 보유 초기화
    }

    // Update is called once per frame
    void Update()
    {
        //if(hitFlashTimer > 0.0f)
        if(hitFlashTimer < hitFlashTime)
        {
            //hitFlashTimer -= Time.deltaTime;
            hitFlashTimer += Time.deltaTime;

            //if(hitFlashTimer <= 0.0f)
            if(hitFlashTimer >= hitFlashTime)
            {
                RestoreColor();
            }
        }
    }

    public void ApplyDamage(float amount)
    {
        if(currentHealth <= 0.0f)
        {
            return;
        }

        HitContext context; // 사용할 문맥
        if (hasLastContext == true)
        {
            context = lastContext; // 최근 문맥 사용
            hasLastContext = false; // 사용 후 해제
        }
        else
        {
            context = HitContext.Create(null, transform, transform.position,
                amount, DamageType.Physical); // 기본 문맥 합성
        }

        float finalDamage = DamageResolver.ComputeFinalDamage(context, resistance, amount); // 최종 피해 계산
        
        currentHealth -= finalDamage;

        PlayHitFlash();

        DamageAppliedEvent e = new DamageAppliedEvent(); // 피해확정 알림 데이터
        e.context = context; // 문맥
        e.finalDamage = finalDamage; // 최종 피해
        e.remainingHp = currentHealth; // 남은 체력
        EventBus.PublishDamageApplied(e); // 피해확정 알림 발행

        if(OnChangedHP != null)
        {
            OnChangedHP.Invoke(currentHealth, maxHealth);
        }

        if (currentHealth <= 0.0f)
        {
            Die();
        }
    }

    void Die()
    {
        if(drop != null)
        {
            drop.SpawnDrops();
        }

        if (waveDirector != null)
        {
            waveDirector.ReportKill();
        }

        DeathEvent deathEvent = new DeathEvent();
        deathEvent.victim = transform;
        EventBus.PublishDeath(deathEvent);

        if(animController != null)
        {
            animController.PlayDeath();
        }

        Destroy(gameObject, 2.0f);
    }

    void PlayHitFlash()
    {
        if(spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            //hitFlashTimer = hitFlashTime;
            hitFlashTimer = 0.0f;
        }
    }

    void RestoreColor()
    {
        if(spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    public void SetMaxHealth(float value)
    {
        maxHealth = value;
        currentHealth = maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SetCurrentHealth(float newHealth)
    {
        currentHealth = newHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void Heal(float value)
    {
        currentHealth = Mathf.Min(currentHealth + value, maxHealth);
    }

    /// <summary>
    /// 다음 피해 적용에서 사용할 히트 문맥을 저장한다
    /// </summary>
    public void SetHitContext(HitContext context)
    {
        lastContext = context; // 문맥 저장
        hasLastContext = true; // 보유 표시
    }
}
