using System.Collections.Generic;
using UnityEngine;

public interface IGroundTrapHandler
{
    void OnGroundTrapEvent();
}

public class GroundTrapEvent : MonoBehaviour
{
    [SerializeField] private List<WeightedGroundTrapHandler> handlers = new List<WeightedGroundTrapHandler>();

    [InspectorButton]
    public void TriggerEvent()
    {
        float totalWeight = GetTotalWeight();
        if (totalWeight <= 0f)
            return;

        float randomValue = Random.Range(0f, totalWeight);
        float accumulatedWeight = 0f;

        foreach (WeightedGroundTrapHandler handler in handlers)
        {
            if (handler == null || handler.Weight <= 0f)
                continue;

            accumulatedWeight += handler.Weight;
            if (randomValue > accumulatedWeight)
                continue;

            handler.Invoke();
            return;
        }
    }

    private float GetTotalWeight()
    {
        float totalWeight = 0f;

        foreach (WeightedGroundTrapHandler handler in handlers)
        {
            if (handler != null && handler.Weight > 0f)
                totalWeight += handler.Weight;
        }

        return totalWeight;
    }
}

[System.Serializable]
public class WeightedGroundTrapHandler
{
    public MonoBehaviour Target;
    public float Weight = 1f;

    public void Invoke()
    {
        IGroundTrapHandler handler = Target as IGroundTrapHandler;
        if (handler == null)
            return;

        handler.OnGroundTrapEvent();
    }
}
