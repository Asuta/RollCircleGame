using UnityEngine;

public class RayThreeCreator : MonoBehaviour, IGroundTrapHandler
{
    public GameObject RayThreePrefab;
    public Transform RayThreeParent;
    public Transform Plane;

    [SerializeField] private float minRadiusRate = 0.2f;
    [SerializeField] private float maxRadiusRate = 0.8f;

    [InspectorButton]
    private void CreateRayThree()
    {
        if (RayThreePrefab == null)
        {
            Debug.LogWarning("RayThreePrefab is not assigned.", this);
            return;
        }

        if (RayThreeParent == null)
        {
            Debug.LogWarning("RayThreeParent is not assigned.", this);
            return;
        }

        if (Plane == null)
        {
            Debug.LogWarning("Plane is not assigned.", this);
            return;
        }

        Vector3 spawnPosition = GetRandomPositionOnPlane();
        Instantiate(RayThreePrefab, spawnPosition, RayThreePrefab.transform.rotation, RayThreeParent);
    }

    public void OnGroundTrapEvent()
    {
        CreateRayThree();
    }

    private Vector3 GetRandomPositionOnPlane()
    {
        float planeRadius = Plane.lossyScale.x;
        float minRadius = planeRadius * Mathf.Min(minRadiusRate, maxRadiusRate);
        float maxRadius = planeRadius * Mathf.Max(minRadiusRate, maxRadiusRate);
        float radius = Random.Range(minRadius, maxRadius) * 0.5f;
        float angle = Random.Range(-Mathf.PI * 0.5f, Mathf.PI * 0.5f);

        Vector3 center = Plane.position;
        Vector3 offset = Vector3.right * (Mathf.Cos(angle) * radius)
            + Vector3.forward * (Mathf.Sin(angle) * radius);
        Vector3 spawnPosition = center + offset;
        spawnPosition.y = RayThreePrefab.transform.position.y;

        return spawnPosition;
    }
}
