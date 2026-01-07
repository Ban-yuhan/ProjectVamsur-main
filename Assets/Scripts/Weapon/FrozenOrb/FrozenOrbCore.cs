using UnityEngine;
using UnityEngine.UIElements;

public class FrozenOrbCore : MonoBehaviour
{
    [SerializeField]
    private GameObject shardPrefab;

    [SerializeField]
    private float moveSpeed = 6.5f;

    [SerializeField]
    private float maxDistance = 7.0f;

    [SerializeField]
    private float lifeTimeSec = 2.0f;

    [SerializeField]
    private float shardIntervalSec = 0.15f; 

    [SerializeField]
    private int shardBurstCount = 20;

    [SerializeField]
    private int shardPerTick = 4; //이동 중 한 번에 발사할 샤드 개수

    [SerializeField]
    private float shardDamage = 3.0f;

    [SerializeField]
    private float shardSpeed = 12.0f;

    [SerializeField]
    private float shardLifeSec = 1.0f;

    [SerializeField]
    private LayerMask enemyLayer;

    private Transform owner; //구체를 쏜 대상의 transform 정보
    private Vector2 moveDir;
    private Vector2 startPos; //시작 위치
    private float liveSec;
    private float shardTimer; //샤드 간격 타이머

    [SerializeField]
    private float frozenDuration = 1.5f;

    [SerializeField]
    private float decreaseSpeedRate = 0.5f;

    public void Setup(Transform ownerTransform, Vector2 direction)
    {
        owner = ownerTransform;
        moveDir = direction.normalized;

        startPos = transform.position;
        liveSec = 0.0f;
        shardTimer = 0.0f;
    }


    /// <summary>
    /// 각도를 방향벡터로 전환
    /// </summary>
    /// <param name="deg">각도</param>
    /// <returns></returns>
    Vector2 DegToDir(float deg)
    {
        float rad = Mathf.Deg2Rad * deg;

        float x = Mathf.Cos(rad);
        float y = Mathf.Sin(rad);

        Vector2 v = new Vector2(x, y);

        return v.normalized;
    }



    void SpawnOneShard(Vector2 dir)
    {
        Vector3 pos = transform.position;

        float deg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject obj = Instantiate(shardPrefab, pos, Quaternion.Euler(0.0f, 0.0f, deg - 90f));
        if(obj == null)
        {
            return;
        }

        IceShardProjectile shard = obj.GetComponent<IceShardProjectile>();
        if(shard == null)
        {
            Destroy(obj);
            return;
        }

        shard.Setup(dir, shardSpeed, shardLifeSec, shardDamage, enemyLayer);
    }


    void SpawnRadialShards(int count)
    {
        if (shardPrefab == null)
        {
            return;
        }

        float stepDeg = 360.0f / count;
        float startDeg = Random.Range(0.0f, stepDeg);

        for (int i = 0; i < count; ++i)
        {
            float deg = startDeg + (stepDeg * i); //각 파편별 각도의 간격 계산
            Vector2 dir = DegToDir(deg); //각도를 이용해 방향 계산
            SpawnOneShard(dir);
        }
    }


    void FinalBurst()
    {
        SpawnRadialShards(shardBurstCount);
    }


    bool IsFinished()
    {
        if(liveSec >= lifeTimeSec)
        {
            return true;
        }

        Vector2 curPos = transform.position;
        Vector2 diff = curPos - startPos;

        float travelSqr = diff.sqrMagnitude;
        float maxSqr = maxDistance * maxDistance;

        if(travelSqr >= maxSqr) //지금까지 이동한 거리가 최대거리보다 긴 경우
        {
            return true;
        }

        return false;
    }


    void UpdateShardTick()
    {
        shardTimer += Time.deltaTime;

        if(shardTimer >= shardIntervalSec)
        {
            shardTimer -= shardIntervalSec;
            SpawnRadialShards(shardPerTick); 
        }
    }


    void MoveForward()
    {
        Vector2 delta = moveDir * moveSpeed* Time.deltaTime;
        transform.position += (Vector3)delta;
    }


    private void Update()
    {
        liveSec += Time.deltaTime;

        MoveForward();
        UpdateShardTick();
        
        if(IsFinished() == true)
        {
            FinalBurst();
            Destroy(gameObject);
        }
    }
}
