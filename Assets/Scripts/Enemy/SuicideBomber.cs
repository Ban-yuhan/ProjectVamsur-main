using UnityEngine;

/// <summary>
/// 플레이어에게 접근해 일정 반경에서 퓨즈 시동 후 폭발하는 적 클래스.
/// 이동 -> 퓨즈 시동 -> 폭발 -> 범위 대미지 적용
/// </summary>
public class SuicideBomber : MonoBehaviour
{
    [SerializeField]
    private Transform player;

    [SerializeField]
    private BomberWarningPulse warning;

    [SerializeField]
    private float moveSpeed = 3.0f;

    [SerializeField]
    private float detectDistance = 10.0f;

    [SerializeField]
    private float fuseRadius = 2.5f;

    [SerializeField]
    private float fuseSeconds = 0.8f;

    [SerializeField]
    private float explodeRadius = 3.0f;

    [SerializeField]
    private float explosionDamage = 20.0f;

    [SerializeField]
    private LayerMask targetLayer;

    [SerializeField]
    private bool instantDetonateOnTrigger = true;   // 접촉 시 즉시 폭발할지 여부.

    [SerializeField]
    private LayerMask instantLayer; // 접촉 대상 레이어.

    [SerializeField]
    private bool chainAffectBomber = true;  // 주변의 자폭형 적들에게 영향을 줄 지 여부.

    [SerializeField]
    private bool chainInstant = false;

    [SerializeField]
    private float chainFuseSeconds = 0.4f;

    [SerializeField]
    private AnimationController animController;

    [SerializeField]
    private SpriteRenderer sr;

    [SerializeField]
    private EnemyHealth enemyHealth;

    private bool fuseArmed; //  퓨즈 시동 중인지 여부.
    private float fuseRemain;   // 퓨즈 타이머 변수.
    private bool exploded;  // 폭발 했는지 여부.

    private void Awake()
    {
        fuseArmed = false;
        fuseRemain = 0.0f;
        exploded = false;

        if(warning == null)
        {
            warning = GetComponentInChildren<BomberWarningPulse>();
        }
    }

    private void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if(go != null)
        {
            player = go.transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(instantDetonateOnTrigger == false)
        {
            return;
        }

        int layer = collision.gameObject.layer;

        // 쉬프트(Shift) 연산 -> 비트를 지정한 회수만큼 왼쪽 또는 오른쪽으로 옮기는 연산.
        // layer 변수의 값을 왼쪽으로 한 칸 쉬프트.
        // 만약 layer 변수에 저장된 값이 1일 경우.
        // 1을 2진수로 변환하면 0000 0001.
        // 위의 값을 왼쪽으로 한 칸 쉬프트 연산을 하면 0000 0010이 된다.
        int bit = 1 << layer;

        // 비트 & 연산.
        // 두 수를 각각 2진수로 변환한 다음,
        // 양쪽의 요소가 모두 1인 경우에만 1, 나머지 경우에는 모두 0.
        int masked = instantLayer.value & bit;

        if(masked != 0)
        {
            // 폭발 처리.
            Explode();
        }
    }

    void Explode()
    {
        if(exploded == true)
        {
            return;
        }

        exploded = true;

        if(warning != null)
        {
            warning.StopWarning();
        }

        Vector3 p = transform.position;

        Collider2D[] hits = Physics2D.OverlapCircleAll(p, explodeRadius, targetLayer);
        if(hits != null)
        {
            for(int i=0; i<hits.Length; ++i)
            {
                Collider2D col = hits[i];
                if(col == null)
                {
                    continue;
                }

                IDamageable damageable = col.GetComponent<IDamageable>();
                if(damageable != null)
                {
                    damageable.ApplyDamage(explosionDamage);
                }
            }
        }

        if(chainAffectBomber == true)
        {
            Collider2D[] nearAll = Physics2D.OverlapCircleAll(p, explodeRadius);
            if(nearAll != null)
            {
                for(int i=0; i<nearAll.Length; ++i)
                {
                    Collider2D col = nearAll[i];
                    if(col == null)
                    {
                        continue;
                    }

                    SuicideBomber other = col.GetComponent<SuicideBomber>();
                    if(other == null)
                    {
                        continue;
                    }

                    if(other == this)
                    {
                        continue;
                    }

                    if(chainInstant == true)
                    {
                        // 폭발 요청.
                        other.RequestInstantExplosion();
                    }
                    else
                    {
                        // 퓨즈 시동 요청.
                        other.ArmFuseShort(chainFuseSeconds);
                    }
                }
            }
        }

        Destroy(gameObject);
    }

    public void RequestInstantExplosion()
    {
        Explode();
    }

    public void ArmFuseShort(float seconds)
    {
        if(exploded == true)
        {
            return;
        }

        fuseArmed = true;
        fuseRemain = seconds;

        if(warning != null)
        {
            warning.StartWarning();
        }
    }

    void ArmFuse()
    {
        if(fuseArmed == true)
        {
            return;
        }

        fuseArmed = true;
        fuseRemain = fuseSeconds;

        if(warning != null)
        {
            warning.StartWarning();
        }
    }

    private void Update()
    {
        if(exploded == true)
        {
            return;
        }

        if(player == null)
        {
            return;
        }

        if (enemyHealth != null)
        {
            if (enemyHealth.GetCurrentHealth() <= 0.0f)
            {
                return;
            }
        }

        Vector3 myPos = transform.position;
        Vector3 toPlayer = player.position - myPos;
        float dist = toPlayer.magnitude;

        if(dist <= detectDistance && fuseArmed == false)
        {
            if(dist > fuseRadius)
            {
                Vector3 dir = toPlayer.normalized;
                float step = moveSpeed * Time.deltaTime;
                transform.position = myPos + dir * step;

                if(animController != null)
                {
                    animController.PlayIdleOrMove(step);
                }

                UpdateDirection(dir.x);
            }
            else
            {
                ArmFuse();
            }
        }

        if(fuseArmed == true)
        {
            fuseRemain -= Time.deltaTime;
            if(fuseRemain <= 0.0f)
            {
                Explode();
            }
        }

        //if(toPlayer.sqrMagnitude > 0.0001f)
        //{
        //    transform.up = Vector3.Slerp(transform.up, toPlayer.normalized, 12.0f * Time.deltaTime);
        //}
    }

    void UpdateDirection(float dir)
    {
        if (sr != null)
        {
            if (dir < 0.0f)
            {
                sr.flipX = true;
            }
            else if (dir > 0.0f)
            {
                sr.flipX = false;
            }
        }
    }
}
