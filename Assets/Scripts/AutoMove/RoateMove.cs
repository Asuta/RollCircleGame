using UnityEngine;

public class RoateMove : MonoBehaviour
{
    public Transform target;
    public float speed = 90f;

    private void Update()
    {
        if (target == null)
            return;

        transform.RotateAround(target.position, Vector3.up, speed * Time.deltaTime);
    }

    [InspectorButton]
    public void LogTest()
    {
        Debug.Log("RoateMove LogTest called");
    }
}
