using System.Collections;
using UnityEngine;

public class SpikesTrap : MonoBehaviour
{
    public Transform SpikesOut;
    public Transform SpikesIn;
    [SerializeField] private float showDuration = 1f;
    [SerializeField] private float downLocalY = -1f;
    [SerializeField] private float moveLerpSpeed = 8f;
    [SerializeField] private float lifeTime = 6f;

    private Coroutine switchCoroutine;
    private Coroutine destroyCoroutine;
    private Vector3 spikesOutUpLocalPosition;
    private float targetLocalY;

    private void Awake()
    {
        if (SpikesOut == null)
            return;

        if (SpikesIn != null)
            SpikesIn.gameObject.SetActive(false);

        spikesOutUpLocalPosition = SpikesOut.localPosition;
        targetLocalY = spikesOutUpLocalPosition.y;
    }

    private void OnEnable()
    {
        switchCoroutine = StartCoroutine(SwitchSpikesRoutine());
        destroyCoroutine = StartCoroutine(DestroyAfterLifeTime());
    }

    private void OnDisable()
    {
        if (switchCoroutine != null)
        {
            StopCoroutine(switchCoroutine);
            switchCoroutine = null;
        }

        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
            destroyCoroutine = null;
        }
    }

    private IEnumerator SwitchSpikesRoutine()
    {
        if (SpikesOut == null)
            yield break;

        SpikesOut.gameObject.SetActive(true);
        if (SpikesIn != null)
            SpikesIn.gameObject.SetActive(false);

        SetSpikesOutLocalY(spikesOutUpLocalPosition.y);
        targetLocalY = spikesOutUpLocalPosition.y;

        float elapsedTime = 0f;
        bool isRaised = true;

        while (true)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= showDuration)
            {
                elapsedTime = 0f;
                isRaised = !isRaised;
                targetLocalY = isRaised ? spikesOutUpLocalPosition.y : downLocalY;
            }

            MoveSpikesOutToTargetY();
            yield return null;
        }
    }

    private void MoveSpikesOutToTargetY()
    {
        if (SpikesOut == null)
            return;

        Vector3 localPosition = SpikesOut.localPosition;
        if (moveLerpSpeed <= 0f)
        {
            localPosition.y = targetLocalY;
        }
        else
        {
            float lerpRate = Mathf.Clamp01(moveLerpSpeed * Time.deltaTime);
            localPosition.y = Mathf.Lerp(localPosition.y, targetLocalY, lerpRate);
        }

        SpikesOut.localPosition = localPosition;
    }

    private void SetSpikesOutLocalY(float localY)
    {
        if (SpikesOut == null)
            return;

        Vector3 localPosition = SpikesOut.localPosition;
        localPosition.y = localY;
        SpikesOut.localPosition = localPosition;
    }

    private IEnumerator DestroyAfterLifeTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
