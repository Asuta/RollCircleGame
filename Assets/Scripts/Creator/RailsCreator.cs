using System.Collections.Generic;
using UnityEngine;

public class RailsCreator : MonoBehaviour, IGroundTrapHandler
{
    public GameObject RayPrefab;
    public Transform RailsParent;
    public Transform Plane;
    [SerializeField] private float spacingA = 0.4f;
    [SerializeField] private float spacingB = 0.8f;
    [SerializeField] private int minWideSpacingCount = 2;
    [SerializeField] private int maxWideSpacingCount = 5;
    [SerializeField] private Vector3 rayLocalScale = new Vector3(0.1f, 2f, 0.1f);

    [InspectorButton]
    private void CreateRails()
    {
        if (RayPrefab == null)
        {
            Debug.LogWarning("RayPrefab is not assigned.", this);
            return;
        }

        if (Plane == null)
        {
            Debug.LogWarning("Plane is not assigned.", this);
            return;
        }

        Vector3 spawnPosition = Plane.position;
        Quaternion spawnRotation = GetRailsRotation();
        GameObject rails = new GameObject("RayRails");
        rails.transform.SetParent(RailsParent);
        rails.transform.position = spawnPosition;
        rails.transform.rotation = spawnRotation;
        CreateRayPrefabs(rails.transform);
    }

    public void OnGroundTrapEvent()
    {
        CreateRails();
    }

    private Quaternion GetRailsRotation()
    {
        return Quaternion.LookRotation(Vector3.forward, Vector3.up);
    }

    private void CreateRayPrefabs(Transform railsTransform)
    {
        if (railsTransform == null)
            return;

        float radius = Plane.lossyScale.x * 0.5f;
        float currentDistance = 0f;
        List<float> spacingList = CreateSpacingList(radius);

        CreateRayPrefab(railsTransform, currentDistance);

        foreach (float spacing in spacingList)
        {
            currentDistance += spacing;
            CreateRayPrefab(railsTransform, currentDistance);
        }
    }

    private List<float> CreateSpacingList(float radius)
    {
        float narrowSpacing = Mathf.Max(0.01f, Mathf.Min(spacingA, spacingB));
        float wideSpacing = Mathf.Max(0.01f, Mathf.Max(spacingA, spacingB));
        int wideSpacingCount = GetRandomWideSpacingCount(radius, wideSpacing);
        List<float> spacingList = new List<float>();
        float totalDistance = 0f;

        for (int i = 0; i < wideSpacingCount; i++)
        {
            spacingList.Add(wideSpacing);
            totalDistance += wideSpacing;
        }

        while (totalDistance + narrowSpacing <= radius)
        {
            spacingList.Add(narrowSpacing);
            totalDistance += narrowSpacing;
        }

        ShuffleSpacingList(spacingList);
        return spacingList;
    }

    private int GetRandomWideSpacingCount(float radius, float wideSpacing)
    {
        int minCount = Mathf.Max(0, Mathf.Min(minWideSpacingCount, maxWideSpacingCount));
        int maxCount = Mathf.Max(0, Mathf.Max(minWideSpacingCount, maxWideSpacingCount));
        int possibleMaxCount = Mathf.FloorToInt(radius / wideSpacing);
        maxCount = Mathf.Min(maxCount, possibleMaxCount);
        minCount = Mathf.Min(minCount, maxCount);

        return Random.Range(minCount, maxCount + 1);
    }

    private void ShuffleSpacingList(List<float> spacingList)
    {
        for (int i = spacingList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            float temp = spacingList[i];
            spacingList[i] = spacingList[randomIndex];
            spacingList[randomIndex] = temp;
        }
    }

    private void CreateRayPrefab(Transform railsTransform, float distance)
    {
        Vector3 localPosition = Vector3.forward * distance;
        GameObject ray = Instantiate(RayPrefab, railsTransform);
        ray.transform.localPosition = localPosition;
        ray.transform.localRotation = Quaternion.identity;
        ray.transform.localScale = rayLocalScale;
    }
}
