using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneSpeechBubbleSetup : MonoBehaviour
{
    public enum Speaker
    {
        LeftPlayer,
        RightPlayer
    }

    [Serializable]
    public class DialogueEntry
    {
        public Speaker speaker;
        [TextArea]
        public string text;
    }

    [SerializeField] private GameObject speechBubblePrefab;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Vector3 bubbleOffset = new Vector3(0f, 2.4f, 0f);
    [SerializeField] private float worldScale = 0.01f;
    [SerializeField] private float bubbleLifetime = 3f;
    [SerializeField] private float bubbleInterval = 0.15f;
    [SerializeField] private bool loopDialogue = true;
    [SerializeField] private DialogueEntry[] dialogueEntries =
    {
        new DialogueEntry
        {
            speaker = Speaker.LeftPlayer,
            text = "动了！我们真的活过来了！喂，既然有了生命，敢不敢去那个旋转的大擂台上赌一把？"
        },
        new DialogueEntry
        {
            speaker = Speaker.RightPlayer,
            text = "赌什么？输的人今晚负责把桌上的胶水盖拧紧？"
        },
        new DialogueEntry
        {
            speaker = Speaker.LeftPlayer,
            text = "切，格局小了！谁先被转盘上面的激光把8层纸全烫穿--谁就要当对方一整年的小跟班！"
        },
        new DialogueEntry
        {
            speaker = Speaker.RightPlayer,
            text = "有点意思。看来踩到那些按钮，还能给你的转盘送点小礼物。你这薄薄的身板，扛得住8下吗？"
        },
        new DialogueEntry
        {
            speaker = Speaker.LeftPlayer,
            text = "少废话，等会别求着我帮你粘伤口就行！转盘见！"
        }
    };

    private readonly Transform[] bubbleRoots = new Transform[2];
    private readonly Transform[] players = new Transform[2];

    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (speechBubblePrefab != null)
            speechBubblePrefab.SetActive(false);

        if (!TryFindPlayers())
            return;

        StartCoroutine(PlayDialogue());
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

    private bool TryFindPlayers()
    {
        if (speechBubblePrefab == null)
        {
            Debug.LogWarning("Speech bubble prefab is not assigned.");
            return false;
        }

        PlayerOnRotatingPlatform[] foundPlayers = FindObjectsOfType<PlayerOnRotatingPlatform>();
        Array.Sort(foundPlayers, (a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        int count = Mathf.Min(foundPlayers.Length, players.Length);
        for (int i = 0; i < count; i++)
            players[i] = foundPlayers[i].transform;

        if (players[0] == null || players[1] == null)
        {
            Debug.LogWarning("Start scene speech bubbles need two players.");
            return false;
        }

        return true;
    }

    private IEnumerator PlayDialogue()
    {
        if (dialogueEntries == null || dialogueEntries.Length == 0)
            yield break;

        int dialogueIndex = 0;
        do
        {
            DialogueEntry entry = dialogueEntries[dialogueIndex];
            if (entry == null)
            {
                dialogueIndex = GetNextDialogueIndex(dialogueIndex);
                if (dialogueIndex < 0)
                    break;

                continue;
            }

            int playerIndex = entry.speaker == Speaker.LeftPlayer ? 0 : 1;
            GameObject bubble = CreateBubble(players[playerIndex], entry.text);
            Transform bubbleTransform = bubble.transform;
            bubbleRoots[playerIndex] = bubble.transform;

            yield return new WaitForSeconds(Mathf.Max(0.01f, bubbleLifetime));

            if (bubble != null)
                Destroy(bubble);

            if (bubbleRoots[playerIndex] == bubbleTransform)
                bubbleRoots[playerIndex] = null;

            dialogueIndex = GetNextDialogueIndex(dialogueIndex);
            if (dialogueIndex < 0)
                break;

            yield return new WaitForSeconds(Mathf.Max(0f, bubbleInterval));
        }
        while (true);
    }

    private int GetNextDialogueIndex(int currentIndex)
    {
        int nextIndex = currentIndex + 1;
        if (nextIndex < dialogueEntries.Length)
            return nextIndex;

        return loopDialogue ? 0 : -1;
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
