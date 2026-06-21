using UnityEngine;
using UnityEngine.UI;

public class SplitScreenSetup : MonoBehaviour
{
    public Camera player1Camera;
    public Camera player2Camera;
    public bool mirrorPlayer2Output = true;

    private RenderTexture player2RenderTexture;
    private RawImage player2MirrorImage;
    private int currentTextureWidth;
    private int currentTextureHeight;

    void Start()
    {
        SetupSplitScreen();
    }

    private void Update()
    {
        if (mirrorPlayer2Output && ScreenSizeChanged())
            SetupPlayer2MirrorOutput();
    }

    private void OnDestroy()
    {
        ReleasePlayer2RenderTexture();
    }

    private void SetupSplitScreen()
    {
        player1Camera.rect = new Rect(0f, 0f, 0.5f, 1f); // 左半屏

        if (mirrorPlayer2Output)
        {
            SetupPlayer2MirrorOutput();
        }
        else
        {
            player2Camera.targetTexture = null;
            player2Camera.rect = new Rect(0.5f, 0f, 0.5f, 1f); // 右半屏
        }
    }

    private void SetupPlayer2MirrorOutput()
    {
        currentTextureWidth = Mathf.Max(1, Screen.width / 2);
        currentTextureHeight = Mathf.Max(1, Screen.height);

        ReleasePlayer2RenderTexture();

        player2RenderTexture = new RenderTexture(currentTextureWidth, currentTextureHeight, 24);
        player2RenderTexture.name = "Player2 Mirror Output";
        player2Camera.targetTexture = player2RenderTexture;
        player2Camera.rect = new Rect(0f, 0f, 1f, 1f);

        if (player2MirrorImage == null)
            player2MirrorImage = CreatePlayer2MirrorImage();

        player2MirrorImage.texture = player2RenderTexture;
        player2MirrorImage.uvRect = new Rect(1f, 0f, -1f, 1f);
    }

    private RawImage CreatePlayer2MirrorImage()
    {
        GameObject canvasObject = new GameObject("Player2 Mirror Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject imageObject = new GameObject("Player2 Mirror Image", typeof(RawImage));
        imageObject.transform.SetParent(canvasObject.transform, false);

        RawImage rawImage = imageObject.GetComponent<RawImage>();
        rawImage.raycastTarget = false;

        RectTransform rectTransform = rawImage.rectTransform;
        rectTransform.anchorMin = new Vector2(0.5f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        return rawImage;
    }

    private bool ScreenSizeChanged()
    {
        return player2RenderTexture == null ||
               currentTextureWidth != Mathf.Max(1, Screen.width / 2) ||
               currentTextureHeight != Mathf.Max(1, Screen.height);
    }

    private void ReleasePlayer2RenderTexture()
    {
        if (player2RenderTexture == null)
            return;

        if (player2Camera != null && player2Camera.targetTexture == player2RenderTexture)
            player2Camera.targetTexture = null;

        player2RenderTexture.Release();
        Destroy(player2RenderTexture);
        player2RenderTexture = null;
    }
}
