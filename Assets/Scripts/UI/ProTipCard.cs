using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

/// <summary>
/// Pro Tip card for the Loading Screen.
/// Shows localized tips with optional images.
/// Card auto-resizes via VerticalLayoutGroup + ContentSizeFitter.
/// </summary>
public class ProTipCard : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI tipText;
    [SerializeField] private TextMeshProUGUI tapNextText;
    [SerializeField] private Image dividerImage;

    [Header("Tip Images")]
    [Tooltip("One sprite per tip key (matched by index). Empty = text-only tip.")]
    [SerializeField] private Sprite[] tipSprites;
    [SerializeField] private Image tipImageDisplay;

    [Header("Tip Keys (localization CSV keys)")]
    [SerializeField]
    private string[] tipKeys = new string[]
    {
        "tip_club", "tip_forecast", "tip_rarities", "tip_swing",
        "tip_accuracy", "tip_leaderboard", "tip_timing", "tip_view"
    };

    [Header("Settings")]
    [SerializeField] private float autoCycleInterval = 8f;
    [SerializeField] private float textFadeDuration = 0.3f; private int _currentTipIndex;
    private bool _initialized;
    private Coroutine _autoCycleCoroutine;
    private CanvasGroup _tipTextCanvasGroup;
    private LayoutElement _imageLayoutElement;

    private void Awake()
    {
        if (tipText != null)
        {
            _tipTextCanvasGroup = tipText.GetComponent<CanvasGroup>();
            if (_tipTextCanvasGroup == null)
                _tipTextCanvasGroup = tipText.gameObject.AddComponent<CanvasGroup>();
        }

        if (tipImageDisplay != null)
        {
            _imageLayoutElement = tipImageDisplay.GetComponent<LayoutElement>();
            if (_imageLayoutElement == null)
                _imageLayoutElement = tipImageDisplay.gameObject.AddComponent<LayoutElement>();
        }
    }

    private void Start()
    {
        if (!_initialized)
            Initialize();
    }

    /// <summary>
    /// Initialize with default keys (or override at runtime if needed).
    /// </summary>
    public void Initialize(string[] keys = null)
    {
        if (keys != null && keys.Length > 0)
            tipKeys = keys;

        _currentTipIndex = 0;
        _initialized = true;

        // Static texts (assuming you hook these up to your localization system)
        if (headerText != null)
        {
            var loc = headerText.GetComponent<LocalizedText>();
            if (loc != null) loc.SetKey("tip_header"); // e.g. "PRO TIP"
        }

        if (tapNextText != null)
        {
            var loc = tapNextText.GetComponent<LocalizedText>();
            if (loc != null) loc.SetKey("tip_next"); // e.g. "Tap for next tip"
        }

        ShowTip(0);
        RestartAutoCycle();
    }

    public void ShowTip(int index)
    {
        if (tipKeys == null || tipKeys.Length == 0) return;

        _currentTipIndex = Mathf.Abs(index) % tipKeys.Length;

        // Localized tip body
        if (tipText != null)
        {
            string text = tipKeys[_currentTipIndex];

            // If your localization system is in place, use it here:
            var locManager = LocalizationManager.Instance; // from your CSV-based system
            if (locManager != null)
                text = locManager.GetText(tipKeys[_currentTipIndex]);

            tipText.text = text;
        }

        // Tip image
        if (tipImageDisplay != null)
        {
            Sprite sprite = null;
            if (tipSprites != null && _currentTipIndex < tipSprites.Length)
                sprite = tipSprites[_currentTipIndex];

            if (sprite != null)
            {
                tipImageDisplay.sprite = sprite;
                tipImageDisplay.gameObject.SetActive(true);

                float nativeW = sprite.rect.width;
                float nativeH = sprite.rect.height;

                if (_imageLayoutElement != null)
                {
                    _imageLayoutElement.preferredWidth = nativeW;
                    _imageLayoutElement.preferredHeight = nativeH;
                }

                var rt = tipImageDisplay.rectTransform;
                rt.sizeDelta = new Vector2(nativeW, nativeH);
                tipImageDisplay.preserveAspect = true;
            }
            else
            {
                tipImageDisplay.gameObject.SetActive(false);
            }
        }

        // Force layout so the card resizes to fit new content
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void NextTip()
    {
        if (tipKeys == null || tipKeys.Length == 0) return;
        StartCoroutine(CrossfadeToTip((_currentTipIndex + 1) % tipKeys.Length));
    }

    private IEnumerator CrossfadeToTip(int index)
    {
        if (_tipTextCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < textFadeDuration)
            {
                elapsed += Time.deltaTime;
                _tipTextCanvasGroup.alpha = 1f - (elapsed / textFadeDuration);
                yield return null;
            }
        }

        ShowTip(index);

        if (_tipTextCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < textFadeDuration)
            {
                elapsed += Time.deltaTime;
                _tipTextCanvasGroup.alpha = elapsed / textFadeDuration;
                yield return null;
            }
            _tipTextCanvasGroup.alpha = 1f;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        NextTip();
        RestartAutoCycle();
    }

    private void RestartAutoCycle()
    {
        if (_autoCycleCoroutine != null)
            StopCoroutine(_autoCycleCoroutine);

        _autoCycleCoroutine = StartCoroutine(AutoCycleRoutine());
    }

    private IEnumerator AutoCycleRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoCycleInterval);
            NextTip();
        }
    }

    private void OnDisable()
    {
        if (_autoCycleCoroutine != null)
            StopCoroutine(_autoCycleCoroutine);
    }
}
