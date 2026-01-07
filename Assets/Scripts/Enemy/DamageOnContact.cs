using UnityEngine;

public class DamageOnContact : MonoBehaviour
{
    [SerializeField]
    private float contactDamage = 5.0f;

    [SerializeField]
    private float damageInterval = 0.8f;

    [SerializeField]
    private float knockbackForce = 4.0f;

    private float damageCooldown;

    // Update is called once per frame
    void Update()
    {
        if(damageCooldown > 0.0f)
        {
            damageCooldown -= Time.deltaTime;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDamage(collision.collider);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryDamage(collision);
    }

    void TryDamage(Collider2D col)
    {
        if(col == null)
        {
            return;
        }

        if(damageCooldown > 0.0f)
        {
            return;
        }

        if(col.CompareTag("Player") == false)
        {
            return;
        }

        //IDamageable damageable = col.GetComponent<IDamageable>();
        PlayerHealth player = col.GetComponent<PlayerHealth>();
        //if(damageable != null)
        if(player != null)
        {
            //damageable.ApplyDamage(contactDamage);
            player.ApplyDamage(contactDamage);
            //PlayerHealth player = col.GetComponent<PlayerHealth>();

            Vector2 dir = (col.transform.position - transform.position).normalized;
            Vector2 force = dir * knockbackForce;
            player.ApplyKnockback(force);

            damageCooldown = damageInterval;
        }
    }
}
