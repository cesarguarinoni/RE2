// Assets/Scripts/Localization/LocalizedText.cs
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string key;
    private TextMeshProUGUI _label;

    private void Awake()
    {
        _label = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (_label != null && !string.IsNullOrEmpty(key))
        {
            _label.text = LocalizationManager.Get(key);
        }
    }

    // Optional helper to change key at runtime
    public void SetKey(string newKey)
    {
        key = newKey;
        Refresh();
    }
}
