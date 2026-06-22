using System;
using UnityEngine;

public static class GlobalEvents
{
    public static event Action<Transform, Transform> RayHitPlayer;
    public static event Action<Transform, Transform> ButtonPressed;
    public static event Action<Transform> ButtonDisappearing;

    public static void RaiseRayHitPlayer(Transform rayTransform, Transform playerTransform)
    {
        RayHitPlayer?.Invoke(rayTransform, playerTransform);
    }

    public static void RaiseButtonPressed(Transform playerTransform, Transform buttonTransform)
    {
        ButtonPressed?.Invoke(playerTransform, buttonTransform);
    }

    public static void RaiseButtonDisappearing(Transform playerTransform)
    {
        ButtonDisappearing?.Invoke(playerTransform);
    }
}
