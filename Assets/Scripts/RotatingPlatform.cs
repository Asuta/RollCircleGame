using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    public float initialRotateSpeed = 0f; // 初始每秒旋转角度，正负决定方向
    public float targetRotateSpeed = 45f; // 最终每秒旋转角度
    public float speedChangeDuration = 10f; // 从初始速度线性变化到最终速度所需时间

    private float currentRotateSpeed;
    private float elapsedTime;

    private void Start()
    {
        currentRotateSpeed = initialRotateSpeed;
        elapsedTime = 0f;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        float speedChangeProgress = speedChangeDuration <= 0f ? 1f : Mathf.Clamp01(elapsedTime / speedChangeDuration);
        currentRotateSpeed = Mathf.Lerp(initialRotateSpeed, targetRotateSpeed, speedChangeProgress);

        transform.Rotate(Vector3.up, currentRotateSpeed * Time.deltaTime, Space.World);
    }
}
