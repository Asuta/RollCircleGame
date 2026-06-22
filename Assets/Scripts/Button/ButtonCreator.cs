using System.Collections;
using UnityEngine;

public class ButtonCreator : MonoBehaviour
{
    public Transform buttonParent;
    public GameObject buttonPrefab;
    // list of transforms to spawn buttons on
    public Transform[] spawnPoints;

    [SerializeField] private float minSpawnInterval = 5f;
    [SerializeField] private float maxSpawnInterval = 10f;
    [SerializeField] private float buttonCheckDistance = 1f;
    [SerializeField] private float spawnedButtonLocalY = -0.05337987f;

    private int nextSpawnPointIndex;
    private Coroutine spawnCoroutine;

    private void OnEnable()
    {
        spawnCoroutine = StartCoroutine(SpawnButtonsRoutine());
    }

    private void OnDisable()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnButtonsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
            yield return FindSpawnPointAndCreateButton();
        }
    }

    private IEnumerator FindSpawnPointAndCreateButton()
    {
        if (buttonPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            yield break;

        while (true)
        {
            Transform spawnPoint = spawnPoints[nextSpawnPointIndex];
            nextSpawnPointIndex = (nextSpawnPointIndex + 1) % spawnPoints.Length;

            if (spawnPoint != null && !HasButtonAtPosition(spawnPoint.position))
            {
                CreateButton(spawnPoint);
                yield break;
            }

            yield return null;
        }
    }

    private bool HasButtonAtPosition(Vector3 position)
    {
        float checkRadius = Mathf.Abs(buttonPrefab.transform.lossyScale.y) * 2f;
        Vector3 castOrigin = position + Vector3.up * (checkRadius + buttonCheckDistance);
        RaycastHit[] hits = Physics.SphereCastAll(
            castOrigin,
            checkRadius,
            Vector3.down,
            buttonCheckDistance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide);

        foreach (RaycastHit hit in hits)
        {
            if (IsButton(hit.collider))
                return true;
        }

        return false;
    }

    private void CreateButton(Transform spawnPoint)
    {
        GameObject button = Instantiate(buttonPrefab, spawnPoint.position, spawnPoint.rotation, buttonParent);
        Vector3 localPosition = button.transform.localPosition;
        localPosition.y = spawnedButtonLocalY;
        button.transform.localPosition = localPosition;
    }

    private bool IsButton(Collider hitCollider)
    {
        if (hitCollider == null)
            return false;

        return hitCollider.CompareTag("button") || hitCollider.GetComponentInParent<ButtonChild>()?.CompareTag("button") == true;
    }
}
