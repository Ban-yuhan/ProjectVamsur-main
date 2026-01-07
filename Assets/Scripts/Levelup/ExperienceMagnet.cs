using UnityEngine;

public class ExperienceMagnet : MonoBehaviour
{
    [SerializeField]
    private float magnetRadius = 4.0f;

    [SerializeField]
    private LayerMask orbLayerMask;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        bool canWork = false;
        if(gameManager != null)
        {
            canWork = gameManager.IsPlaying();
        }

        if(canWork == false)
        {
            return;
        }

        Collider2D[] buffer = Physics2D.OverlapCircleAll(transform.position, magnetRadius, orbLayerMask);
        for(int i=0; i<buffer.Length; ++i)
        {
            if (buffer[i] != null)
            {
                ExpPickup orb = buffer[i].GetComponent<ExpPickup>();
                if(orb != null)
                {
                    orb.BeginMagnetize(transform);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, magnetRadius);
    }
}
