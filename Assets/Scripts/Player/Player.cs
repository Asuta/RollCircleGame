using UnityEngine;

public class Player : MonoBehaviour
{
    private void OnEnable()
    {
        GlobalEvents.RayHitPlayer += OnRayHitPlayer;
    }

    private void OnDisable()
    {
        GlobalEvents.RayHitPlayer -= OnRayHitPlayer;
    }

    private void OnRayHitPlayer(Transform rayTransform, Transform playerTransform)
    {
        if (playerTransform != transform)
            return;

        Debug.Log("我被击中了");
    }
}
