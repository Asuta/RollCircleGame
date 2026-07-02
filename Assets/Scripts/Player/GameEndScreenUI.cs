using System;
using UnityEngine;
using UnityEngine.UI;

public class GameEndScreenUI : MonoBehaviour
{
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private Player leftPlayer;
    [SerializeField] private Player rightPlayer;
    [SerializeField] private Vector2 placeholderSize = new Vector2(220f, 220f);
    [SerializeField] private Color leftWinnerColor = Color.red;
    [SerializeField] private Color rightLoserColor = Color.green;
    [SerializeField] private bool pauseGameOnShow = true;

    private readonly Player[] boundPlayers = new Player[2];
    private GameObject endPanel;
    private bool isShowing;

    private void Start()
    {
        BuildEndScreen();
        RefreshPlayerBindings();
        CheckGameEnd();
    }

    private void Update()
    {
        if (isShowing)
            return;

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
        endPanel.SetActive(true);
        endPanel.transform.SetAsLastSibling();

        if (pauseGameOnShow)
            Time.timeScale = 0f;
    }
}
