using UnityEngine;

public class FallJoyCreator : MonoBehaviour, IGroundTrapHandler
{
    public GameObject FallJoyPrefab;
    public Transform FallJoyParent;
    public Transform TargetPlayer;
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
        JoyCreator joyCreator = fallJoy.GetComponent<JoyCreator>();
        if (joyCreator != null)
            joyCreator.SetTargetPlayer(TargetPlayer);
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
}
