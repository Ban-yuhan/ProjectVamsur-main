using UnityEngine;

public class EnemyLifetimeHook : MonoBehaviour
{
    private EnemySpawner spawner;

    public void SetSpawner(EnemySpawner value)
    {
        spawner = value;
    }

    private void OnDestroy()
    {
        if(spawner != null)
        {
            spawner.NotifyEnemyDied();
        }
    }
}
