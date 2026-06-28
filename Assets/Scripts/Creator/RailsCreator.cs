using UnityEngine;

public class RailsCreator : MonoBehaviour, IGroundTrapHandler
{
    public GameObject RayRailsPrefab;
    public GameObject RayPrefab;
    public Transform RailsParent;

    [InspectorButton]
    private void CreateRails()
    {
        if (RayRailsPrefab == null)
        {
            Debug.LogWarning("RayRailsPrefab is not assigned.", this);
            return;
        }

        Instantiate(RayRailsPrefab, transform.position, transform.rotation, RailsParent);
    }

    public void OnGroundTrapEvent()
    {
        CreateRails();
    }
}
