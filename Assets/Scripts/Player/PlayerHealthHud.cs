using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthHud : MonoBehaviour
{
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private Player leftPlayer;
    [SerializeField] private Player rightPlayer;
    [SerializeField] private int displayedHealthCount = 8;
    [SerializeField] private Vector2 screenPadding = new Vector2(22f, 18f);
    [SerializeField] private Vector2 logoSize = new Vector2(38f, 38f);
    [SerializeField] private Vector2 healthIconSize = new Vector2(24f, 24f);
    [SerializeField] private float iconSpacing = 8f;
    [SerializeField] private float logoGap = 14f;
    [SerializeField] private Color fullHealthColor = new Color(1f, 0.1f, 0.1f, 1f);
    [SerializeField] private Color emptyHealthColor = new Color(0.45f, 0.45f, 0.45f, 1f);
    [SerializeField] private Color logoColor = Color.white;

    private readonly Player[] boundPlayers = new Player[2];
    private readonly Image[][] healthImages = new Image[2][];
    private RectTransform hudRoot;

    private void Start()
    {
        BuildHud();
        RefreshPlayerBindings();
        RefreshAllHealthViews();
    }

    private void Update()
    {
        RefreshPlayerBindings();
    }

    private void OnDisable()
    {
        UnbindPlayer(0);
        UnbindPlayer(1);
    }

    private void BuildHud()
    {
        if (hudRoot != null)
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

        GameObject rootObject = new GameObject("Game Health HUD", typeof(RectTransform));
        rootObject.transform.SetParent(targetCanvas.transform, false);
        rootObject.transform.SetAsFirstSibling();

        hudRoot = rootObject.GetComponent<RectTransform>();
        hudRoot.anchorMin = Vector2.zero;
        hudRoot.anchorMax = Vector2.one;
        hudRoot.offsetMin = Vector2.zero;
        hudRoot.offsetMax = Vector2.zero;

        CreatePlayerHud(0, "Left Player Health", true);
        CreatePlayerHud(1, "Right Player Health", false);
    }

    private void CreatePlayerHud(int sideIndex, string panelName, bool isLeftSide)
    {
        float heartRowWidth = displayedHealthCount * healthIconSize.x + Mathf.Max(0, displayedHealthCount - 1) * iconSpacing;
        float panelWidth = logoSize.x + logoGap + heartRowWidth;
        float panelHeight = Mathf.Max(logoSize.y, healthIconSize.y);

        GameObject panelObject = new GameObject(panelName, typeof(RectTransform));
        panelObject.transform.SetParent(hudRoot, false);

        RectTransform panel = panelObject.GetComponent<RectTransform>();
        panel.anchorMin = isLeftSide ? new Vector2(0f, 0f) : new Vector2(1f, 0f);
        panel.anchorMax = panel.anchorMin;
        panel.pivot = isLeftSide ? new Vector2(0f, 0f) : new Vector2(1f, 0f);
        panel.anchoredPosition = isLeftSide
            ? new Vector2(screenPadding.x, screenPadding.y)
            : new Vector2(-screenPadding.x, screenPadding.y);
        panel.sizeDelta = new Vector2(panelWidth, panelHeight);

        float logoX = isLeftSide ? logoSize.x * 0.5f : -logoSize.x * 0.5f;
        CreateSolidImage(panel, "Player Logo", logoColor, logoSize, logoX, !isLeftSide);

        healthImages[sideIndex] = new Image[displayedHealthCount];

        for (int i = 0; i < displayedHealthCount; i++)
        {
            float iconX = isLeftSide
                ? logoSize.x + logoGap + healthIconSize.x * 0.5f + i * (healthIconSize.x + iconSpacing)
                : -(logoSize.x + logoGap + heartRowWidth) + healthIconSize.x * 0.5f + i * (healthIconSize.x + iconSpacing);

            healthImages[sideIndex][i] = CreateSolidImage(panel, "Health Icon", fullHealthColor, healthIconSize, iconX, !isLeftSide);
        }
    }

    private Image CreateSolidImage(RectTransform parent, string objectName, Color color, Vector2 size, float anchoredX, bool anchorFromRight)
    {
        GameObject imageObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.transform.SetParent(parent, false);

        RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
        float anchorX = anchorFromRight ? 1f : 0f;
        rectTransform.anchorMin = new Vector2(anchorX, 0.5f);
        rectTransform.anchorMax = new Vector2(anchorX, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(anchoredX, 0f);
        rectTransform.sizeDelta = size;

        Image image = imageObject.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
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

    private void BindPlayer(int sideIndex, Player player)
    {
        if (boundPlayers[sideIndex] == player)
            return;

        UnbindPlayer(sideIndex);
        boundPlayers[sideIndex] = player;

        if (player != null)
            player.HealthChanged += OnPlayerHealthChanged;

        RefreshHealthView(sideIndex);
    }

    private void UnbindPlayer(int sideIndex)
    {
        if (boundPlayers[sideIndex] != null)
            boundPlayers[sideIndex].HealthChanged -= OnPlayerHealthChanged;

        boundPlayers[sideIndex] = null;
    }

    private void OnPlayerHealthChanged(Player player)
    {
        for (int i = 0; i < boundPlayers.Length; i++)
        {
            if (boundPlayers[i] == player)
            {
                RefreshHealthView(i);
                return;
            }
        }
    }

    private void RefreshAllHealthViews()
    {
        RefreshHealthView(0);
        RefreshHealthView(1);
    }

    private void RefreshHealthView(int sideIndex)
    {
        if (healthImages[sideIndex] == null)
            return;

        Player player = boundPlayers[sideIndex];
        int currentHealth = player != null ? player.CurrentHealth : displayedHealthCount;

        for (int i = 0; i < healthImages[sideIndex].Length; i++)
        {
            if (healthImages[sideIndex][i] != null)
                healthImages[sideIndex][i].color = i < currentHealth ? fullHealthColor : emptyHealthColor;
        }
    }
}
