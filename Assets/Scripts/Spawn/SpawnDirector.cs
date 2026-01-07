using UnityEngine;

public class SpawnDirector : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve spawnRateCurve; // 시간에 따른 초당 생성 속도를 담는 곡선

    [SerializeField]
    private float startDelay = 2.0f; // 시작을 잠깐 늦추기 위한 지연 시간

    [SerializeField]
    private EnemySpawnerPlus spawner; // 실제 생성을 담당하는 실행자 참조

    [SerializeField]
    private float timeScale = 1.0f; // 내부 시간의 빠르기를 조절하는 배율

    private float elapsed; // 시작 이후 누적된 시간
    private float accumulator; // 초당 속도를 적립해 두는 누적 점수

    private void OnEnable()
    {
        elapsed = 0.0f; // 누적 시간 초기화
        accumulator = 0.0f; // 누적 점수 초기화
    }

    private void Update()
    {
        elapsed += Time.deltaTime * timeScale; // 시간 배율을 반영해 누적 시간 증가
        if (elapsed < startDelay)
        {
            return;
        }

        float t = elapsed - startDelay; // 곡선에 넣을 기준 시간
        if (t < 0.0f)
        {
            t = 0.0f;
        }

        float rate = 0.0f; // 현재 시점의 초당 생성 속도
        if (spawnRateCurve != null)
        {
            rate = spawnRateCurve.Evaluate(t);
        }

        if (rate < 0.0f)
        {
            rate = 0.0f;
        }

        accumulator += rate * Time.deltaTime; // 누적 점수 증가

        while (accumulator >= 1.0f)
        {
            bool didSpawn = false; // 이번 반복에서 실제로 생성했는지 여부
            if (spawner != null)
            {
                didSpawn = spawner.TrySpawnOne(); // 스포너가 상한을 확인하고 생성
            }

            if (didSpawn == true)
            {
                accumulator -= 1.0f; // 성공하면 기준만큼 차감
            }
            else
            {
                break; // 상한으로 보류된 경우, 점수를 유지하고 다음 기회를 기다림
            }
        }
    }
}
