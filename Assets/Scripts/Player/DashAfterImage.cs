using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class DashAfterImage : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private float lifeSec;
    private float elapsedSec;
    private float startAlpha;
    private float endAlpha;

    public void Initialize(Sprite sprite, bool flipX, bool flipY, string sortingLayerName, int sortingOrder, float durationSec, float fromAlpha, float toAlpha)
    {
        if(spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.sprite = sprite;
        spriteRenderer.flipX = flipX;
        spriteRenderer.flipY = flipY;
        spriteRenderer.sortingLayerName = sortingLayerName;
        spriteRenderer.sortingOrder = sortingOrder;

        lifeSec = Mathf.Max(0.01f, durationSec);
        elapsedSec = 0.0f;

        startAlpha = Mathf.Clamp01(fromAlpha);
        endAlpha = Mathf.Clamp01(toAlpha);

        Color c = spriteRenderer.color;
        c.a = startAlpha;
        spriteRenderer.color = c;
    }

    // Update is called once per frame
    void Update()
    {
        if(spriteRenderer == null)
        {
            return;
        }

        elapsedSec += Time.deltaTime;

        float t = elapsedSec / lifeSec;
        t = Mathf.Clamp01(t);
        float a = Mathf.Lerp(startAlpha, endAlpha, t);

        Color c = spriteRenderer.color;
        c.a = a;
        spriteRenderer.color = c;

        if(elapsedSec >= lifeSec)
        {
            gameObject.SetActive(false);
        }
    }
}
