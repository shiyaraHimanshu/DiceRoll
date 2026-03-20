using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class SettingPanel : MonoBehaviour
{
    [Header("Position Settings")]
    [Tooltip("Y Position when the panel is open (Up)")]
    [SerializeField] private float upYPosition = 0f;
    [Tooltip("Y Position when the panel is closed (Down)")]
    [SerializeField] private float downYPosition = -1500f;

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("IAP Settings")]
    [SerializeField] private GameObject removeAdsButton;

    private CanvasGroup canvasGroup;
    public RectTransform rectTransform;
    private Coroutine currentAnimationCoroutine;


    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
       // rectTransform = GetComponent<RectTransform>();
    }

    [ContextMenu("Open")]
    public void Open()
    {
        gameObject.SetActive(true);
        if (currentAnimationCoroutine != null)
        {
            StopCoroutine(currentAnimationCoroutine);
        }
        currentAnimationCoroutine = StartCoroutine(AnimatePanel(upYPosition, 1f, true));
        
        // Ensure UI boolean is updated
        UiManager uiManager = UiManager.instance != null ? UiManager.instance : FindObjectOfType<UiManager>();
        if (uiManager != null)
        {
            uiManager.isSettingOpen = true;
            uiManager.RefreshSettingsUI();
        }

        // Handle Remove Ads Button visibility
        if (removeAdsButton != null)
        {
            bool alreadyPurchased = PlayerPrefs.GetInt("NoAdsPurchased", 0) == 1;
            removeAdsButton.SetActive(!alreadyPurchased);
        }
        if (DiceManager.instance != null)
        {
            DiceManager.instance.isSettingsOpen = true;
        }
    }

    [ContextMenu("Close")]
    public void Close()
    {
        if (currentAnimationCoroutine != null)
        {
            StopCoroutine(currentAnimationCoroutine);
        }
        currentAnimationCoroutine = StartCoroutine(AnimatePanel(downYPosition, 0f, false));

        // Ensure UI boolean is updated
        UiManager uiManager = FindObjectOfType<UiManager>();
        if (uiManager != null)
        {
            uiManager.isSettingOpen = false;
        }
        if (DiceManager.instance != null)
        {
            DiceManager.instance.isSettingsOpen = false;
        }
    }

    private void SetState(float yPos, float alpha, bool interactable)
    {
        Vector2 pos = rectTransform.anchoredPosition;
        pos.y = yPos;
        rectTransform.anchoredPosition = pos;

        canvasGroup.alpha = alpha;
        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;
    }

    private IEnumerator AnimatePanel(float targetY, float targetAlpha, bool isOpening)
    {
        float startY = rectTransform.anchoredPosition.y;
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;

        // Set immediate interactability
        canvasGroup.interactable = isOpening;
        canvasGroup.blocksRaycasts = isOpening;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationDuration);
            float curveT = animationCurve.Evaluate(t);

            // Interpolate position
            Vector2 pos = rectTransform.anchoredPosition;
            pos.y = Mathf.Lerp(startY, targetY, curveT);
            rectTransform.anchoredPosition = pos;

            // Interpolate alpha
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, curveT);

            yield return null;
        }

        // Snap to exact target values to finish cleanly
        SetState(targetY, targetAlpha, isOpening);

        // Disable game object if it is completely closed
        if (!isOpening)
        {
            gameObject.SetActive(false);
        }
        else
        {
            // After opening animation is complete, finalize UI positions
            if (UiManager.instance != null)
            {
                UiManager.instance.FinalizeSelectionPositions();
            }
        }
    }
}
