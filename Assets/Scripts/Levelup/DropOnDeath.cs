using UnityEngine;

public class DropOnDeath : MonoBehaviour
{
    [SerializeField]
    private GameObject experienceOrbPrefab;

    [SerializeField]
    private int count = 1;

    [SerializeField]
    private float scatterRadius = 0.5f;

    public void SpawnDrops()
    {
        if(experienceOrbPrefab == null)
        {
            return;
        }

        int randomCount = Random.Range(1, count+1);

        for(int i=0; i< randomCount; ++i)
        {
            Vector2 offset = Random.insideUnitCircle * scatterRadius;
            Vector3 pos = transform.position + new Vector3(offset.x, offset.y, 0.0f);
            Instantiate(experienceOrbPrefab, pos, Quaternion.identity);
        }
    }
}
