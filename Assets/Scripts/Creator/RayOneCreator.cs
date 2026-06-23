using System.Collections.Generic;
using UnityEngine;

public class RayOneCreator : MonoBehaviour
{
    public List<Transform> transformList;
    public GameObject RayOnePrefab;
    public Transform RayOneParent;
    public Transform RayOneCenter;
    [SerializeField] private float randomYawOffsetRange = 45f;

    public void Create()
    {
        if (RayOnePrefab == null)
        {
            Debug.LogWarning("RayOnePrefab is not assigned.", this);
            return;
        }

        Transform spawnPoint = GetRandomSpawnPoint();
        if (spawnPoint == null)
        {
            Debug.LogWarning("No valid spawn point found in transformList.", this);
            return;
        }

        GameObject rayOne = Instantiate(RayOnePrefab, spawnPoint.position, spawnPoint.rotation, RayOneParent);
        FaceCenter(rayOne.transform);
    }



    [InspectorButton("Create RayOne")]
    public void CreateRayOne()
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

    private void FaceCenter(Transform target)
    {
        if (target == null || RayOneCenter == null)
            return;

        Vector3 directionToCenter = RayOneCenter.position - target.position;
        directionToCenter.y = 0f;

        if (directionToCenter == Vector3.zero)
            return;

        Quaternion centerRotation = Quaternion.LookRotation(directionToCenter, Vector3.up);
        float randomYawOffset = Random.Range(-randomYawOffsetRange, randomYawOffsetRange);
        target.rotation = centerRotation * Quaternion.Euler(0f, randomYawOffset, 0f);
    }
}
