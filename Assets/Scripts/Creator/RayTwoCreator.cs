using UnityEngine;

public class RayTwoCreator : MonoBehaviour
{
    public GameObject RayTwoPrefab;
    public Transform CircleCenter;
    public Transform CreatTrans;

    [InspectorButton]
    private void CreateRayTwo()
    {
        if (RayTwoPrefab == null)
        {
            Debug.LogWarning("RayTwoPrefab is not assigned.", this);
            return;
        }

        if (CreatTrans == null)
        {
            Debug.LogWarning("CreatTrans is not assigned.", this);
            return;
        }

        GameObject rayTwo = Instantiate(RayTwoPrefab, CreatTrans.position, CreatTrans.rotation);
        Vector3 eulerAngles = rayTwo.transform.eulerAngles;
        eulerAngles.y = Random.Range(0f, 360f);
        rayTwo.transform.eulerAngles = eulerAngles;
    }
}
