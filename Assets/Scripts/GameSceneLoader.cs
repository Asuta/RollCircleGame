using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameSceneLoader
{
    public static void RestartCurrentScene()
    {
        ResetRuntimeState();

        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.buildIndex >= 0)
        {
            SceneManager.LoadScene(currentScene.buildIndex, LoadSceneMode.Single);
            return;
        }

        SceneManager.LoadScene(currentScene.name, LoadSceneMode.Single);
    }

    public static void LoadMainMenu(string sceneName)
    {
        ResetRuntimeState();

        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            return;
        }

        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    private static void ResetRuntimeState()
    {
        Time.timeScale = 1f;
        GlobalEvents.ClearAllListeners();
        Car.ClearHitPlayerListeners();
    }
}
