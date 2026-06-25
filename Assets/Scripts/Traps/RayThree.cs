using UnityEngine;
using System.Collections.Generic;

public class RayThree : MonoBehaviour
{
    public List<int> intList = new List<int>();

    private float rotateSpeed;

    private void Start()
    {
        if (intList == null || intList.Count == 0)
            return;

        rotateSpeed = intList[Random.Range(0, intList.Count)];
        rotateSpeed *= Random.value < 0.5f ? -1f : 1f;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
    }
}
