using System;
using UnityEngine;
using UnityEngine.UI;

public class GameEndScreenUI : MonoBehaviour
{
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private Player leftPlayer;
    [SerializeField] private Player rightPlayer;
    [SerializeField] private Vector2 placeholderSize = new Vector2(220f, 220f);
    [SerializeField] private float buttonShowDelay = 5f;
    [SerializeField] private Vector2 buttonSize = new Vector2(64f, 64f);
    [SerializeField] private float buttonSpacing = 48f;
    [SerializeField] private float buttonBottomOffset = 44f;
    [SerializeField] private Color leftWinnerColor = Color.red;
    [SerializeField] private Color rightLoserColor = Color.green;
    [SerializeField] private Color buttonNormalColor = Color.white;
    [SerializeField] private Color buttonHoverColor = new Color(1f, 0.85f, 0.25f, 1f);
    [SerializeField] private Color buttonSelectedColor = new Color(1f, 0.55f, 0.15f, 1f);
    [SerializeField] private bool pauseGameOnShow = true;
    [SerializeField] private string mainMenuSceneName;

    private readonly Player[] boundPlayers = new Player[2];
    private readonly Button[] endButtons = new Button[2];
    private readonly Image[] endButtonImages = new Image[2];
    private GameObject endPanel;
    private GameObject buttonGroup;
    private bool isShowing;
    private bool areButtonsVisible;
    private float buttonShowTime;
    private int selectedButtonIndex;

    private void Start()
    {
        BuildEndScreen();
        RefreshPlayerBindings();
        CheckGameEnd();
    }

    private void Update()
    {
        if (isShowing)
        {
            HandleShownEndScreen();
            return;
        }

        RefreshPlayerBindings();
        CheckGameEnd();
    }

    private void OnDisable()
    {
        UnbindPlayer(0);
        UnbindPlayer(1);
    }

    private void BuildEndScreen()
    {
        if (endPanel != null)
            return;

        if (targetCanvas == null)
        {
            GameObject canvasObject = GameObject.Find("Global UI Canvas");
            if (canvasObject != null)
                targetCanvas = canvasObject.GetComponent<Canvas>();
        }

        if (targetCanvas == null)
            targetCanvas = FindObjectOfType<Canvas>();

        if (targetCanvas == null)
            return;

        endPanel = new GameObject("Game End Panel", typeof(RectTransform));
        endPanel.transform.SetParent(targetCanvas.transform, false);
        endPanel.transform.SetAsLastSibling();

        RectTransform panelRect = endPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        RectTransform leftContent = CreateMaskedArea("Left End Area", new Vector2(0f, 0f), new Vector2(0.5f, 1f));
        RectTransform rightContent = CreateMaskedArea("Right End Area", new Vector2(0.5f, 0f), new Vector2(1f, 1f));

        CreatePlaceholder(leftContent, "Left Winner Placeholder", leftWinnerColor);
        CreatePlaceholder(rightContent, "Right Loser Placeholder", rightLoserColor);
        CreateButtonGroup();

        endPanel.SetActive(false);
    }

    private RectTransform CreateMaskedArea(string areaName, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject areaObject = new GameObject(areaName, typeof(RectTransform), typeof(RectMask2D));
        areaObject.transform.SetParent(endPanel.transform, false);

        RectTransform areaRect = areaObject.GetComponent<RectTransform>();
        areaRect.anchorMin = anchorMin;
        areaRect.anchorMax = anchorMax;
        areaRect.offsetMin = Vector2.zero;
        areaRect.offsetMax = Vector2.zero;

        GameObject contentObject = new GameObject(areaName + " Content", typeof(RectTransform));
        contentObject.transform.SetParent(areaObject.transform, false);

        RectTransform contentRect = contentObject.GetComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;

        return contentRect;
    }

    private void CreatePlaceholder(RectTransform parent, string objectName, Color color)
    {
        GameObject imageObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.transform.SetParent(parent, false);

        RectTransform imageRect = imageObject.GetComponent<RectTransform>();
        imageRect.anchorMin = new Vector2(0.5f, 0.5f);
        imageRect.anchorMax = new Vector2(0.5f, 0.5f);
        imageRect.pivot = new Vector2(0.5f, 0.5f);
        imageRect.anchoredPosition = Vector2.zero;
        imageRect.sizeDelta = placeholderSize;

        Image image = imageObject.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
    }

    private void CreateButtonGroup()
    {
        buttonGroup = new GameObject("End Screen Buttons", typeof(RectTransform));
        buttonGroup.transform.SetParent(endPanel.transform, false);

        RectTransform groupRect = buttonGroup.GetComponent<RectTransform>();
        groupRect.anchorMin = new Vector2(0.5f, 0f);
        groupRect.anchorMax = new Vector2(0.5f, 0f);
        groupRect.pivot = new Vector2(0.5f, 0f);
        groupRect.anchoredPosition = new Vector2(0f, buttonBottomOffset);
        groupRect.sizeDelta = new Vector2(buttonSize.x * 2f + buttonSpacing, buttonSize.y);

        endButtons[0] = CreateEndButton(groupRect, "Restart Button", -((buttonSize.x + buttonSpacing) * 0.5f), RestartGame);
        endButtons[1] = CreateEndButton(groupRect, "Main Menu Button", (buttonSize.x + buttonSpacing) * 0.5f, BackToMainMenu);

        buttonGroup.SetActive(false);
    }

    private Button CreateEndButton(RectTransform parent, string objectName, float anchoredX, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = new Vector2(anchoredX, 0f);
        buttonRect.sizeDelta = buttonSize;

        Image buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.color = buttonNormalColor;
        buttonImage.raycastTarget = true;

        Button button = buttonObject.GetComponent<Button>();
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(onClick);

        ColorBlock colors = button.colors;
        colors.normalColor = buttonNormalColor;
        colors.highlightedColor = buttonHoverColor;
        colors.selectedColor = buttonSelectedColor;
        colors.pressedColor = buttonSelectedColor;
        colors.disabledColor = new Color(0.6f, 0.6f, 0.6f, 0.5f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.1f;
        button.colors = colors;

        int buttonIndex = objectName.Contains("Restart") ? 0 : 1;
        endButtonImages[buttonIndex] = buttonImage;
        return button;
    }

    private void RefreshPlayerBindings()
    {
        Player nextLeftPlayer = leftPlayer;
        Player nextRightPlayer = rightPlayer;

        Player[] players = FindObjectsOfType<Player>();
        Array.Sort(players, (a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        if (nextLeftPlayer == null && players.Length > 0)
            nextLeftPlayer = players[0];

        if (nextRightPlayer == null && players.Length > 1)
            nextRightPlayer = players[players.Length - 1];

        BindPlayer(0, nextLeftPlayer);
        BindPlayer(1, nextRightPlayer);
    }

    private void BindPlayer(int index, Player player)
    {
        if (boundPlayers[index] == player)
            return;

        UnbindPlayer(index);
        boundPlayers[index] = player;

        if (player != null)
            player.HealthChanged += OnPlayerHealthChanged;
    }

    private void UnbindPlayer(int index)
    {
        if (boundPlayers[index] != null)
            boundPlayers[index].HealthChanged -= OnPlayerHealthChanged;

        boundPlayers[index] = null;
    }

    private void OnPlayerHealthChanged(Player player)
    {
        CheckGameEnd();
    }

    private void CheckGameEnd()
    {
        if (isShowing)
            return;

        for (int i = 0; i < boundPlayers.Length; i++)
        {
            if (boundPlayers[i] != null && boundPlayers[i].CurrentHealth <= 0)
            {
                ShowEndScreen();
                return;
            }
        }
    }

    private void ShowEndScreen()
    {
        if (endPanel == null)
            BuildEndScreen();

        if (endPanel == null)
            return;

        isShowing = true;
        areButtonsVisible = false;
        buttonShowTime = Time.unscaledTime + buttonShowDelay;
        selectedButtonIndex = 0;
        endPanel.SetActive(true);
        endPanel.transform.SetAsLastSibling();

        if (buttonGroup != null)
            buttonGroup.SetActive(false);

        if (pauseGameOnShow)
            Time.timeScale = 0f;
    }

    private void HandleShownEndScreen()
    {
        if (!areButtonsVisible && Time.unscaledTime >= buttonShowTime)
            ShowButtons();

        if (areButtonsVisible)
            HandleButtonInput();
    }

    private void ShowButtons()
    {
        areButtonsVisible = true;

        if (buttonGroup != null)
            buttonGroup.SetActive(true);

        SelectButton(0);
    }

    private void HandleButtonInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            SelectButton(selectedButtonIndex == 0 ? 1 : 0);
            return;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) ||
            Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            SelectButton(selectedButtonIndex == 0 ? 1 : 0);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.F))
            ClickSelectedButton();
    }

    private void SelectButton(int index)
    {
        selectedButtonIndex = Mathf.Clamp(index, 0, endButtons.Length - 1);

        for (int i = 0; i < endButtonImages.Length; i++)
        {
            if (endButtonImages[i] != null)
                endButtonImages[i].color = i == selectedButtonIndex ? buttonSelectedColor : buttonNormalColor;
        }
    }

    private void ClickSelectedButton()
    {
        if (selectedButtonIndex < 0 || selectedButtonIndex >= endButtons.Length || endButtons[selectedButtonIndex] == null)
            return;

        endButtons[selectedButtonIndex].onClick.Invoke();
    }

    private void RestartGame()
    {
        GameSceneLoader.RestartCurrentScene();
    }

    private void BackToMainMenu()
    {
        GameSceneLoader.LoadMainMenu(mainMenuSceneName);
    }
}
