using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class ButtonChild : MonoBehaviour
{
    public float targetYPosition;
    [SerializeField] private float disappearDelay = 1f;

    private readonly HashSet<Collider> pressingColliders = new HashSet<Collider>();
    private readonly Dictionary<Transform, int> pressingPlayerCounts = new Dictionary<Transform, int>();

    private float initialYPosition;
    private Coroutine disappearCoroutine;
    private bool isPressed;
    private Transform disappearTriggerPlayer;

    private void OnEnable()
    {
        GlobalEvents.ButtonPressed += OnButtonPressed;
    }

    private void OnDisable()
    {
        GlobalEvents.ButtonPressed -= OnButtonPressed;

        if (disappearCoroutine != null)
        {
            StopCoroutine(disappearCoroutine);
            disappearCoroutine = null;
        }

        pressingColliders.Clear();
        pressingPlayerCounts.Clear();
    }

    private void Start()
    {
        initialYPosition = transform.position.y;

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
        isPressed = true;
        AddPressingPlayer(other, playerTransform);
    }

    private void OnTriggerStay(Collider other)
    {
        Transform playerTransform = GetPlayerTransform(other);

        if (playerTransform == null)
            return;

        if (isPressed)
            return;

        MoveToTargetY();
    }

    private void OnTriggerExit(Collider other)
    {
        Transform playerTransform = GetPlayerTransform(other);

        if (playerTransform == null)
            return;

        RemovePressingPlayer(other, playerTransform);

        if (!isPressed && pressingColliders.Count == 0)
            MoveToInitialY();
    }

    private Transform GetPlayerTransform(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player != null)
            return player.transform;

        PlayerOnRotatingPlatform playerOnRotatingPlatform = other.GetComponentInParent<PlayerOnRotatingPlatform>();
        if (playerOnRotatingPlatform != null)
            return playerOnRotatingPlatform.transform;

        return null;
    }

    private void OnButtonPressed(Transform playerTransform, Transform buttonTransform)
    {
        if (buttonTransform != transform)
            return;

        Debug.Log($"踩踏的是：{playerTransform.name}，按钮是：{buttonTransform.name}");
        StartDisappearCountdown(playerTransform);
    }

    private void StartDisappearCountdown(Transform playerTransform)
    {
        if (disappearCoroutine != null)
            return;

        disappearTriggerPlayer = playerTransform;
        disappearCoroutine = StartCoroutine(DisappearAfterDelay());
    }

    private IEnumerator DisappearAfterDelay()
    {
        yield return new WaitForSeconds(disappearDelay);

        GameObject buttonGameObject = GetButtonGameObject();
        GlobalEvents.RaiseButtonDisappearing(disappearTriggerPlayer);
        Destroy(buttonGameObject);
    }

    private GameObject GetButtonGameObject()
    {
        if (transform.parent != null)
            return transform.parent.gameObject;

        return gameObject;
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
