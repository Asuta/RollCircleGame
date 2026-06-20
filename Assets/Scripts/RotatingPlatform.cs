using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    public float rotateSpeed = 45f; // 每秒旋转角度，正负决定方向

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
    }
}