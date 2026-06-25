using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayOneCreator : MonoBehaviour, IGroundTrapHandler
{
    public List<Transform> transformList;
    public GameObject RayOnePrefab;
    public Transform RayOneParent;
    public Transform RayOneCenter;
    [SerializeField] private float minRandomYawOffset = 15f;
    [SerializeField] private float maxRandomYawOffset = 45f;
    [SerializeField] private float obstacleCheckDistance = 1f;

    public void Create()
    {
        StartCoroutine(CreateWhenPositionAvailable());
    }

    private IEnumerator CreateWhenPositionAvailable()
    {
        if (RayOnePrefab == null)
        {
            Debug.LogWarning("RayOnePrefab is not assigned.", this);
            yield break;
        }

        while (true)
        {
            Transform spawnPoint = GetRandomSpawnPoint();
            if (spawnPoint == null)
            {
                Debug.LogWarning("No valid spawn point found in transformList.", this);
                yield break;
            }

            if (!HasRayOneAtPosition(spawnPoint.position))
            {
                CreateAtSpawnPoint(spawnPoint);
                yield break;
            }

            yield return null;
        }
    }

    private void CreateAtSpawnPoint(Transform spawnPoint)
    {
        GameObject rayOne = Instantiate(RayOnePrefab, spawnPoint.position, spawnPoint.rotation, RayOneParent);
        SetCircleCenter(rayOne);
        FaceCenter(rayOne.transform);
    }



    [InspectorButton("Create RayOne")]
    public void CreateRayOne()
    {
        Create();
    }

    public void OnGroundTrapEvent()
    {
        Create();
    }

    private Transform GetRandomSpawnPoint()
    {
        if (transformList == null || transformList.Count == 0)
            return null;

        int startIndex = Random.Range(0, transformList.Count);

        for (int i = 0; i < transformList.Count; i++)
        {
            int index = (startIndex + i) % transformList.Count;
            if (transformList[index] != null)
                return transformList[index];
        }

        return null;
    }

    private bool HasRayOneAtPosition(Vector3 position)
    {
        float checkRadius = Mathf.Max(0.1f, Mathf.Abs(RayOnePrefab.transform.lossyScale.y) * 2f);
        Vector3 castOrigin = position + Vector3.up * (checkRadius + obstacleCheckDistance);
        RaycastHit[] hits = Physics.SphereCastAll(
            castOrigin,
            checkRadius,
            Vector3.down,
            obstacleCheckDistance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide);

        foreach (RaycastHit hit in hits)
        {
            if (IsRayOne(hit.collider))
                return true;
        }

        return false;
    }

    private bool IsRayOne(Collider hitCollider)
    {
        if (hitCollider == null)
            return false;

        return hitCollider.GetComponentInParent<RayOne>() != null;
    }

    private void FaceCenter(Transform target)
    {
        if (target == null || RayOneCenter == null)
            return;

        Vector3 directionToCenter = RayOneCenter.position - target.position;
        directionToCenter.y = 0f;

        if (directionToCenter == Vector3.zero)
            return;

        Quaternion centerRotation = Quaternion.LookRotation(directionToCenter, Vector3.up);
        float minOffset = Mathf.Min(minRandomYawOffset, maxRandomYawOffset);
        float maxOffset = Mathf.Max(minRandomYawOffset, maxRandomYawOffset);
        float randomYawOffset = Random.Range(minOffset, maxOffset);
        randomYawOffset *= Random.value < 0.5f ? -1f : 1f;
        target.rotation = centerRotation * Quaternion.Euler(0f, randomYawOffset, 0f);
    }

    private void SetCircleCenter(GameObject rayOne)
    {
        RayOne rayOneComponent = rayOne.GetComponent<RayOne>();
        if (rayOneComponent == null)
        {
            Debug.LogWarning("RayOne component is not found on created object.", rayOne);
            return;
        }

        rayOneComponent.CircleCenter = RayOneCenter;
    }
}
