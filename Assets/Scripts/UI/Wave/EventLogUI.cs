using UnityEngine;
using UnityEngine.UI;
using System.Text;

/// <summary>
/// 스테이지 이벤트 로그를 단순 누적 출력한다.
/// </summary>
public class EventLogUI : MonoBehaviour
{
    [SerializeField]
    private Text logText;

    private StringBuilder builder = new StringBuilder(1024);

    /// <summary>
    /// 로그 한 줄을 추가한다.
    /// </summary>
    /// <param name="line">추가할 문자열</param>
    public void Append(string line)
    {
        if (string.IsNullOrEmpty(line) == true)
        {
            return;
        }

        builder.AppendLine(line);

        if (logText != null)
        {
            logText.text = builder.ToString();
        }
    }
}
