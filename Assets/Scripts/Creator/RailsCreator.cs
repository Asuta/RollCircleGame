using UnityEngine;

public class RailsCreator : MonoBehaviour, IGroundTrapHandler
{
    public GameObject RayPrefab;
    public Transform RailsParent;
    public Transform Plane;
    [SerializeField] private int rayCount = 10;
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
        Quaternion spawnRotation = GetRandomRailsRotation();
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

    private Quaternion GetRandomRailsRotation()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

        return Quaternion.LookRotation(direction, Vector3.up);
    }

    private void CreateRayPrefabs(Transform railsTransform)
    {
        if (railsTransform == null)
            return;

        int safeRayCount = Mathf.Max(0, rayCount);
        float radius = Plane.lossyScale.x * 0.5f;
        float step = safeRayCount > 0 ? radius / safeRayCount : 0f;

        for (int i = 0; i < safeRayCount; i++)
        {
            Vector3 localPosition = Vector3.forward * ((i + 0.5f) * step);
            GameObject ray = Instantiate(RayPrefab, railsTransform);
            ray.transform.localPosition = localPosition;
            ray.transform.localRotation = Quaternion.identity;
            ray.transform.localScale = rayLocalScale;
        }
    }
}
