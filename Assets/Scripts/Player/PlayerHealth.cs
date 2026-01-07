using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable, IRecover
{
    [SerializeField]
    private float maxHealth = 100.0f;

    [SerializeField]
    private float currentHealth;

    [SerializeField]
    private float invulnerableTime = 0.8f;

    private float invulnerableTimer;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Color normalColor = Color.white;

    [SerializeField]
    private Color hitColor = Color.red;

    [SerializeField]
    private Rigidbody2D body;

    [SerializeField]
    private float knockbackDamper = 0.9f;

    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private AnimationController animController;

    [SerializeField]
    private PlayerDash playerDash;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(invulnerableTimer > 0.0f)
        {
            invulnerableTimer -= Time.deltaTime;
            if(invulnerableTimer <= 0.0f)
            {
                EndInvulnerability();
            }
        }

        if(Input.GetKeyDown(KeyCode.F) == true)
        {
            ApplyDamage(5.0f);
        }
    }

    public void ApplyDamage(float amount)
    {
        if(gameManager != null)
        {
            if(gameManager.IsPlaying() == false)
            {
                return;
            }
        }

        if(currentHealth <= 0.0f)
        {
            return;
        }

        if(amount <= 0.0f)
        {
            return;
        }

        if(invulnerableTimer > 0.0f)
        {
            return;
        }

        if(playerDash != null && playerDash.IsDashing() == true)
        {
            return;
        }

        currentHealth -= amount;

        PlayHitFeedback();

        BeginInvulnerability();

        if(currentHealth <= 0.0f)
        {
            currentHealth = 0.0f;
            HandleDeath();
        }
    }

    public void ApplyKnockback(Vector2 force)
    {
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if(movement != null)
        {
            movement.AddKnockback(force);
        }
        else
        {
            if (body != null)
            {
                //body.linearVelocity = body.linearVelocity * knockbackDamper;
                body.AddForce(force, ForceMode2D.Impulse);
            }
        }
    }

    void HandleDeath()
    {
        if(animController != null)
        {
            animController.PlayDeath();
        }

        if(gameManager != null)
        {
            //gameManager.HandleGameClear();
            gameManager.HandleGameOver();
        }
    }

    void BeginInvulnerability()
    {
        invulnerableTimer = invulnerableTime;
        PlayHitFeedback();
    }

    void EndInvulnerability()
    {
        invulnerableTimer = 0.0f;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
    }

    void PlayHitFeedback()
    {
        if(spriteRenderer != null)
        {
            spriteRenderer.color = hitColor;
        }
    }

    public void SetMaxHealth(float value)
    {
        maxHealth = value;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void Heal(float value)
    {
        currentHealth = Mathf.Min(currentHealth + value, maxHealth);
    }
}
