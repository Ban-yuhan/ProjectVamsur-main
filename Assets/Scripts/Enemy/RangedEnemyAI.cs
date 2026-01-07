using UnityEngine;

/// <summary>
/// 원거리 적의 이동/조준 기능 담당.
/// 플레이어와의 거리를 일정 값으로 유지하고 플레이어를 향하도록 한다.
/// </summary>
public class RangedEnemyAI : MonoBehaviour
{
    [SerializeField]
    private Transform player;

    [SerializeField]
    private float moveSpeed = 2.5f;

    [SerializeField]
    private float minDistance = 5.0f;   // 플레이어와의 최소 거리.

    [SerializeField]
    private float maxDistance = 8.0f;   // 플레이어와의 최대 거리.

    [SerializeField]
    private float rotateLerp = 12.0f;   // 회전 보간 속도. 값이 클수록 빨리 회전.

    [SerializeField]
    private AnimationController animController;

    [SerializeField]
    private SpriteRenderer sr;

    [SerializeField]
    private EnemyHealth enemyHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if(go != null)
        {
            player = go.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
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

        Vector3 pos = transform.position;
        Vector3 toPlayer = player.position - pos;
        float dist = toPlayer.magnitude;    // 거리 구하기.

        // 이동할 방향 설정.
        Vector3 moveDir = Vector3.zero;
        if(dist > maxDistance)  // 플레이어와의 거리가 너무 멀면 다가감.
        {
            moveDir = toPlayer.normalized;
        }
        else if(dist < minDistance) // 플레이어와의 거리가 너무 가까우면 물러남.
        {
            moveDir = (-toPlayer).normalized;
        }

        if(moveDir.sqrMagnitude > 0.0f)
        {
            float step = moveSpeed * Time.deltaTime;    // 이동량 계산.
            transform.position = pos + moveDir * step;

            if(animController != null)
            {
                animController.PlayIdleOrMove(step);
            }
        }

        // 플레이어를 향해서 부드럽게 회전 보간.
        //if(toPlayer.sqrMagnitude > 0.0001f)
        //{
        //    Vector3 targetUp = toPlayer.normalized;
        //    Vector3 newUp = Vector3.Slerp(transform.up, targetUp, rotateLerp * Time.deltaTime);
        //    transform.up = newUp;
        //}

        UpdateDirection(moveDir.x);
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
