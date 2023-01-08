using UnityEngine;
using TMPro;

// Added ch3 - Update Pickup Count
public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI CollectedItemText;
    // Added ch3 - Additions to UIManager
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI GameOverText;

    // Start is called before the first frame update
    void Start()
    {
        CollectedItemText.text = string.Empty;
        // Added ch3 - Additions to UIManager
        TimerText.text = string.Empty;        
        GameOverText.text = string.Empty;        
    }

    public void UpdateCollectedItemsText(int currentItemCount, int totalCollectableItems) =>
        CollectedItemText.text = $"{currentItemCount} of {totalCollectableItems}";

    // Added ch3 - Additions to UIManager
    public void UpdateTimerText(int currentSeconds, int totalSeconds)
    {
        var ts = System.TimeSpan.FromSeconds(totalSeconds - currentSeconds);
        TimerText.text = $"{ts.Minutes}:{ts.Seconds:00}";
    }

    
    // Added ch3 - Additions to UIManager
    public void SetGameOverText(string text) => GameOverText.text = text;


    // HACK: Used for playable build outside the scope of the book.
#if !UNITY_EDITOR || TEST_DEPLOY
    public void SetGameOverText(string text, bool hasResetMessage)
    {
        GameOverText.text = text;
        if (hasResetMessage)
            TimerText.text = "Press [Esc] to Restart";
    }
#endif
}
