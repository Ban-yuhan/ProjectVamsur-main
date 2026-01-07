using UnityEngine;

public class UpgradeDebugSelector : MonoBehaviour
{
    [SerializeField] private PlayerWeaponStatsRuntime stats;

    // Update is called once per frame
    void Update()
    {
        if(stats == null)
        {
            return;
        }

        // 관통 회수 증가.
        if(Input.GetKeyDown(KeyCode.Alpha1) == true)
        {
            stats.AddPierceBulletUpgrade(1, 0.0f);
        }

        // 총알 대미지 증가.
        if (Input.GetKeyDown(KeyCode.Alpha2) == true)
        {
            stats.AddPierceBulletUpgrade(0, 0.25f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) == true)
        {
            stats.AddOrbitBladesUpgrade(1, 0.0f, 0.0f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) == true)
        {
            stats.AddOrbitBladesUpgrade(0, 0.25f, 0.0f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5) == true)
        {
            stats.AddOrbitBladesUpgrade(0, 0.0f, 60.0f);
        }
    }
}
