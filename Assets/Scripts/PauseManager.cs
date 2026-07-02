using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Button[] pauseButtons;
    [SerializeField] private Color selectedButtonColor = new Color(1f, 0.8f, 0.25f, 1f);

    private bool isPaused;
    private bool canPause = true;
    private bool isTutorialOpen;
    private int selectedButtonIndex;
    private int suppressPauseMenuInputFrame = -1;
    private Graphic[] buttonGraphics;
    private Color[] normalButtonColors;

    private void Start()
    {
        CacheButtonColors();

        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }

    private void Update()
    {
        if (IsTutorialPanelOpen())
        {
            isTutorialOpen = true;

            if (IsTutorialCloseInputDown())
                CloseTutorial();

            return;
        }

        if (!canPause)
            return;

        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        TogglePause();
    }

    private void LateUpdate()
    {
        if (suppressPauseMenuInputFrame == Time.frameCount)
            return;

        if (!canPause || IsTutorialPanelOpen() || !IsPauseMenuOpen())
            return;

        HandlePauseMenuInput();
    }

    public void TogglePause()
    {
        SetPaused(!IsPauseMenuOpen());
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

        ClearSelectedButton();
    }

    public void CloseTutorial()
    {
        suppressPauseMenuInputFrame = Time.frameCount;
        isTutorialOpen = false;

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);

        if (isPaused || IsPauseMenuOpen())
            SelectCurrentButton();
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
        ClearSelectedButton();
    }

    private void SetPaused(bool paused)
    {
        isPaused = paused;

        if (pausePanel != null)
            pausePanel.SetActive(paused);

        if (!paused)
            CloseTutorial();

        Time.timeScale = paused ? 0f : 1f;

        if (paused)
            SelectFirstButton();
        else
            ClearSelectedButton();
    }

    private void HandlePauseMenuInput()
    {
        if (IsPauseMenuOpen() && !isPaused)
            isPaused = true;

        if (!HasAnyButton())
            return;

        SyncSelectedButtonIndex();

        if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null)
            SelectCurrentButton();

        if (!IsCurrentButtonAvailable())
            SelectFirstButton();

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            SelectRelativeButton(-1);
            return;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            SelectRelativeButton(1);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.F))
            ClickCurrentButton();
    }

    private void SelectFirstButton()
    {
        selectedButtonIndex = 0;
        SelectRelativeButton(0);
    }

    private void SelectCurrentButton()
    {
        SelectButton(selectedButtonIndex);
    }

    private void SelectRelativeButton(int offset)
    {
        if (!HasAnyButton())
            return;

        int buttonCount = pauseButtons.Length;
        int nextIndex = selectedButtonIndex;

        for (int i = 0; i < buttonCount; i++)
        {
            nextIndex = WrapIndex(nextIndex + offset, buttonCount);

            if (IsButtonAvailable(pauseButtons[nextIndex]))
            {
                SelectButton(nextIndex);
                return;
            }

            if (offset == 0)
                nextIndex++;
        }
    }

    private void SelectButton(int index)
    {
        if (!HasAnyButton())
            return;

        selectedButtonIndex = WrapIndex(index, pauseButtons.Length);
        Button button = pauseButtons[selectedButtonIndex];

        if (!IsButtonAvailable(button))
            return;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        RefreshButtonVisuals();
    }

    private void ClickCurrentButton()
    {
        if (!IsCurrentButtonAvailable())
            return;

        pauseButtons[selectedButtonIndex].onClick.Invoke();
    }

    private void ClearSelectedButton()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        ResetButtonVisuals();
    }

    private void SyncSelectedButtonIndex()
    {
        if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null)
            return;

        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        for (int i = 0; i < pauseButtons.Length; i++)
        {
            if (pauseButtons[i] != null && pauseButtons[i].gameObject == selectedObject)
            {
                selectedButtonIndex = i;
                return;
            }
        }
    }

    private bool IsCurrentButtonAvailable()
    {
        if (!HasAnyButton())
            return false;

        selectedButtonIndex = WrapIndex(selectedButtonIndex, pauseButtons.Length);
        return IsButtonAvailable(pauseButtons[selectedButtonIndex]);
    }

    private bool IsButtonAvailable(Button button)
    {
        return button != null && button.gameObject.activeInHierarchy && button.interactable;
    }

    private bool HasAnyButton()
    {
        return pauseButtons != null && pauseButtons.Length > 0;
    }

    private bool IsPauseMenuOpen()
    {
        return pausePanel != null && pausePanel.activeInHierarchy;
    }

    private bool IsTutorialPanelOpen()
    {
        return tutorialPanel != null && tutorialPanel.activeInHierarchy;
    }

    private bool IsTutorialCloseInputDown()
    {
        return Input.GetKeyDown(KeyCode.Escape)
            || Input.GetKeyDown(KeyCode.F)
            || Input.GetKeyDown(KeyCode.Space);
    }

    private void CacheButtonColors()
    {
        if (!HasAnyButton())
            return;

        buttonGraphics = new Graphic[pauseButtons.Length];
        normalButtonColors = new Color[pauseButtons.Length];

        for (int i = 0; i < pauseButtons.Length; i++)
        {
            if (pauseButtons[i] == null || pauseButtons[i].targetGraphic == null)
                continue;

            buttonGraphics[i] = pauseButtons[i].targetGraphic;
            normalButtonColors[i] = pauseButtons[i].targetGraphic.color;
        }
    }

    private void RefreshButtonVisuals()
    {
        if (!HasAnyButton() || buttonGraphics == null || normalButtonColors == null)
            return;

        for (int i = 0; i < pauseButtons.Length; i++)
        {
            if (buttonGraphics[i] == null)
                continue;

            buttonGraphics[i].color = i == selectedButtonIndex ? selectedButtonColor : normalButtonColors[i];
        }
    }

    private void ResetButtonVisuals()
    {
        if (!HasAnyButton() || buttonGraphics == null || normalButtonColors == null)
            return;

        for (int i = 0; i < pauseButtons.Length; i++)
        {
            if (buttonGraphics[i] != null)
                buttonGraphics[i].color = normalButtonColors[i];
        }
    }

    private int WrapIndex(int index, int count)
    {
        if (count <= 0)
            return 0;

        return (index % count + count) % count;
    }

    private void OnDestroy()
    {
        if (isPaused)
            Time.timeScale = 1f;
    }
}
