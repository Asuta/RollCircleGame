using System.Collections.Generic;
using UnityEngine;

public interface IGroundTrapHandler
{
    void OnGroundTrapEvent();
}

public class GroundTrapEvent : MonoBehaviour
{
    [SerializeField] private List<WeightedGroundTrapHandler> handlers = new List<WeightedGroundTrapHandler>();
    [SerializeField] private List<string> cooldownTargetNames = new List<string> { "RailsCreator" };
    [SerializeField] private float cooldownDuration = 3f;

    private readonly Dictionary<string, float> cooldownEndTimes = new Dictionary<string, float>();

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
            if (!CanUseHandler(handler))
                continue;

            accumulatedWeight += handler.Weight;
            if (randomValue > accumulatedWeight)
                continue;

            handler.Invoke();
            StartCooldownIfNeeded(handler);
            return;
        }
    }

    private float GetTotalWeight()
    {
        float totalWeight = 0f;

        foreach (WeightedGroundTrapHandler handler in handlers)
        {
            if (CanUseHandler(handler))
                totalWeight += handler.Weight;
        }

        return totalWeight;
    }

    private bool CanUseHandler(WeightedGroundTrapHandler handler)
    {
        if (handler == null || handler.Weight <= 0f)
            return false;

        string cooldownKey = GetCooldownKey(handler);
        if (!string.IsNullOrEmpty(cooldownKey)
            && cooldownEndTimes.TryGetValue(cooldownKey, out float cooldownEndTime)
            && Time.time < cooldownEndTime)
        {
            return false;
        }

        return true;
    }

    private void StartCooldownIfNeeded(WeightedGroundTrapHandler handler)
    {
        string cooldownKey = GetCooldownKey(handler);
        if (string.IsNullOrEmpty(cooldownKey))
            return;

        cooldownEndTimes[cooldownKey] = Time.time + cooldownDuration;
    }

    private string GetCooldownKey(WeightedGroundTrapHandler handler)
    {
        if (handler == null || handler.Target == null)
            return null;

        string targetName = handler.Target.GetType().Name;
        foreach (string cooldownTargetName in cooldownTargetNames)
        {
            if (targetName == cooldownTargetName)
                return targetName;
        }

        return null;
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
