using System;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneSpeechBubbleSetup : MonoBehaviour
{
    [SerializeField] private GameObject speechBubblePrefab;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Vector3 bubbleOffset = new Vector3(0f, 2.4f, 0f);
    [SerializeField] private float worldScale = 0.01f;
    [SerializeField] private string leftText = "Hello!";
    [SerializeField] private string rightText = "Ready?";

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
        if (speechBubblePrefab == null)
        {
            Debug.LogWarning("Speech bubble prefab is not assigned.");
            return;
        }

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
        GameObject root = Instantiate(speechBubblePrefab, player, false);
        root.name = player.name + " Speech Bubble";
        root.transform.localPosition = bubbleOffset;
        root.transform.localRotation = Quaternion.identity;
        root.transform.localScale = Vector3.one * worldScale;

        Canvas canvas = root.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = targetCamera;
            canvas.sortingOrder = 20;
        }

        Text text = root.GetComponentInChildren<Text>(true);
        if (text != null)
            text.text = bubbleText;

        root.SetActive(true);

        return root;
    }
}
