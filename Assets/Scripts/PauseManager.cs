using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject tutorialPanel;

    private bool isPaused;
    private bool canPause = true;
    private bool isTutorialOpen;

    private void Start()
    {
        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }

    private void Update()
    {
        if (!canPause)
            return;

        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        if (isTutorialOpen)
        {
            CloseTutorial();
            return;
        }

        TogglePause();
    }

    public void TogglePause()
    {
        SetPaused(!isPaused);
    }

    public void Pause()
    {
        if (!canPause)
            return;

        SetPaused(true);
    }

    public void Resume()
    {
        CloseTutorial();
        SetPaused(false);
    }

    public void OpenTutorial()
    {
        if (tutorialPanel == null)
            return;

        isPaused = true;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        isTutorialOpen = true;
        tutorialPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseTutorial()
    {
        isTutorialOpen = false;

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }

    public void Restart()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name, LoadSceneMode.Single);
    }

    public void SetPauseInputEnabled(bool enabled)
    {
        canPause = enabled;

        if (enabled)
            return;

        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        CloseTutorial();
    }

    private void SetPaused(bool paused)
    {
        isPaused = paused;

        if (pausePanel != null)
            pausePanel.SetActive(paused);

        if (!paused)
            CloseTutorial();

        Time.timeScale = paused ? 0f : 1f;
    }

    private void OnDestroy()
    {
        if (isPaused)
            Time.timeScale = 1f;
    }
}
