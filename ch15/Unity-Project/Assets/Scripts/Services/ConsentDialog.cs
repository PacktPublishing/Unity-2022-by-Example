using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConsentDialog : MonoBehaviour
{
    public UnityEvent OnAcceptClicked;
    public UnityEvent OnDeclineClicked;

    private const string TEXT_DIALOG = "Opt-in to data collection for analytics?";
    private const string TEXT_ACCEPT = "Accept";
    private const string TEXT_DECLINE = "Decline";
    private Canvas _canvas;
    private GameObject _dialogPanel;
    private Button _acceptButton;
    private Button _declineButton;

    private void Start()
    {
        _canvas = FindObjectsOfType<Canvas>().FirstOrDefault(c => c.renderMode == RenderMode.ScreenSpaceOverlay);
        if (_canvas == null)
        {
            Debug.LogError("No Canvas found in the scene!");
            return;
        }
    }

    public void ShowConsentDialog()
    {
        if (_canvas == null)
            return;

        _dialogPanel = new GameObject("ConsentDialog");

        var panelRectTransform = _dialogPanel.AddComponent<RectTransform>();
        _dialogPanel.AddComponent<CanvasRenderer>();

        var panelImage = _dialogPanel.AddComponent<Image>();
        panelImage.color = new Color(0.8f, 0.8f, 0.8f, 0.9f);
        panelRectTransform.SetParent(_canvas.transform, false);
        panelRectTransform.sizeDelta = new Vector2(300, 200);
        panelRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        panelRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        panelRectTransform.pivot = new Vector2(0.5f, 0.5f);
        panelRectTransform.anchoredPosition = Vector2.zero;

        CreateDialogLabel(TEXT_DIALOG);

        _acceptButton = CreateButton("AcceptButton", TEXT_ACCEPT, new Vector2(-75, -75), AcceptClicked);
        _declineButton = CreateButton("DeclineButton", TEXT_DECLINE, new Vector2(75, -75), DeclineClicked);
    }

    private void CreateDialogLabel(string labelText)
    {
        var labelObj = new GameObject("DialogLabel");
        var rectTransform = labelObj.AddComponent<RectTransform>();
        rectTransform.SetParent(_dialogPanel.transform, false);
        rectTransform.sizeDelta = new Vector2(280, 100);
        rectTransform.anchoredPosition = new Vector2(0, 50);

        var txt = labelObj.AddComponent<TextMeshProUGUI>();
        txt.text = labelText;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.black;
        txt.fontSize = 24;
    }

    private Button CreateButton(string name, string buttonText, Vector2 position, UnityEngine.Events.UnityAction clickAction)
    {
        var buttonObj = new GameObject(name);
        var rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100, 50);

        var button = buttonObj.AddComponent<Button>();
        var buttonImage = buttonObj.AddComponent<Image>();
        button.targetGraphic = buttonImage;
        button.colors = ColorBlock.defaultColorBlock;

        var textObj = new GameObject("Text");
        var textRectTransform = textObj.AddComponent<RectTransform>();
        textRectTransform.SetParent(buttonObj.transform, false);
        textRectTransform.sizeDelta = new Vector2(100, 50);

        var txt = textObj.AddComponent<TextMeshProUGUI>();
        txt.text = buttonText;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.black;
        txt.enableAutoSizing = true;

        buttonObj.transform.SetParent(_dialogPanel.transform, false);
        rectTransform.anchoredPosition = position;
        button.onClick.AddListener(clickAction);

        return button;
    }

    private void AcceptClicked()
    {
        Debug.Log("Player accepted!");
        
        OnAcceptClicked?.Invoke();
        Destroy(_dialogPanel);
    }

    private void DeclineClicked()
    {
        Debug.Log("Player declined!");
        
        OnDeclineClicked?.Invoke();
        Destroy(_dialogPanel);
    }
}