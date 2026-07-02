using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Car : MonoBehaviour
{
    public static event Action<Car> HitPlayer;

    private readonly HashSet<Transform> hitPlayers = new HashSet<Transform>();

    private int playerLayer;

    public static void ClearHitPlayerListeners()
    {
        HitPlayer = null;
    }

    private void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHitPlayer(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryHitPlayer(collision.collider);
    }

    private void OnDisable()
    {
        hitPlayers.Clear();
    }

    private void TryHitPlayer(Collider other)
    {
        Transform playerTransform = GetPlayerTransform(other);
        if (playerTransform == null)
            return;

        if (!hitPlayers.Add(playerTransform))
            return;

        GlobalEvents.RaiseRayHitPlayer(transform, playerTransform);
        HitPlayer?.Invoke(this);
    }

    private Transform GetPlayerTransform(Collider other)
    {
        if (other == null)
            return null;

        Player player = other.GetComponentInParent<Player>();
        if (player != null)
            return player.transform;

        PlayerOnRotatingPlatform playerOnRotatingPlatform = other.GetComponentInParent<PlayerOnRotatingPlatform>();
        if (playerOnRotatingPlatform != null)
            return playerOnRotatingPlatform.transform;

        if (playerLayer >= 0)
        {
            if (other.gameObject.layer == playerLayer)
                return other.transform;

            if (other.transform.root.gameObject.layer == playerLayer)
                return other.transform.root;
        }

        return null;
    }
}
