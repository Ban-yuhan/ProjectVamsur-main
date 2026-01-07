using UnityEngine;

public class AliveRegistryAuto : MonoBehaviour
{
    [SerializeField]
    private AliveRegistry aliveRegistry; // 레지스트리 참조

    private void Awake()
    {
        aliveRegistry = GameObject.FindAnyObjectByType<AliveRegistry>();
    }

    private void OnDestroy()
    {
        if (aliveRegistry != null)
        {
            aliveRegistry.Unregister(gameObject); // 파괴 시 해제
        }
    }
}
