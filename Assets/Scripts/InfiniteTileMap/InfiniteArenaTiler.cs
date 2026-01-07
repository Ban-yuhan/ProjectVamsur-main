using System.Collections.Generic;
using UnityEngine;

public class InfiniteArenaTiler : MonoBehaviour
{
    [SerializeField]
    private Transform player;

    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    private float tileSize = 10.0f;

    [SerializeField]
    private int halfGridX = 2;  // 플레이어 기준 가로 방향 절반 칸 수.

    [SerializeField]
    private int halfGridY = 2;  // 플레이어 기준 세로 방향 절반 칸 수.

    [SerializeField]
    private bool snapToWhole = true;    // 타일 경계와 정확히 맞추는지 여부.

    private List<Transform> tiles = new List<Transform>();
    private int gridWidth;  // 전체 가로 칸 수.
    private int gridHeight; // 전체 세로 칸 수.
    private int currentCenterX; // 현재 중심 격자의 가로 좌표.
    private int currentCenterY; // 현재 중심 격자의 세로 좌표.
    private bool initialized;   // 초기화 여부.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(player == null)
        {
            return;
        }

        if(tilePrefab == null)
        {
            return;
        }

        if(tileSize <= 0.0f)
        {
            tileSize = 1.0f;
        }

        if(halfGridX < 1)
        {
            halfGridX = 1;
        }

        if (halfGridY < 1)
        {
            halfGridY = 1;
        }

        gridWidth = halfGridX * 2 + 1;
        gridHeight = halfGridY * 2 + 1;

        int total = gridWidth * gridHeight;

        for(int i=0; i<total; ++i)
        {
            GameObject go = Instantiate(tilePrefab, transform);
            if(go != null)
            {
                go.transform.localScale = new Vector3(tileSize, tileSize, go.transform.localScale.z);
                tiles.Add(go.transform);
            }
        }

        Vector2Int center = GetPlayerCell();
        currentCenterX = center.x;
        currentCenterY = center.y;

        RepositionAll();
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(initialized  == false)
        {
            return;
        }

        if(player == null)
        {
            return;
        }

        Vector2Int cell = GetPlayerCell();
        if(cell.x != currentCenterX || cell.y != currentCenterY)
        {
            currentCenterX = cell.x;
            currentCenterY = cell.y;
            RepositionAll();
        }
    }

    /// <summary>
    /// 타일들을 현재 중심 격자 기준으로 모두 재배치한다.
    /// </summary>
    void RepositionAll()
    {
        int index = 0;

        for(int gy=-halfGridY; gy<=halfGridY; ++gy)
        {
            for(int gx=-halfGridX; gx<=halfGridX; ++gx)
            {
                if(index >= tiles.Count)
                {
                    return;
                }

                int cellX = currentCenterX + gx;    // 배치할 격자 가로 좌표.
                int CellY = currentCenterY + gy;    // 배치할 격자 세로 좌표.

                float px = cellX * tileSize;    // 배치할 살제 가로 위치.
                float py = CellY * tileSize;    // 배치할 실제 세로 위치.

                tiles[index].position = new Vector3(px, py, tiles[index].position.z);

                ++index;
            }
        }
    }

    /// <summary>
    /// 플레이어 위치로부터 격자 좌표를 계산한다.
    /// </summary>
    /// <returns>격자 좌표</returns>
    Vector2Int GetPlayerCell()
    {
        Vector3 p = player.position;
        float gx = p.x / tileSize;  // 가로 방향 비율.
        float gy = p.y / tileSize;  // 세로 방향 비율.

        int cx;
        int cy;

        if(snapToWhole == true)
        {
            cx = Mathf.FloorToInt(gx);
            cy = Mathf.FloorToInt(gy);
        }
        else
        {
            cx = Mathf.RoundToInt(gx);
            cy = Mathf.RoundToInt(gy);
        }

        return new Vector2Int(cx, cy);
    }
}
