using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class Ray : MonoBehaviour
{
    private readonly HashSet<Collider> detectedPlayers = new HashSet<Collider>();
    private readonly Dictionary<Transform, int> detectedPlayerCounts = new Dictionary<Transform, int>();

    private Collider triggerCollider;
    private Rigidbody rayRigidbody;
    private Renderer rayRenderer;
    private int playerLayer;


    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        rayRigidbody = GetComponent<Rigidbody>();
        rayRenderer = GetComponent<Renderer>();
        playerLayer = LayerMask.NameToLayer("Player");

        if (triggerCollider != null)
            triggerCollider.isTrigger = true;

        if (rayRigidbody == null)
            rayRigidbody = gameObject.AddComponent<Rigidbody>();

        rayRigidbody.isKinematic = true;
        rayRigidbody.useGravity = false;

        SetRayColor(Color.red);
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform playerTransform = GetPlayerTransform(other);

        if (playerTransform == null)
            return;

        AddDetectedPlayer(other, playerTransform);
        SetRayColor(Color.black);
    }

    private void OnTriggerStay(Collider other)
    {
        Transform playerTransform = GetPlayerTransform(other);

        if (playerTransform == null)
            return;

        AddDetectedPlayer(other, playerTransform);
        SetRayColor(Color.black);
    }

    private void OnTriggerExit(Collider other)
    {
        Transform playerTransform = GetPlayerTransform(other);

        if (playerTransform == null)
            return;

        RemoveDetectedPlayer(other, playerTransform);

        if (detectedPlayers.Count == 0)
            SetRayColor(Color.red);
    }

    private void OnDisable()
    {
        detectedPlayers.Clear();
        detectedPlayerCounts.Clear();
        SetRayColor(Color.red);
    }

    private Transform GetPlayerTransform(Collider other)
    {
        if (playerLayer < 0)
            return null;

        Player player = other.GetComponentInParent<Player>();
        if (player != null)
            return player.transform;

        if (other.gameObject.layer == playerLayer)
            return other.transform;

        if (other.transform.root.gameObject.layer == playerLayer)
            return other.transform.root;

        return null;
    }

    private void AddDetectedPlayer(Collider playerCollider, Transform playerTransform)
    {
        if (!detectedPlayers.Add(playerCollider))
            return;

        if (!detectedPlayerCounts.ContainsKey(playerTransform))
        {
            detectedPlayerCounts[playerTransform] = 1;
            GlobalEvents.RaiseRayHitPlayer(transform, playerTransform);
            return;
        }

        detectedPlayerCounts[playerTransform]++;
    }

    private void RemoveDetectedPlayer(Collider playerCollider, Transform playerTransform)
    {
        if (!detectedPlayers.Remove(playerCollider))
            return;

        if (!detectedPlayerCounts.TryGetValue(playerTransform, out int count))
            return;

        count--;

        if (count <= 0)
        {
            detectedPlayerCounts.Remove(playerTransform);
            return;
        }

        detectedPlayerCounts[playerTransform] = count;
    }

    private void SetRayColor(Color color)
    {
        if (rayRenderer == null)
            return;

        rayRenderer.material.color = color;
    }
}
