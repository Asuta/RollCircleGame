using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private bool isPaused;
    private bool canPause = true;

    private void Start()
    {
        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (!canPause)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
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
        SetPaused(false);
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
    }

    private void SetPaused(bool paused)
    {
        isPaused = paused;

        if (pausePanel != null)
            pausePanel.SetActive(paused);

        Time.timeScale = paused ? 0f : 1f;
    }

    private void OnDestroy()
    {
        if (isPaused)
            Time.timeScale = 1f;
    }
}
