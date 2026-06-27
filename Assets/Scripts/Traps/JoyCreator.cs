using System.Collections;
using UnityEngine;

public class JoyCreator : MonoBehaviour
{
    public Transform TargetPlayer;
    public GameObject JoyPrefab;
    [SerializeField] private float followDuration = 2f;
    [SerializeField] private float headOffset = 2f;
    [SerializeField] private float fallSpeed = 5f;

    private Coroutine followCoroutine;
    private Coroutine fallCoroutine;
    private float fixedYPosition;

    private void Awake()
    {
        SetJoyPrefabActive(false);
    }

    private void Start()
    {
        StartFollowing();
    }

    private void OnDisable()
    {
        if (followCoroutine != null)
        {
            StopCoroutine(followCoroutine);
            followCoroutine = null;
        }

        if (fallCoroutine != null)
        {
            StopCoroutine(fallCoroutine);
            fallCoroutine = null;
        }
    }

    public void SetTargetPlayer(Transform targetPlayer)
    {
        TargetPlayer = targetPlayer;

        if (isActiveAndEnabled)
            StartFollowing();
    }

    private void StartFollowing()
    {
        if (TargetPlayer == null)
            return;

        if (followCoroutine != null)
            StopCoroutine(followCoroutine);

        fixedYPosition = transform.position.y;
        followCoroutine = StartCoroutine(FollowPlayerRoutine());
    }

    private IEnumerator FollowPlayerRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < followDuration)
        {
            FollowPlayer();
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        FollowPlayer();
        followCoroutine = null;
        StartFalling();
    }

    private void FollowPlayer()
    {
        if (TargetPlayer == null)
            return;

        Vector3 position = TargetPlayer.position + Vector3.up * headOffset;
        position.y = fixedYPosition;
        transform.position = position;
    }

    private void StartFalling()
    {
        SetJoyPrefabActive(true);

        if (fallCoroutine != null)
            StopCoroutine(fallCoroutine);

        fallCoroutine = StartCoroutine(FallRoutine());
    }

    private IEnumerator FallRoutine()
    {
        if (JoyPrefab == null)
            yield break;

        Transform joyTransform = JoyPrefab.transform;

        while (true)
        {
            float moveDistance = fallSpeed * Time.deltaTime;

            if (Physics.Raycast(joyTransform.position, Vector3.down, out RaycastHit hit, moveDistance)
                && hit.collider.CompareTag("Plane"))
            {
                joyTransform.position = hit.point;
                Destroy(gameObject);
                yield break;
            }

            joyTransform.position += Vector3.down * moveDistance;
            yield return null;
        }
    }

    private void SetJoyPrefabActive(bool isActive)
    {
        if (JoyPrefab != null)
            JoyPrefab.SetActive(isActive);
    }
}
