using UnityEngine;

public class RayTwo : MonoBehaviour
{
    [SerializeField] private float rotateSpeedOne = 45f;
    [SerializeField] private float rotateSpeedTwo = 90f;
    [SerializeField] private float rotateSpeedThree = 135f;

    private float rotateSpeed;

    private void Start()
    {
        float[] speeds = { rotateSpeedOne, rotateSpeedTwo, rotateSpeedThree };
        rotateSpeed = speeds[Random.Range(0, speeds.Length)];
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
    }
}
