using UnityEngine;

public class FallJoyCreator : MonoBehaviour, IGroundTrapHandler
{
    public GameObject FallJoyPrefab;
    public Transform FallJoyParent;
    public Transform TargetPlayer;
    public GameObject ShadowPrefab;
    [SerializeField] private float headOffset = 2f;

    [InspectorButton]
    private void CreateFallJoy()
    {
        if (FallJoyPrefab == null)
        {
            Debug.LogWarning("FallJoyPrefab is not assigned.", this);
            return;
        }

        Vector3 spawnPosition = GetSpawnPosition();
        GameObject fallJoy = Instantiate(FallJoyPrefab, spawnPosition, transform.rotation, FallJoyParent);
        Transform shadowTransform = UpdateShadowPosition(fallJoy.transform);

        JoyCreator joyCreator = fallJoy.GetComponent<JoyCreator>();
        if (joyCreator != null)
        {
            joyCreator.SetTargetPlayer(TargetPlayer);
            joyCreator.ShadowTransform = shadowTransform;
            joyCreator.InitializeShadow();
        }
    }

    public void OnGroundTrapEvent()
    {
        CreateFallJoy();
    }

    public void SetTargetPlayer(Transform targetPlayer)
    {
        TargetPlayer = targetPlayer;
    }

    private Vector3 GetSpawnPosition()
    {
        if (TargetPlayer == null)
            return transform.position;

        return TargetPlayer.position + Vector3.up * headOffset;
    }

    private Transform UpdateShadowPosition(Transform fallJoyTransform)
    {
        if (ShadowPrefab == null || fallJoyTransform == null)
            return null;

        RaycastHit[] hits = Physics.RaycastAll(fallJoyTransform.position, Vector3.down);
        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.CompareTag("Plane"))
                continue;

            Transform shadowTransform = FindShadowTransform(fallJoyTransform);
            if (shadowTransform != null)
            {
                shadowTransform.position = hit.point;
                return shadowTransform;
            }

            return null;
        }

        return null;
    }

    private Transform FindShadowTransform(Transform fallJoyTransform)
    {
        if (ShadowPrefab == null || fallJoyTransform == null)
            return null;

        if (ShadowPrefab.transform.IsChildOf(fallJoyTransform))
            return ShadowPrefab.transform;

        Transform[] childTransforms = fallJoyTransform.GetComponentsInChildren<Transform>(true);
        foreach (Transform childTransform in childTransforms)
        {
            if (childTransform.name == ShadowPrefab.name)
                return childTransform;
        }

        return null;
    }
}
