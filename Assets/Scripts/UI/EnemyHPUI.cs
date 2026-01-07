using UnityEngine;
using UnityEngine.UI;

public class EnemyHPUI : MonoBehaviour
{
    [SerializeField]
    private EnemyHealth enemyHealth;

    [SerializeField]
    private Image imageHP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(enemyHealth != null)
        {
            HandleChangedHP(enemyHealth.GetCurrentHealth(), enemyHealth.GetMaxHealth());
        }
    }

    private void OnEnable()
    {
        if(enemyHealth != null)
        {
            enemyHealth.OnChangedHP += HandleChangedHP;
        }
    }

    private void OnDisable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnChangedHP -= HandleChangedHP;
        }
    }

    void HandleChangedHP(float current, float max)
    {
        if(current <= 0.0f)
        {
            imageHP.fillAmount = 0.0f;
            return;
        }

        imageHP.fillAmount = current / max;
    }
}
