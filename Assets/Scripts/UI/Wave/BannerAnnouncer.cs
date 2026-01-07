using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 상단 경고 배너를 잠시 표시했다가 페이드아웃.
/// </summary>
public class BannerAnnouncer : MonoBehaviour
{
    [SerializeField]
    private Text label;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private float fadeOutTime = 0.5f;

    private float remain;

    /// <summary>
    /// 배너를 표시한다.
    /// </summary>
    /// <param name="message">표시할 메시지</param>
    /// <param name="holdSeconds">완전 표시 유지 시간</param>
    public void Show(string message, float holdSeconds)
    {
        if (label != null)
        {
            label.text = message;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1.0f;
        }

        remain = Mathf.Max(0.0f, holdSeconds);
        gameObject.SetActive(true);
    }

    /// <summary>
    /// unscaledDeltaTime으로 페이드아웃 진행.
    /// </summary>
    private void Update()
    {
        if (remain > 0.0f)
        {
            remain -= Time.unscaledDeltaTime;
            return;
        }

        if (canvasGroup != null)
        {
            float a = canvasGroup.alpha;
            a -= Time.unscaledDeltaTime / Mathf.Max(0.01f, fadeOutTime);
            if (a <= 0.0f)
            {
                a = 0.0f;
                gameObject.SetActive(false);
            }
            canvasGroup.alpha = a;
        }
    }
}
