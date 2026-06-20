using System;
using UnityEngine;

public static class GlobalEvents
{
    public static event Action<Transform, Transform> RayHitPlayer;

    public static void RaiseRayHitPlayer(Transform rayTransform, Transform playerTransform)
    {
        RayHitPlayer?.Invoke(rayTransform, playerTransform);
    }
}
