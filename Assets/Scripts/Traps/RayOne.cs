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
        SetRayLineActive(false);
        transform.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        SetRayLineActive(false);

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
            UpdateRayLineLength();
            SetRayLineActive(true);
            StartRayLineCountdown();
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
        UpdateRayLineLength();
        SetRayLineActive(true);
        StartRayLineCountdown();
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
        float rayLength = CalculateRayLength(angleOffset, circleRadius) * 0.5f;

        SetRayLineLossyScaleZ(rayLength);
        RayLine.position = transform.position + RayLine.forward * (rayLength * 0.5f);
        transform.localScale = currentScale;
    }

    private void SetRayLineLossyScaleZ(float targetLossyScaleZ)
    {
        Vector3 rayLineScale = RayLine.localScale;
        float currentLossyScaleZ = RayLine.lossyScale.z;

        if (Mathf.Abs(currentLossyScaleZ) <= Mathf.Epsilon)
        {
            rayLineScale.z = targetLossyScaleZ;
        }
        else
        {
            rayLineScale.z *= targetLossyScaleZ / currentLossyScaleZ;
        }

        RayLine.localScale = rayLineScale;
    }

    private void SetRayLineActive(bool isActive)
    {
        if (RayLine != null)
            RayLine.gameObject.SetActive(isActive);
    }

    private void StartRayLineCountdown()
    {
        if (RayLine == null)
            return;

        RayLine rayLine = RayLine.GetComponent<RayLine>();
        if (rayLine != null)
            rayLine.StartCreateRayCubeCountdown();
    }
}
