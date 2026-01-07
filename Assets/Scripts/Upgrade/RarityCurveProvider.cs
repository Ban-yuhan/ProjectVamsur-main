using UnityEngine;

/// <summary>
/// 시간(분)에 따라 등급별 "잘 나오는 정도(점수)"를 돌려준다.
/// 그래프를 이용해 쉽게 조정할 수 있다.
/// </summary>
public class RarityCurveProvider : MonoBehaviour
{
    [Header("등급별 그래프 (x=분, y=점수)")]
    [SerializeField]
    private AnimationCurve weightCommonCurve;

    [SerializeField]
    private AnimationCurve weightUncommonCurve;

    [SerializeField]
    private AnimationCurve weightRareCurve;

    [SerializeField]
    private AnimationCurve weightEpicCurve;

    /// <summary>
    /// 시간(초)을 넣으면 등급별 점수(정수)를 돌려준다.
    /// </summary>
    /// <param name="elapsedSeconds">게임 시작 후 지난 시간(초)</param>
    /// <param name="wC">Common 점수</param>
    /// <param name="wU">Uncommon 점수</param>
    /// <param name="wR">Rare 점수</param>
    /// <param name="wE">Epic 점수</param>
    /// <remarks>
    /// 부작용 없음. 예외 없음(그래프가 없으면 기본값).
    /// </remarks>
    public void GetWeightsByTime(float elapsedSeconds, out int wC, out int wU, out int wR, out int wE)
    {
        float minutes = Mathf.Max(0.0f, elapsedSeconds / 60.0f);

        wC = EvalInt(weightCommonCurve, minutes, 60);
        wU = EvalInt(weightUncommonCurve, minutes, 28);
        wR = EvalInt(weightRareCurve, minutes, 10);
        wE = EvalInt(weightEpicCurve, minutes, 2);

        if (wC < 1)
        {
            wC = 1;
        }
        if (wU < 1)
        {
            wU = 1;
        }
        if (wR < 1)
        {
            wR = 1;
        }
        if (wE < 1)
        {
            wE = 1;
        }
    }

    /// <summary>
    /// 그래프에서 값을 읽어 정수로 만든다. 없으면 기본값을 쓴다.
    /// </summary>
    private int EvalInt(AnimationCurve curve, float x, int fallback)
    {
        if (curve == null)
        {
            return fallback;
        }

        float v = curve.Evaluate(x);
        int i = Mathf.Max(1, Mathf.RoundToInt(v));
        return i;
    }
}
