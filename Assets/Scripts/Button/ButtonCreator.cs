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
    [SerializeField] private int maxButtonCount = 20;

    private int nextSpawnPointIndex;
    private Coroutine spawnCoroutine;
    private Coroutine forceSpawnCoroutine;

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

        if (forceSpawnCoroutine != null)
        {
            StopCoroutine(forceSpawnCoroutine);
            forceSpawnCoroutine = null;
        }
    }

    private IEnumerator SpawnButtonsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));

            if (IsButtonCountFull())
                continue;

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

    [InspectorButton]
    private void ForceCreateButton()
    {
        if (buttonPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return;

        if (forceSpawnCoroutine != null)
            StopCoroutine(forceSpawnCoroutine);

        forceSpawnCoroutine = StartCoroutine(ForceCreateButtonRoutine());
    }

    private IEnumerator ForceCreateButtonRoutine()
    {
        yield return FindSpawnPointAndCreateButton();
        forceSpawnCoroutine = null;
    }

    private bool HasButtonAtPosition(Vector3 position)
    {
        float checkRadius = Mathf.Abs(buttonPrefab.transform.lossyScale.y) * 2f;
        Collider[] overlapColliders = Physics.OverlapSphere(
            position,
            checkRadius,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide);

        foreach (Collider overlapCollider in overlapColliders)
        {
            if (IsBlockedSpawnCollider(overlapCollider))
                return true;
        }

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
            if (IsBlockedSpawnCollider(hit.collider))
                return true;
        }

        return false;
    }

    private bool IsButtonCountFull()
    {
        if (maxButtonCount <= 0)
            return true;

        return CountButtonsInParent() >= maxButtonCount;
    }

    private int CountButtonsInParent()
    {
        if (buttonParent == null)
            return 0;

        int buttonCount = 0;

        for (int i = 0; i < buttonParent.childCount; i++)
        {
            Transform child = buttonParent.GetChild(i);
            if (child.CompareTag("button") || child.GetComponentInChildren<ButtonChild>(true) != null)
                buttonCount++;
        }

        return buttonCount;
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

    private bool IsBlockedSpawnCollider(Collider hitCollider)
    {
        return IsButton(hitCollider) || IsBaseRayCube(hitCollider);
    }

    private bool IsBaseRayCube(Collider hitCollider)
    {
        if (hitCollider == null)
            return false;

        Transform current = hitCollider.transform;
        while (current != null)
        {
            if (current.name.Contains("BaseRayCube"))
                return true;

            current = current.parent;
        }

        return false;
    }
}
