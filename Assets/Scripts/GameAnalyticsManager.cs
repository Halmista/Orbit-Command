using UnityEngine;
using GameAnalyticsSDK;

public class GameAnalyticsManager : MonoBehaviour, IGameAnalyticsATTListener
{
    public static GameAnalyticsManager Instance;

    private bool isInitialized = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
#if UNITY_IOS
        GameAnalytics.RequestTrackingAuthorization(this);
#else
        InitializeGA();
#endif
    }

    void InitializeGA()
    {
        if (isInitialized) return;

        GameAnalytics.Initialize();
        isInitialized = true;
    }

    // ATT callbacks
    public void GameAnalyticsATTListenerNotDetermined() => InitializeGA();
    public void GameAnalyticsATTListenerRestricted() => InitializeGA();
    public void GameAnalyticsATTListenerDenied() => InitializeGA();
    public void GameAnalyticsATTListenerAuthorized() => InitializeGA();

    // =========================
    // GAME EVENTS
    // =========================

    public void StartRun()
    {
        if (!isInitialized) return;
        // Funnel Step 1: Start run
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "run", "survival", "start");
    }

    public void EndRun(float survivalTime, int meteorsAlive, bool quit = false)
    {
        if (!isInitialized) return;

        int seconds = Mathf.RoundToInt(survivalTime);

        if (quit)
        {
            // Funnel Step 3: Player quit
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "run", "survival", "quit");
        }
        else
        {
            // Funnel Step 2: Player died
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "run", "survival", "death");
        }

        // Optional: log extra info as design event
        GameAnalytics.NewDesignEvent($"player:death_meteors:{meteorsAlive}");
        GameAnalytics.NewDesignEvent($"player:survival_seconds:{seconds}");
    }

    // =========================
    // ABILITY EVENTS
    // =========================

    public void LogUpgradeUsed(string upgradeName)
    {
        if (!isInitialized) return;
        GameAnalytics.NewDesignEvent($"upgrade:used:{upgradeName}");
    }

    public void LogUltimateUsed()
    {
        if (!isInitialized) return;
        GameAnalytics.NewDesignEvent("ultimate:used");
    }

    public void LogUltimateSuccess()
    {
        if (!isInitialized) return;
        GameAnalytics.NewDesignEvent("ultimate:success");
    }

    public void LogUltimateFail()
    {
        if (!isInitialized) return;
        GameAnalytics.NewDesignEvent("ultimate:fail");
    }

    public void LogSurvivalBucket(float seconds)
    {
        if (!isInitialized) return;
        int bucket = Mathf.FloorToInt(seconds / 30f) * 30;
        GameAnalytics.NewDesignEvent($"player:survival_bucket:{bucket}");
    }
}