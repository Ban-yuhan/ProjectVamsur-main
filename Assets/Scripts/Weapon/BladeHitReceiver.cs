using UnityEngine;

public class BladeHitReceiver : MonoBehaviour
{
    private OrbitingBladesWeapon weapon;

    public void Setup(OrbitingBladesWeapon owner)
    {
        weapon = owner;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            if(weapon != null)
            {
                weapon.TryDealDamage(enemyHealth);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            if (weapon != null)
            {
                weapon.TryDealDamage(enemyHealth);
            }
        }
    }
}
