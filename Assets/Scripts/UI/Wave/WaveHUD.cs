using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 웨이브 HUD 표시(웨이브 번호, 다음 웨이브까지 남은 시간).
/// - 표시만 담당.
/// </summary>
public class WaveHUD : MonoBehaviour
{
    [SerializeField]
    private Text waveText;     // "Wave 3"

    [SerializeField]
    private Text timerText;    // "Next in 12.3s"

    private float pendingTimer;

    /// <summary>
    /// 현재 웨이브 번호를 갱신한다.
    /// </summary>
    public void SetWave(int wave)
    {
        if (waveText != null)
        {
            waveText.text = "Wave " + wave;
        }
    }

    /// <summary>
    /// 다음 웨이브까지 남은 시간을 갱신한다(초).
    /// </summary>
    public void SetTimer(float seconds)
    {
        pendingTimer = seconds;
        if (timerText != null)
        {
            timerText.text = "Next in " + pendingTimer.ToString("0.0") + "s";
        }
    }

    /// <summary>
    /// UI를 부드럽게 보이게 하기 위해 unscaledDeltaTime으로 숫자만 미세 업데이트한다.
    /// </summary>
    private void Update()
    {
        // 표시 부드러움 용도.
        if (pendingTimer > 0.0f)
        {
            pendingTimer = Mathf.Max(0.0f, pendingTimer - Time.unscaledDeltaTime);
            if (timerText != null)
            {
                timerText.text = "Next in " + pendingTimer.ToString("0.0") + "s";
            }
        }
    }
}
