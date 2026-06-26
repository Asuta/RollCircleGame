using System.Collections;
using UnityEngine;

public class SweepLaser : MonoBehaviour
{
    [SerializeField] private float scaleInDuration = 2f;
    [SerializeField] private float createSweepLaserDelay = 1f;

    private Vector3 initialScale;
    private Coroutine scaleInCoroutine;
    public GameObject SweepLaserPrefab;
    public Transform SweepLaserParent;
    public Transform SweepLaserTransform;

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
            yield return CreateSweepLaserAfterDelay();
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
        yield return CreateSweepLaserAfterDelay();
        scaleInCoroutine = null;
    }

    private IEnumerator CreateSweepLaserAfterDelay()
    {
        if (createSweepLaserDelay > 0f)
            yield return new WaitForSeconds(createSweepLaserDelay);

        if (SweepLaserPrefab == null)
            yield break;

        Transform spawnTransform = SweepLaserTransform != null ? SweepLaserTransform : transform;
        Transform parent = SweepLaserParent != null ? SweepLaserParent : transform.parent;

        Instantiate(SweepLaserPrefab, spawnTransform.position, spawnTransform.rotation, parent);
    }
}
