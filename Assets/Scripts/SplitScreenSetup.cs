using UnityEngine;
using UnityEngine.UI;

public class SplitScreenSetup : MonoBehaviour
{
    public Camera player1Camera;
    public Camera player2Camera;
    public bool mirrorPlayer2Output = true;
    public GameObject player2CameraSourceObject;
    public bool createPlayer2CameraFromCopy;

    private static readonly Vector3 Player2CopyOffset = new Vector3(50f, 0f, 0f);
    private RenderTexture player2RenderTexture;
    private RawImage player2MirrorImage;
    private int currentTextureWidth;
    private int currentTextureHeight;

    void Start()
    {
        CreatePlayer2CameraCopyIfNeeded();
        RefreshSplitScreen();
    }

    private void Update()
    {
        RefreshSplitScreen();
    }

    private void OnDestroy()
    {
        ReleasePlayer2RenderTexture();
    }

    private void CreatePlayer2CameraCopyIfNeeded()
    {
        if (!createPlayer2CameraFromCopy || player2CameraSourceObject == null)
            return;

        GameObject copiedObject = Instantiate(player2CameraSourceObject, player2CameraSourceObject.transform.parent);
        copiedObject.name = player2CameraSourceObject.name + " Player2 Copy";
        copiedObject.transform.position += Player2CopyOffset;

        Camera copiedCamera = copiedObject.GetComponentInChildren<Camera>(true);
        if (copiedCamera != null)
        {
            player2Camera = copiedCamera;
        }
        else
        {
            Debug.LogWarning("复制出来的对象里没有找到 Camera：" + copiedObject.name);
        }

        PlayerOnRotatingPlatform copiedPlayer = copiedObject.GetComponentInChildren<PlayerOnRotatingPlatform>(true);
        if (copiedPlayer != null)
        {
            copiedPlayer.moveControlMode = PlayerOnRotatingPlatform.MoveControlMode.ArrowKeys;
        }
        else
        {
            Debug.LogWarning("复制出来的对象里没有找到 PlayerOnRotatingPlatform：" + copiedObject.name);
        }
    }

    private void RefreshSplitScreen()
    {
        bool hasPlayer1Camera = player1Camera != null;
        bool hasPlayer2Camera = player2Camera != null;

        if (player1Camera != null)
            player1Camera.rect = hasPlayer2Camera ? new Rect(0f, 0f, 0.5f, 1f) : new Rect(0f, 0f, 1f, 1f);

        if (!hasPlayer2Camera)
        {
            HidePlayer2MirrorOutput();
            return;
        }

        Rect player2ScreenRect = hasPlayer1Camera ? new Rect(0.5f, 0f, 0.5f, 1f) : new Rect(0f, 0f, 1f, 1f);

        if (mirrorPlayer2Output)
        {
            EnsurePlayer2MirrorOutput(player2ScreenRect);
        }
        else
        {
            HidePlayer2MirrorOutput();
            player2Camera.targetTexture = null;
            player2Camera.rect = player2ScreenRect;
        }
    }

    private void EnsurePlayer2MirrorOutput(Rect screenRect)
    {
        int textureWidth = Mathf.Max(1, Mathf.RoundToInt(Screen.width * screenRect.width));
        int textureHeight = Mathf.Max(1, Mathf.RoundToInt(Screen.height * screenRect.height));

        if (player2RenderTexture == null || currentTextureWidth != textureWidth || currentTextureHeight != textureHeight)
        {
            currentTextureWidth = textureWidth;
            currentTextureHeight = textureHeight;

            ReleasePlayer2RenderTexture();

            player2RenderTexture = new RenderTexture(currentTextureWidth, currentTextureHeight, 24);
            player2RenderTexture.name = "Player2 Mirror Output";
        }

        player2Camera.targetTexture = player2RenderTexture;
        player2Camera.rect = new Rect(0f, 0f, 1f, 1f);

        if (player2MirrorImage == null)
            player2MirrorImage = CreatePlayer2MirrorImage();

        player2MirrorImage.texture = player2RenderTexture;
        player2MirrorImage.uvRect = new Rect(1f, 0f, -1f, 1f);
        SetPlayer2MirrorImageRect(screenRect);
        player2MirrorImage.enabled = true;
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

    private void SetPlayer2MirrorImageRect(Rect screenRect)
    {
        RectTransform rectTransform = player2MirrorImage.rectTransform;
        rectTransform.anchorMin = new Vector2(screenRect.xMin, screenRect.yMin);
        rectTransform.anchorMax = new Vector2(screenRect.xMax, screenRect.yMax);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private void HidePlayer2MirrorOutput()
    {
        if (player2MirrorImage != null)
            player2MirrorImage.enabled = false;

        ReleasePlayer2RenderTexture();
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
