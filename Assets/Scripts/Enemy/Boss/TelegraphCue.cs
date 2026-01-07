using UnityEngine;

/// <summary>
/// 간단한 텔레그래프 표시(원/직사각/스윕라인).
/// 실제 게임에서는 전용 쉐이더/라인 렌더러를 권장.
/// </summary>
public class TelegraphCue : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;     // 간단 표시용

    public void Show()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.transform.localScale = Vector3.one;
        }
    }

    public void ShowSweep(Vector3 origin, Vector2 dir, float length, float width)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.transform.position = origin + (Vector3)(dir.normalized * (length * 0.5f));
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            spriteRenderer.transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
            spriteRenderer.transform.localScale = new Vector3(length, width, 1.0f);
        }
    }

    public void Hide()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }
}
