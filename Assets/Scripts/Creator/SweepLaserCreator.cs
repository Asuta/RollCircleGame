using UnityEngine;

public class SweepLaserCreator : MonoBehaviour, IGroundTrapHandler
{
    public GameObject SweepLaserPrefabMock;
    public GameObject SweepLaserPrefab;
    public Transform planeTransform;
    public Transform SweepLaserParent;
    public float radiusOffset = 5f;

    private void Start()
    {
        CreateSweepLaserPrefabMocks();
    }

    [InspectorButton]
    private void CreateSweepLaser()
    {
        if (SweepLaserPrefab == null)
        {
            Debug.LogWarning("SweepLaserPrefab is not assigned.", this);
            return;
        }

        if (planeTransform == null)
        {
            Debug.LogWarning("planeTransform is not assigned.", this);
            return;
        }

        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject sweepLaser = Instantiate(SweepLaserPrefab, spawnPosition, SweepLaserPrefab.transform.rotation, SweepLaserParent);
        FacePlaneCenter(sweepLaser.transform);
    }

    public void OnGroundTrapEvent()
    {
        CreateSweepLaser();
    }

    private void CreateSweepLaserPrefabMocks()
    {
        if (SweepLaserPrefabMock == null || planeTransform == null)
            return;

        Vector3[] directions =
        {
            Vector3.back,
            Vector3.forward,
            Vector3.right
        };

        foreach (Vector3 direction in directions)
        {
            Vector3 spawnPosition = GetSpawnPosition(direction, SweepLaserPrefabMock);
            GameObject sweepLaserMock = Instantiate(SweepLaserPrefabMock, spawnPosition, SweepLaserPrefabMock.transform.rotation, SweepLaserParent);
            FacePlaneCenter(sweepLaserMock.transform);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3[] directions =
        {
            Vector3.back,
            Vector3.forward,
            Vector3.right
        };

        Vector3 direction = directions[Random.Range(0, directions.Length)];
        return GetSpawnPosition(direction, SweepLaserPrefab);
    }

    private Vector3 GetSpawnPosition(Vector3 direction, GameObject prefab)
    {
        direction.y = 0f;
        direction.Normalize();

        float radius = planeTransform.lossyScale.x;
        float spawnDistance = radius * 0.5f + radiusOffset;
        Vector3 spawnPosition = planeTransform.position + direction * spawnDistance;
        spawnPosition.y = prefab.transform.position.y;

        return spawnPosition;
    }

    private void FacePlaneCenter(Transform target)
    {
        if (target == null || planeTransform == null)
            return;

        Vector3 directionToCenter = planeTransform.position - target.position;
        directionToCenter.y = 0f;

        if (directionToCenter == Vector3.zero)
            return;

        target.rotation = Quaternion.LookRotation(directionToCenter, Vector3.up);
    }
}
