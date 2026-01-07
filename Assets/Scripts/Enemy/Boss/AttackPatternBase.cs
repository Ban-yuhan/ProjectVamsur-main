using UnityEngine;
using System.Collections;

/// <summary>
/// 보스 패턴의 공통 수명주기 템플릿.
/// </summary>
public abstract class AttackPatternBase : MonoBehaviour
{
    [SerializeField]
    protected float prepareTime = 0.6f;   // 예고 시간

    [SerializeField]
    protected float recoverTime = 0.5f;   // 후딜 시간

    [SerializeField]
    protected TelegraphCue telegraph;     // 예고 비주얼

    protected GameManager gameManager;

    protected virtual void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public IEnumerator Run()
    {
        yield return StartCoroutine(Prepare());
        yield return StartCoroutine(Execute());
        yield return StartCoroutine(Recover());
    }

    protected virtual IEnumerator Prepare()
    {
        if (telegraph != null)
        {
            telegraph.Show();
        }

        float t = 0.0f;
        while (t < prepareTime)
        {
            if (gameManager != null)
            {
                if (gameManager.IsPlaying() == false)
                {
                    yield return null;
                    continue;
                }
            }

            t = t + Time.deltaTime;
            yield return null;
        }

        if (telegraph != null)
        {
            telegraph.Hide();
        }
    }

    protected abstract IEnumerator Execute();

    protected virtual IEnumerator Recover()
    {
        float t = 0.0f;
        while (t < recoverTime)
        {
            if (gameManager != null)
            {
                if (gameManager.IsPlaying() == false)
                {
                    yield return null;
                    continue;
                }
            }

            t = t + Time.deltaTime;
            yield return null;
        }
    }
}
