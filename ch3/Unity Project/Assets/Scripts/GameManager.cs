using UnityEngine;

// Added ch3 - Update Pickup Count
public class GameManager : MonoBehaviour
{
    public UIManager UI;
    // Added ch3 - Additions to GameManager
    public Timer GameTimer;
    
    [Tooltip("Time in seconds.")]
    public int GameplayTime;
    public string WinText = "You win!";
    public string LoseText = "You lose!";

    private int _totalCollectibleItems;
    private int _collectedItemCount;

    // Singleton instance.
    public static GameManager Instance { get; private set; }
    private void Awake() => Instance = this;

    // Added ch3 - Additions to GameManager
    private void Start() => Invoke(nameof(StartTimer), 3f);
    private void StartTimer() => GameTimer.StartTimer(GameplayTime);

    private void OnEnable()
    {
        CollectItem.OnItemCollected += ItemCollected;
        // Added ch3 - Additions to GameManager
        GameTimer.OnTimeUpdate += TimeUpdated;
        // Added ch3 - Winning the Game
        GameTimer.OnTimeExpired += TimeExpired;
    }

    private void OnDisable()
    {
        CollectItem.OnItemCollected -= ItemCollected;
        // Added ch3 - Additions to GameManager
        GameTimer.OnTimeUpdate -= TimeUpdated;
        // Added ch3 - Winning the Game
        GameTimer.OnTimeExpired -= TimeExpired;
    }

    public void AddCollectibleItem() => _totalCollectibleItems++;

    private void ItemCollected()
    {
        UI.UpdateCollectedItemsText(++_collectedItemCount, _totalCollectibleItems);

        // Added ch3 - Winning the Game
        if (_collectedItemCount == _totalCollectibleItems)
            Win();
    }

    // Added ch3 - Additions to GameManager
    private void TimeUpdated(int seconds) => UI.UpdateTimerText(seconds, GameplayTime);

    // Added ch3 - Winning the Game
    private void TimeExpired() => Lose();

    // Added ch3 - Winning the Game
    private void Win()
    {
        // HACK: Used for playable build outside the scope of the book.
#if !UNITY_EDITOR || TEST_DEPLOY
        GameTimer.StopTimer();
        UI.SetGameOverText(WinText, true);
        Time.timeScale = 0f;
        return;
#endif

        GameTimer.StopTimer();
        UI.SetGameOverText(WinText);
        Time.timeScale = 0f;
    }

    // Added ch3 - Winning the Game
    private void Lose()
    {
        // HACK: Used for playable build outside the scope of the book.
#if !UNITY_EDITOR || TEST_DEPLOY
        UI.SetGameOverText(LoseText, true);
        Time.timeScale = 0f;
        return;
#endif

        UI.SetGameOverText(LoseText);
        Time.timeScale = 0f;
    }
}
