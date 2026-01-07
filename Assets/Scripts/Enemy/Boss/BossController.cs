using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 보스 패턴 실행을 순환 관리한다.
/// - Playing 상태에서만 동작.
/// - 패턴 사이 쿨다운, 중복 실행 방지, 취소 처리.
/// </summary>
public class BossController : MonoBehaviour
{
    [SerializeField]
    private List<AttackPatternBase> patterns = new List<AttackPatternBase>();

    [SerializeField]
    private float betweenPatternDelay = 0.6f;

    private GameManager gameManager;
    private Coroutine loopRoutine;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        if (patterns == null || patterns.Count == 0)
        {
            patterns = new List<AttackPatternBase>(GetComponentsInChildren<AttackPatternBase>(true));
        }
    }

    private void OnEnable()
    {
        if (loopRoutine == null)
        {
            loopRoutine = StartCoroutine(Loop());
        }
    }

    private void OnDisable()
    {
        if (loopRoutine != null)
        {
            StopCoroutine(loopRoutine);
            loopRoutine = null;
        }
    }

    private IEnumerator Loop()
    {
        int index = 0;
        while (true)
        {
            if (gameManager != null)
            {
                if (gameManager.IsPlaying() == false)
                {
                    yield return null;
                    continue;
                }
            }

            if (patterns.Count == 0)
            {
                yield return null;
                continue;
            }

            AttackPatternBase pat = patterns[index];
            if (pat != null)
            {
                yield return StartCoroutine(pat.Run());
            }

            // 딜레이
            float t = 0.0f;
            while (t < betweenPatternDelay)
            {
                if (gameManager != null)
                {
                    if (gameManager.IsPlaying() == false)
                    {
                        yield return null;
                        continue;
                    }
                }
                t += Time.deltaTime;
                yield return null;
            }

            index = (index + 1) % patterns.Count;
        }
    }
}
