using System.Collections;
using UnityEngine;

public class JoyCreator : MonoBehaviour
{
    public Transform TargetPlayer;
    public GameObject JoyPrefab;
    [SerializeField] private float followDuration = 2f;
    [SerializeField] private float headOffset = 2f;

    private Coroutine followCoroutine;
    private float fixedYPosition;

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
    }

    private void FollowPlayer()
    {
        if (TargetPlayer == null)
            return;

        Vector3 position = TargetPlayer.position + Vector3.up * headOffset;
        position.y = fixedYPosition;
        transform.position = position;
    }
}
