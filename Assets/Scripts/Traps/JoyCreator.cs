using System.Collections;
using UnityEngine;

public class JoyCreator : MonoBehaviour
{
    public Transform TargetPlayer;
    public GameObject JoyPrefab;
    public Transform ShadowTransform;
    [SerializeField] private float followDuration = 2f;
    [SerializeField] private float followLerpSpeed = 5f;
    [SerializeField] private float headOffset = 2f;
    [SerializeField] private float fallSpeed = 5f;

    private Coroutine followCoroutine;
    private Coroutine fallCoroutine;
    private Coroutine shadowCoroutine;
    private float fixedYPosition;
    private Vector3 shadowDefaultScale;
    private Renderer[] shadowRenderers;

    private void Awake()
    {
        SetJoyPrefabActive(false);
        InitializeShadow();
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

        if (shadowCoroutine != null)
        {
            StopCoroutine(shadowCoroutine);
            shadowCoroutine = null;
        }
    }

    public void SetTargetPlayer(Transform targetPlayer)
    {
        TargetPlayer = targetPlayer;

        if (isActiveAndEnabled)
            StartFollowing();
    }

    public void InitializeShadow()
    {
        if (ShadowTransform == null)
            return;

        shadowDefaultScale = ShadowTransform.localScale;
        shadowRenderers = ShadowTransform.GetComponentsInChildren<Renderer>(true);
        SetShadowProgress(0f);
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
            FollowPlayer(Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        followCoroutine = null;
        StartFalling();
    }

    private void FollowPlayer(float deltaTime)
    {
        if (TargetPlayer == null)
            return;

        Vector3 targetPosition = TargetPlayer.position + Vector3.up * headOffset;
        targetPosition.y = fixedYPosition;

        float lerpRate = Mathf.Clamp01(followLerpSpeed * deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpRate);
    }

    private void StartFalling()
    {
        SetJoyPrefabActive(true);

        if (fallCoroutine != null)
            StopCoroutine(fallCoroutine);

        float fallDuration = GetFallDuration();
        StartShadowGrow(fallDuration);
        fallCoroutine = StartCoroutine(FallRoutine());
    }

    private float GetFallDuration()
    {
        if (JoyPrefab == null || fallSpeed <= 0f)
            return 0f;

        RaycastHit[] hits = Physics.RaycastAll(JoyPrefab.transform.position, Vector3.down);
        float closestDistance = float.MaxValue;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Plane") && hit.distance < closestDistance)
                closestDistance = hit.distance;
        }

        if (closestDistance == float.MaxValue)
            return 0f;

        return closestDistance / fallSpeed;
    }

    private void StartShadowGrow(float duration)
    {
        if (ShadowTransform == null)
            return;

        if (shadowCoroutine != null)
            StopCoroutine(shadowCoroutine);

        shadowCoroutine = StartCoroutine(GrowShadowRoutine(duration));
    }

    private IEnumerator GrowShadowRoutine(float duration)
    {
        if (duration <= 0f)
        {
            SetShadowProgress(1f);
            shadowCoroutine = null;
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            SetShadowProgress(Mathf.Clamp01(elapsedTime / duration));
            yield return null;
        }

        SetShadowProgress(1f);
        shadowCoroutine = null;
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

    private void SetShadowProgress(float progress)
    {
        if (ShadowTransform == null)
            return;

        float scaleRate = Mathf.Lerp(0.5f, 1f, progress);
        float alpha = Mathf.Lerp(0.3f, 1f, progress);

        ShadowTransform.localScale = shadowDefaultScale * scaleRate;
        SetShadowAlpha(alpha);
    }

    private void SetShadowAlpha(float alpha)
    {
        if (shadowRenderers == null)
            return;

        foreach (Renderer shadowRenderer in shadowRenderers)
        {
            foreach (Material material in shadowRenderer.materials)
            {
                Color color = material.color;
                color.a = alpha;
                material.color = color;
            }
        }
    }
}
