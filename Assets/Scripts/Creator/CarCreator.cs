using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCreator : MonoBehaviour, IGroundTrapHandler
{
    public GameObject CarPrefab;
    public GameObject GolinePrefab;
    public Transform CarParent;
    public Transform Plane;
    public float radiusOffset;
    public float yOffset;
    [SerializeField] private int spawnCount = 3;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float carMoveDelay = 2f;
    [SerializeField] private float carMoveSpeed = 5f;
    [SerializeField] private float carMoveDistanceOffset = 0f;

    private Coroutine spawnCoroutine;
    private readonly Dictionary<Car, SpawnedCar> spawnedCars = new Dictionary<Car, SpawnedCar>();

    private class SpawnedCar
    {
        public GameObject CarObject;
        public GameObject GolineObject;
    }

    private void OnEnable()
    {
        Car.HitPlayer += OnCarHitPlayer;
    }

    private void OnDisable()
    {
        Car.HitPlayer -= OnCarHitPlayer;
    }

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
            yield return CreateSingleCarWhenPositionAvailable();
        }

        spawnCoroutine = null;
    }

    private IEnumerator CreateSingleCarWhenPositionAvailable()
    {
        while (true)
        {
            Vector3 spawnPosition = GetSpawnPosition();

            if (!HasCarAtPosition(spawnPosition))
            {
                GameObject car = Instantiate(CarPrefab, spawnPosition, CarPrefab.transform.rotation, CarParent);
                FacePlaneCenter(car.transform);
                GameObject goLine = CreateGoline(car.transform);
                Car carComponent = car.GetComponentInChildren<Car>();
                RegisterSpawnedCar(carComponent, car, goLine);
                StartCoroutine(MoveCarAfterDelay(carComponent, car, goLine));
                yield break;
            }

            yield return null;
        }
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

    private GameObject CreateGoline(Transform carTransform)
    {
        if (GolinePrefab == null || Plane == null || carTransform == null)
            return null;

        Vector3 goLinePosition = Plane.position;
        goLinePosition.y = Plane.position.y + yOffset;
        GameObject goLine = Instantiate(GolinePrefab, goLinePosition, carTransform.rotation, CarParent);
        SetGolineLength(goLine.transform, Plane.lossyScale.x);
        return goLine;
    }

    private IEnumerator MoveCarAfterDelay(Car carComponent, GameObject car, GameObject goLine)
    {
        yield return new WaitForSeconds(carMoveDelay);

        if (car == null || Plane == null)
            yield break;

        Transform carTransform = car.transform;
        Vector3 moveDirection = carTransform.forward;
        moveDirection.y = 0f;

        if (moveDirection == Vector3.zero)
            yield break;

        moveDirection.Normalize();

        float targetDistance = Plane.lossyScale.x + carMoveDistanceOffset;
        float movedDistance = 0f;

        while (movedDistance < targetDistance)
        {
            if (car == null)
                yield break;

            float moveDistance = Mathf.Min(carMoveSpeed * Time.deltaTime, targetDistance - movedDistance);
            carTransform.position += moveDirection * moveDistance;
            movedDistance += moveDistance;
            yield return null;
        }

        if (carComponent != null)
        {
            DestroyCarAndGoline(carComponent);
            yield break;
        }

        if (goLine != null)
            Destroy(goLine);

        if (car != null)
            Destroy(car);
    }

    private void RegisterSpawnedCar(Car carComponent, GameObject carObject, GameObject goLineObject)
    {
        if (carComponent == null)
            return;

        spawnedCars[carComponent] = new SpawnedCar
        {
            CarObject = carObject,
            GolineObject = goLineObject
        };
    }

    private void OnCarHitPlayer(Car car)
    {
        DestroyCarAndGoline(car);
    }

    private void DestroyCarAndGoline(Car car)
    {
        if (car == null)
            return;

        if (spawnedCars.TryGetValue(car, out SpawnedCar spawnedCar))
        {
            if (spawnedCar.GolineObject != null)
                Destroy(spawnedCar.GolineObject);

            if (spawnedCar.CarObject != null)
                Destroy(spawnedCar.CarObject);

            spawnedCars.Remove(car);
            return;
        }

        return;
    }

    private void SetGolineLength(Transform goLineTransform, float targetLength)
    {
        if (goLineTransform == null)
            return;

        Vector3 localScale = goLineTransform.localScale;
        float currentLossyScaleZ = goLineTransform.lossyScale.z;

        if (Mathf.Abs(currentLossyScaleZ) <= Mathf.Epsilon)
            localScale.z = targetLength;
        else
            localScale.z *= targetLength / currentLossyScaleZ;

        goLineTransform.localScale = localScale;
    }

    private bool HasCarAtPosition(Vector3 position)
    {
        float checkRadius = GetCarCheckRadius();
        Collider[] colliders = Physics.OverlapSphere(position, checkRadius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);

        foreach (Collider collider in colliders)
        {
            if (collider == null)
                continue;

            if (IsExistingCar(collider.transform))
                return true;
        }

        return false;
    }

    private bool IsExistingCar(Transform hitTransform)
    {
        if (hitTransform == null)
            return false;

        return hitTransform.name.ToLower().Contains("car");
    }

    private float GetCarCheckRadius()
    {
        return Mathf.Abs(CarPrefab.transform.lossyScale.x);
    }
}
