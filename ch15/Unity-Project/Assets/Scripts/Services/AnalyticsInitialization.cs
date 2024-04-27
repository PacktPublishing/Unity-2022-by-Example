/*
 * As the game developer, you are responsible for the privacy and consent of your players.
 * Data won’t be collected unless you inform the SDK that a player has consented. See the privacy page.
 *   - If a player wants to opt in, call the StartDataCollection() method.
 *   - If a player wants to opt out, call the StopDataCollection() method.
 *   - If a player wants to delete their data, call the RequestDataDeletion() method.
 * */

using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Events;

public class AnalyticsInitialization : MonoBehaviour
{
    public UnityEvent OnInitialized;

    private const string KEY_CONSENT = "AnalyticsConsent";

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        if (!PlayerPrefs.HasKey(KEY_CONSENT))
        {
            AskForConsent();
        }
        else if (PlayerPrefs.GetInt(KEY_CONSENT) == 1)
        {
            // Consent was previously given, start analytics.
            StartAnalyticsCollection();
        }
    }

    private void AskForConsent()
    {
        // ... show the player a UI element that asks for consent.
        OnInitialized?.Invoke();

        // You also need to present the user with the privacy URL. To get the privacy URL use:
        Application.OpenURL(AnalyticsService.Instance.PrivacyUrl);
    }

    private void StartAnalyticsCollection()
    {
        Debug.Log("Analytics starting data collection.");
        AnalyticsService.Instance.StartDataCollection();
    }

    public void OptIn()
    {
        PlayerPrefs.SetInt(KEY_CONSENT, 1);
        StartAnalyticsCollection();
    }

    public void OptOut()
    {
        // TODO: Ask the player for consent again after a set time. On second time asking, provide a "remember answer" option if declining again.
        PlayerPrefs.SetInt(KEY_CONSENT, 0);
        AnalyticsService.Instance.StopDataCollection();
    }

    public void RequestDataDeletion()
        => AnalyticsService.Instance.RequestDataDeletion();
}