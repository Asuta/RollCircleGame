using System.Collections;
using UnityEngine;

public class RayLine : MonoBehaviour
{
    public GameObject RayCube;
    [SerializeField] private float createRayCubeDelay = 1f;

    private Coroutine createRayCubeCoroutine;

    public void StartCreateRayCubeCountdown()
    {
        if (createRayCubeCoroutine != null)
            StopCoroutine(createRayCubeCoroutine);

        createRayCubeCoroutine = StartCoroutine(CreateRayCubeAfterDelay());
    }

    private void OnDisable()
    {
        if (createRayCubeCoroutine != null)
        {
            StopCoroutine(createRayCubeCoroutine);
            createRayCubeCoroutine = null;
        }
    }

    private IEnumerator CreateRayCubeAfterDelay()
    {
        yield return new WaitForSeconds(createRayCubeDelay);

        if (RayCube == null)
        {
            Debug.LogWarning("RayCube is not assigned.", this);
            createRayCubeCoroutine = null;
            yield break;
        }

        GameObject rayCube = Instantiate(RayCube, transform.position, transform.rotation, transform.parent);
        rayCube.transform.localScale = transform.localScale;

        createRayCubeCoroutine = null;
        Destroy(gameObject);
    }
}
