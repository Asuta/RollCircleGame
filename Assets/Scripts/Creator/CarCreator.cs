using System.Collections;
using UnityEngine;

public class CarCreator : MonoBehaviour, IGroundTrapHandler
{
    public GameObject CarPrefab;
    public Transform CarParent;
    public Transform Plane;
    public float radiusOffset;
    public float yOffset;
    [SerializeField] private int spawnCount = 3;
    [SerializeField] private float spawnInterval = 1f;

    private Coroutine spawnCoroutine;

    [InspectorButton]
    private void CreateCar()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(CreateCarsRoutine());
    }

    private IEnumerator CreateCarsRoutine()
    {
        if (CarPrefab == null)
        {
            Debug.LogWarning("CarPrefab is not assigned.", this);
            yield break;
        }

        if (Plane == null)
        {
            Debug.LogWarning("Plane is not assigned.", this);
            yield break;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            yield return new WaitForSeconds(spawnInterval);
            CreateSingleCar();
        }

        spawnCoroutine = null;
    }

    private void CreateSingleCar()
    {
        Vector3 spawnPosition = GetSpawnPosition();
        GameObject car = Instantiate(CarPrefab, spawnPosition, CarPrefab.transform.rotation, CarParent);
        FacePlaneCenter(car.transform);
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

    private void FacePlaneCenter(Transform target)
    {
        if (target == null || Plane == null)
            return;

        Vector3 directionToCenter = Plane.position - target.position;
        directionToCenter.y = 0f;

        if (directionToCenter == Vector3.zero)
            return;

        target.rotation = Quaternion.LookRotation(directionToCenter, Vector3.up);
    }
}
