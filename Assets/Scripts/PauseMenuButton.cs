using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PauseMenuButton : MonoBehaviour
{
    public enum ButtonAction
    {
        Resume,
        Restart,
        Tutorial,
        Quit
    }

    [SerializeField] private PauseManager pauseManager;
    [SerializeField] private ButtonAction action;

    private Button button;

    private void OnEnable()
    {
        button = GetComponent<Button>();
        button.onClick.RemoveListener(HandleClick);
        button.onClick.AddListener(HandleClick);
    }

    private void OnDisable()
    {
        if (button != null)
            button.onClick.RemoveListener(HandleClick);
    }

    private void HandleClick()
    {
        switch (action)
        {
            case ButtonAction.Resume:
                if (pauseManager != null)
                    pauseManager.Resume();
                break;
            case ButtonAction.Restart:
                if (pauseManager != null)
                    pauseManager.Restart();
                break;
            case ButtonAction.Tutorial:
                if (pauseManager != null)
                    pauseManager.OpenTutorial();
                break;
            case ButtonAction.Quit:
                Time.timeScale = 1f;
                Application.Quit();
                break;
        }
    }
}
