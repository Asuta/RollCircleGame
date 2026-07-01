using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameStartSequence : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private Text messageText;
    [SerializeField] private PauseManager pauseManager;
    [SerializeField] private float readyDuration = 1f;
    [SerializeField] private float goDuration = 1f;
    [SerializeField] private float startScale = 0.2f;
    [SerializeField] private float endScale = 1f;

    private void Awake()
    {
        Time.timeScale = 0f;

        if (startPanel != null)
            startPanel.SetActive(true);

        if (pauseManager != null)
            pauseManager.SetPauseInputEnabled(false);
    }

    private void Start()
    {
        StartCoroutine(StartSequenceRoutine());
    }

    private IEnumerator StartSequenceRoutine()
    {
        yield return ShowScaledMessage("Ready", readyDuration);
        yield return ShowScaledMessage("Go!", goDuration);

        if (startPanel != null)
            startPanel.SetActive(false);

        if (pauseManager != null)
            pauseManager.SetPauseInputEnabled(true);

        Time.timeScale = 1f;
    }

    private IEnumerator ShowScaledMessage(string message, float duration)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.transform.localScale = Vector3.one * startScale;
        }

        float elapsedTime = 0f;
        float safeDuration = Mathf.Max(0.01f, duration);

        while (elapsedTime < safeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / safeDuration);

            if (messageText != null)
                messageText.transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, t);

            yield return null;
        }

        if (messageText != null)
        {
            messageText.text = string.Empty;
            messageText.transform.localScale = Vector3.one * startScale;
        }
    }
}
