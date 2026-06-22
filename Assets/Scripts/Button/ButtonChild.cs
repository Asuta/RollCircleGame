using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class ButtonChild : MonoBehaviour
{
    public float targetYPosition;

    private readonly HashSet<Collider> pressingColliders = new HashSet<Collider>();
    private readonly Dictionary<Transform, int> pressingPlayerCounts = new Dictionary<Transform, int>();

    private float initialYPosition;
    private int playerLayer;

    private void OnEnable()
    {
        GlobalEvents.ButtonPressed += OnButtonPressed;
    }

    private void OnDisable()
    {
        GlobalEvents.ButtonPressed -= OnButtonPressed;
        pressingColliders.Clear();
        pressingPlayerCounts.Clear();
    }

    private void Start()
    {
        initialYPosition = transform.position.y;
        playerLayer = LayerMask.NameToLayer("Player");

        Collider triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null)
            triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform playerTransform = GetPlayerTransform(other);

        if (playerTransform == null)
            return;

        MoveToTargetY();
        AddPressingPlayer(other, playerTransform);
    }

    private void OnTriggerStay(Collider other)
    {
        Transform playerTransform = GetPlayerTransform(other);

        if (playerTransform == null)
            return;

        MoveToTargetY();
    }

    private void OnTriggerExit(Collider other)
    {
        Transform playerTransform = GetPlayerTransform(other);

        if (playerTransform == null)
            return;

        RemovePressingPlayer(other, playerTransform);

        if (pressingColliders.Count == 0)
            MoveToInitialY();
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

    private void OnButtonPressed(Transform playerTransform, Transform buttonTransform)
    {
        if (buttonTransform != transform)
            return;

        Debug.Log($"踩踏的是：{playerTransform.name}，按钮是：{buttonTransform.name}");
    }

    private void AddPressingPlayer(Collider playerCollider, Transform playerTransform)
    {
        if (!pressingColliders.Add(playerCollider))
            return;

        if (!pressingPlayerCounts.ContainsKey(playerTransform))
        {
            pressingPlayerCounts[playerTransform] = 1;
            GlobalEvents.RaiseButtonPressed(playerTransform, transform);
            return;
        }

        pressingPlayerCounts[playerTransform]++;
    }

    private void RemovePressingPlayer(Collider playerCollider, Transform playerTransform)
    {
        if (!pressingColliders.Remove(playerCollider))
            return;

        if (!pressingPlayerCounts.TryGetValue(playerTransform, out int count))
            return;

        count--;

        if (count <= 0)
        {
            pressingPlayerCounts.Remove(playerTransform);
            return;
        }

        pressingPlayerCounts[playerTransform] = count;
    }

    private void MoveToTargetY()
    {
        Vector3 position = transform.position;
        position.y = targetYPosition;
        transform.position = position;
    }

    private void MoveToInitialY()
    {
        Vector3 position = transform.position;
        position.y = initialYPosition;
        transform.position = position;
    }
}
