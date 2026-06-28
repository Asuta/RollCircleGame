using System.Collections;
using UnityEngine;

public class SpikesTrap : MonoBehaviour
{
    public Transform SpikesOut;
    public Transform SpikesIn;
    [SerializeField] private float showDuration = 1f;
    [SerializeField] private float lifeTime = 6f;

    private Coroutine switchCoroutine;
    private Coroutine destroyCoroutine;

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
        while (true)
        {
            SetSpikesActive(true);
            yield return new WaitForSeconds(showDuration);

            SetSpikesActive(false);
            yield return new WaitForSeconds(showDuration);
        }
    }

    private void SetSpikesActive(bool showOut)
    {
        if (SpikesOut != null)
            SpikesOut.gameObject.SetActive(showOut);

        if (SpikesIn != null)
            SpikesIn.gameObject.SetActive(!showOut);
    }

    private IEnumerator DestroyAfterLifeTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
