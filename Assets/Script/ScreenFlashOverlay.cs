using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 화면 전체를 밝은 빛으로 덮습니다. Player에 붙이거나 텔레포트 시 자동 추가됩니다.
/// </summary>
public class ScreenFlashOverlay : MonoBehaviour
{
    [SerializeField] private Image overlayImage;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float fadeOutSeconds = 0.25f;

    private Coroutine flashRoutine;

    private void Awake()
    {
        EnsureOverlay();
    }

    public void Flash(float holdSeconds)
    {
        EnsureOverlay();
        if (overlayImage == null)
            return;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine(holdSeconds));
    }

    private IEnumerator FlashRoutine(float holdSeconds)
    {
        overlayImage.gameObject.SetActive(true);
        overlayImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 1f);

        yield return new WaitForSeconds(holdSeconds);

        float elapsed = 0f;
        while (elapsed < fadeOutSeconds)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / fadeOutSeconds);
            overlayImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
            yield return null;
        }

        overlayImage.gameObject.SetActive(false);
        flashRoutine = null;
    }

    private void EnsureOverlay()
    {
        if (overlayImage != null)
            return;

        GameObject canvasObj = new GameObject("FlashOverlayCanvas");
        canvasObj.transform.SetParent(transform, false);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        GameObject imageObj = new GameObject("FlashImage");
        imageObj.transform.SetParent(canvasObj.transform, false);

        RectTransform rect = imageObj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        overlayImage = imageObj.AddComponent<Image>();
        overlayImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);
        overlayImage.raycastTarget = false;
        overlayImage.gameObject.SetActive(false);
    }
}
