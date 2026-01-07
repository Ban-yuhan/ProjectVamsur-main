using UnityEngine;

/// <summary>
/// 카메라 루트를 진동시켜 타격감을 준다.
/// </summary>
public class CameraShaker : MonoBehaviour
{
    private Vector3 initialPos;
    private float intensity;
    private float time;

    private void Awake()
    {
        initialPos = transform.localPosition;
    }

    private void Update()
    {
        if (time > 0.0f)
        {
            time = time - Time.unscaledDeltaTime;
            Vector2 jitter = Random.insideUnitCircle * intensity;
            transform.localPosition = initialPos + new Vector3(jitter.x, jitter.y, 0.0f);

            intensity = Mathf.Lerp(intensity, 0.0f, 10.0f * Time.unscaledDeltaTime);

            if (time <= 0.0f)
            {
                transform.localPosition = initialPos;
            }
        }
    }

    public void Shake(float duration, float startIntensity)
    {
        initialPos = transform.localPosition;
        time = Mathf.Max(time, duration);
        intensity = Mathf.Max(intensity, startIntensity);
    }
}
