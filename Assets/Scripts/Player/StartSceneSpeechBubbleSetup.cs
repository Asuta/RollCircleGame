using System;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneSpeechBubbleSetup : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Vector3 bubbleOffset = new Vector3(0f, 2.4f, 0f);
    [SerializeField] private Vector2 bubbleSize = new Vector2(260f, 120f);
    [SerializeField] private Vector2 tailSize = new Vector2(34f, 34f);
    [SerializeField] private float worldScale = 0.01f;
    [SerializeField] private string leftText = "Hello!";
    [SerializeField] private string rightText = "Ready?";
    [SerializeField] private Color bubbleColor = Color.white;
    [SerializeField] private Color textColor = Color.black;
    [SerializeField] private Color outlineColor = Color.black;

    private readonly Transform[] bubbleRoots = new Transform[2];

    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        CreateBubbles();
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
            return;

        for (int i = 0; i < bubbleRoots.Length; i++)
        {
            if (bubbleRoots[i] != null)
                bubbleRoots[i].rotation = targetCamera.transform.rotation;
        }
    }

    private void CreateBubbles()
    {
        PlayerOnRotatingPlatform[] players = FindObjectsOfType<PlayerOnRotatingPlatform>();
        Array.Sort(players, (a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        int count = Mathf.Min(players.Length, bubbleRoots.Length);
        for (int i = 0; i < count; i++)
        {
            string bubbleText = i == 0 ? leftText : rightText;
            bubbleRoots[i] = CreateBubble(players[i].transform, bubbleText).transform;
        }
    }

    private GameObject CreateBubble(Transform player, string bubbleText)
    {
        GameObject root = new GameObject("Speech Bubble", typeof(RectTransform), typeof(Canvas));
        root.transform.SetParent(player, false);
        root.transform.localPosition = bubbleOffset;
        root.transform.localScale = Vector3.one * worldScale;

        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.sizeDelta = bubbleSize;

        Canvas canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = targetCamera;
        canvas.sortingOrder = 20;

        GameObject tail = CreateImage("Tail", root.transform, tailSize, bubbleColor, true);
        RectTransform tailRect = tail.GetComponent<RectTransform>();
        tailRect.anchorMin = new Vector2(0.5f, 0f);
        tailRect.anchorMax = new Vector2(0.5f, 0f);
        tailRect.pivot = new Vector2(0.5f, 0.5f);
        tailRect.anchoredPosition = new Vector2(-45f, -8f);
        tailRect.localEulerAngles = new Vector3(0f, 0f, 45f);

        GameObject body = CreateImage("Body", root.transform, bubbleSize, bubbleColor, true);
        RectTransform bodyRect = body.GetComponent<RectTransform>();
        bodyRect.anchorMin = Vector2.zero;
        bodyRect.anchorMax = Vector2.one;
        bodyRect.offsetMin = Vector2.zero;
        bodyRect.offsetMax = Vector2.zero;

        GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(body.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(18f, 14f);
        textRect.offsetMax = new Vector2(-18f, -14f);

        Text text = textObject.GetComponent<Text>();
        text.text = bubbleText;
        text.color = textColor;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.alignment = TextAnchor.MiddleCenter;
        text.fontSize = 30;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 12;
        text.resizeTextMaxSize = 30;

        return root;
    }

    private GameObject CreateImage(string name, Transform parent, Vector2 size, Color color, bool withOutline)
    {
        GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.transform.SetParent(parent, false);

        RectTransform rect = imageObject.GetComponent<RectTransform>();
        rect.sizeDelta = size;

        Image image = imageObject.GetComponent<Image>();
        image.color = color;

        if (withOutline)
        {
            Outline outline = imageObject.AddComponent<Outline>();
            outline.effectColor = outlineColor;
            outline.effectDistance = new Vector2(4f, -4f);
        }

        return imageObject;
    }
}
