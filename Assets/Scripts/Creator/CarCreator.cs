using UnityEngine;

public class CarCreator : MonoBehaviour, IGroundTrapHandler
{
    public GameObject CarPrefab;
    public Transform CarParent;
    public Transform Plane;
    public float radiusOffset;
    public float yOffset;

    [InspectorButton]
    private void CreateCar()
    {
        if (CarPrefab == null)
        {
            Debug.LogWarning("CarPrefab is not assigned.", this);
            return;
        }

        if (Plane == null)
        {
            Debug.LogWarning("Plane is not assigned.", this);
            return;
        }

        Vector3 spawnPosition = GetSpawnPosition();
        Instantiate(CarPrefab, spawnPosition, CarPrefab.transform.rotation, CarParent);
    }

    public void OnGroundTrapEvent()
    {
        CreateCar();
    }

    private Vector3 GetSpawnPosition()
    {
        float radius = Plane.lossyScale.x/2;
        float spawnDistance = radius + radiusOffset;
        float angle = Random.Range(-Mathf.PI * 0.5f, Mathf.PI * 0.5f);
        Vector3 offset = Vector3.right * (Mathf.Cos(angle) * spawnDistance)
            + Vector3.forward * (Mathf.Sin(angle) * spawnDistance);
        Vector3 spawnPosition = Plane.position + offset;
        spawnPosition.y = Plane.position.y + yOffset;

        return spawnPosition;
    }
}
