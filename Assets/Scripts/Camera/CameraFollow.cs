using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float followLerp = 0.15f;

    public float fixedZ = -10.0f;

    private void LateUpdate()
    {
        if(target == null)
        {
            return;
        }

        Vector3 current = transform.position;
        Vector3 desired = new Vector3(target.position.x, target.position.y, fixedZ);

        Vector3 smoothed = Vector3.Lerp(current, desired, followLerp);

        transform.position = smoothed;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
