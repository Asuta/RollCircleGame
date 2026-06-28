using System.Collections;
using UnityEngine;

public class SpikesCreator : MonoBehaviour, IGroundTrapHandler
{
    public GameObject SpikesPrefab;
    public Transform Plane;
    public Transform CreatorTransform;
    [SerializeField] private float minRadiusRate = 0.2f;
    [SerializeField] private float maxRadiusRate = 0.8f;

    [InspectorButton]
    private void CreateSpikes()
    {
        StartCoroutine(CreateSpikesWhenPositionAvailable());
    }

    private IEnumerator CreateSpikesWhenPositionAvailable()
    {
        if (SpikesPrefab == null)
        {
            Debug.LogWarning("SpikesPrefab is not assigned.", this);
            yield break;
        }

        if (Plane == null)
        {
            Debug.LogWarning("Plane is not assigned.", this);
            yield break;
        }

        while (true)
        {
            Vector3 spawnPosition = GetSpawnPosition();

            if (!HasSpikesAtPosition(spawnPosition))
            {
                CreateAtPosition(spawnPosition);
                yield break;
            }

            yield return null;
        }
    }

    public void OnGroundTrapEvent()
    {
        CreateSpikes();
    }

    private Vector3 GetSpawnPosition()
    {
        float radius = Plane.lossyScale.x * 0.5f;
        float minRate = Mathf.Min(minRadiusRate, maxRadiusRate);
        float maxRate = Mathf.Max(minRadiusRate, maxRadiusRate);
        float distance = radius * Random.Range(minRate, maxRate);
        Vector3 spawnPosition = Plane.position + Vector3.forward * distance;
        spawnPosition.y = GetPlaneSurfaceY(spawnPosition);

        return spawnPosition;
    }

    private void CreateAtPosition(Vector3 spawnPosition)
    {
        Quaternion spawnRotation = CreatorTransform != null ? CreatorTransform.rotation : SpikesPrefab.transform.rotation;
        Vector3 targetLossyScale = SpikesPrefab.transform.lossyScale;
        GameObject spikes = Instantiate(SpikesPrefab, spawnPosition, spawnRotation, CreatorTransform);
        SetLossyScale(spikes.transform, targetLossyScale);
    }

    private bool HasSpikesAtPosition(Vector3 position)
    {
        float checkRadius = Mathf.Abs(SpikesPrefab.transform.lossyScale.x);
        Collider[] colliders = Physics.OverlapSphere(position, checkRadius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);

        foreach (Collider collider in colliders)
        {
            if (collider == null)
                continue;

            if (IsExistingSpikes(collider.transform))
                return true;
        }

        return false;
    }

    private bool IsExistingSpikes(Transform hitTransform)
    {
        if (hitTransform == null)
            return false;

        if (hitTransform.GetComponentInParent<SpikesTrap>() != null)
            return true;

        return hitTransform.name.ToLower().Contains("spikes");
    }

    private float GetPlaneSurfaceY(Vector3 spawnPosition)
    {
        Vector3 rayStartPosition = new Vector3(spawnPosition.x, Plane.position.y + 5f, spawnPosition.z);
        RaycastHit[] hits = Physics.RaycastAll(rayStartPosition, Vector3.down);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform == Plane || hit.transform.IsChildOf(Plane))
                return hit.point.y;
        }

        return Plane.position.y;
    }

    private void SetLossyScale(Transform target, Vector3 targetLossyScale)
    {
        if (target == null)
            return;

        Transform parent = target.parent;
        if (parent == null)
        {
            target.localScale = targetLossyScale;
            return;
        }

        Vector3 parentLossyScale = parent.lossyScale;
        Vector3 localScale = target.localScale;
        localScale.x = Mathf.Abs(parentLossyScale.x) <= Mathf.Epsilon ? targetLossyScale.x : targetLossyScale.x / parentLossyScale.x;
        localScale.y = Mathf.Abs(parentLossyScale.y) <= Mathf.Epsilon ? targetLossyScale.y : targetLossyScale.y / parentLossyScale.y;
        localScale.z = Mathf.Abs(parentLossyScale.z) <= Mathf.Epsilon ? targetLossyScale.z : targetLossyScale.z / parentLossyScale.z;
        target.localScale = localScale;
    }
}
