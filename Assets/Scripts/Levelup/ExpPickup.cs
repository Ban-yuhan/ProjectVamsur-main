using UnityEngine;

/// <summary>
/// 경험치 구슬. 플레이어가 트리거에 진입하면 지정된 경험치를 지급하고 사라진다.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ExpPickup : MonoBehaviour
{
    [SerializeField]
    private float expAmount = 5.0f; // 지급할 경험치

    [SerializeField]
    private float moveSpeedWhenMagnet = 9.0f;

    private Transform targetPlayer;
    private bool isMagnetized;

    // Update is called once per frame
    void Update()
    {
        if (isMagnetized == true && targetPlayer != null)
        {
            Vector3 current = transform.position;
            Vector3 goal = targetPlayer.position;
            Vector3 to = goal - current;
            float dist = to.magnitude;

            Vector3 dir = Vector3.zero;
            if (dist > 0.0001f)
            {
                dir = to / dist;
            }

            Vector3 delta = dir * moveSpeedWhenMagnet * Time.deltaTime;
            transform.position = current + delta;
        }
    }

    /// <summary>
    /// 플레이어가 트리거에 진입하면 경험치를 지급한다.
    /// </summary>
    /// <param name="other">진입한 콜라이더</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }

        PlayerExperience px = other.GetComponent<PlayerExperience>();
        if (px != null)
        {
            px.AddExp(expAmount);
        }

        GameObject.Destroy(gameObject);
    }

    public void BeginMagnetize(Transform player)
    {
        targetPlayer = player;
        isMagnetized = true;
    }

    public void SetAmount(float value)
    {
        expAmount = value;
    }
}
