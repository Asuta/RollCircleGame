using System.Collections;
using UnityEngine;

public class RayOne : MonoBehaviour
{
    [SerializeField] private float scaleInDuration = 0.5f;

    private Vector3 initialScale;
    private Coroutine scaleInCoroutine;

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
}
