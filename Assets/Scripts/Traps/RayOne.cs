using System.Collections;
using UnityEngine;

public class RayOne : MonoBehaviour
{
    [SerializeField] private float scaleInDuration = 0.5f;

    private Vector3 initialScale;
    private Coroutine scaleInCoroutine;
    public Transform CircleCenter;
    public Transform RayLine;

    private void Awake()
    {
        initialScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        if (scaleInCoroutine != null)
            StopCoroutine(scaleInCoroutine);

        scaleInCoroutine = StartCoroutine(ScaleInRoutine());
    }

    private void Start()
    {
        UpdateRayLineLength();
    }

    private void OnDisable()
    {
        if (scaleInCoroutine != null)
        {
            StopCoroutine(scaleInCoroutine);
            scaleInCoroutine = null;
        }
    }

    private IEnumerator ScaleInRoutine()
    {
        if (scaleInDuration <= 0f)
        {
            transform.localScale = initialScale;
            scaleInCoroutine = null;
            yield break;
        }

        float elapsedTime = 0f;
        transform.localScale = Vector3.zero;

        while (elapsedTime < scaleInDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / scaleInDuration);
            transform.localScale = Vector3.Lerp(Vector3.zero, initialScale, t);
            yield return null;
        }

        transform.localScale = initialScale;
        scaleInCoroutine = null;
    }

    public static float CalculateRayLength(float angleOffsetDegrees, float circleRadius)
    {
        if (circleRadius <= 0f)
            return 0f;

        float angleOffsetRadians = angleOffsetDegrees * Mathf.Deg2Rad;
        float length = 2f * circleRadius * Mathf.Cos(angleOffsetRadians);

        return Mathf.Max(0f, length);
    }

    private void UpdateRayLineLength()
    {
        if (CircleCenter == null || RayLine == null)
            return;

        Vector3 currentScale = transform.localScale;
        transform.localScale = initialScale;

        Vector3 forward = transform.forward;
        forward.y = 0f;

        Vector3 directionToCenter = CircleCenter.position - transform.position;
        directionToCenter.y = 0f;

        if (forward.sqrMagnitude <= Mathf.Epsilon || directionToCenter.sqrMagnitude <= Mathf.Epsilon)
        {
            transform.localScale = currentScale;
            return;
        }

        float angleOffset = Vector3.Angle(forward, directionToCenter);
        float circleRadius = CircleCenter.lossyScale.x;
        float rayLength = CalculateRayLength(angleOffset, circleRadius);

        SetRayLineLossyScaleX(rayLength);
        transform.localScale = currentScale;
    }

    private void SetRayLineLossyScaleX(float targetLossyScaleX)
    {
        Vector3 rayLineScale = RayLine.localScale;
        float currentLossyScaleX = RayLine.lossyScale.x;

        if (Mathf.Abs(currentLossyScaleX) <= Mathf.Epsilon)
        {
            rayLineScale.x = targetLossyScaleX;
        }
        else
        {
            rayLineScale.x *= targetLossyScaleX / currentLossyScaleX;
        }

        RayLine.localScale = rayLineScale;
    }
}
