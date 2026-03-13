using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject pausePanel;

    [Header("UI References")]
    public TMP_Text earthHPText;
    public TMP_Text meteorsDestroyedText;
    public TMP_Text meteorSpawnLogText;
    public TMP_Text ultimateChargeText;
    public TMP_Text activeMeteorsText;
    public TMP_Text gameOverText;

    [Header("Survival Timer")]
    public TMP_Text survivalTimerText;

    private float survivalTime = 0f;
    //private int lastSecond = -1;

    [Header("Tactical Bars")]
    public SegmentedBar earthHPBar;
    public SegmentedBar ultimateBar;


    [Header("Game Over Flash")]
    public float flashSpeed = 3f;
    private bool gameOverActive = false;

    public GameObject restartButton;
    public GameObject gameOverPanel;

    public int destroyedMeteors;
    private int activeMeteors;   
    
    private const int maxLogLines = 10;
    private bool isPaused = false;
    private readonly System.Collections.Generic.Queue<string> spawnLogs = new();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (!gameOverActive)
        {
            survivalTime += Time.deltaTime;
            UpdateSurvivalTimer();
        }

        if (gameOverActive && gameOverText != null)
        {
            float alpha = Mathf.Abs(Mathf.Sin(Time.unscaledTime * flashSpeed));

            Color c = gameOverText.color;
            c.a = alpha;
            gameOverText.color = c;
        }
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);


        // Hide GameOver text initially
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
    
    void Start()
    {
        destroyedMeteors = 0;
        activeMeteors = 1;

        //UpdateEarthHP(100f);
        UpdateUltimateCharge(100f);
        UpdateDestroyedText();
        UpdateActiveMeteorsText();
        UpdateSurvivalTimer();
    }

    public void UpdateEarthHP(float hpPercent)
    {
        if (earthHPText != null)
            earthHPText.text = $"Earth Integrity: {hpPercent:F0}%";

        if (earthHPBar != null)
            earthHPBar.SetPercent(hpPercent / 100f);

    }

    public void IncrementDestroyedMeteors()
    {
        destroyedMeteors++;
        UpdateDestroyedText();
    }

    public void UpdateUltimateCharge(float percent)
    {
        if (ultimateChargeText != null)
            ultimateChargeText.text = $"ULTIMATE PULSE CHARGE: {percent:F0}%";
        
        if (ultimateBar != null)
            ultimateBar.SetPercent(percent / 100f);

    }

    public void LogMeteorSpawn(Vector3 pos)
    {
        string logEntry = $"meteor sighted at: X:{pos.x:F1}, Y:{pos.y:F1}, Z:{pos.z:F1}";
        spawnLogs.Enqueue(logEntry);

        while (spawnLogs.Count > maxLogLines)
            spawnLogs.Dequeue();

        if (meteorSpawnLogText != null)
            meteorSpawnLogText.text = string.Join("\n", spawnLogs);
    }

    void UpdateSurvivalTimer()
    {
        int minutes = (int)(survivalTime / 60f);
        int seconds = (int)(survivalTime % 60f);
        int milliseconds = (int)((survivalTime * 1000f) % 1000f);

        if (survivalTimerText != null)
        {
            survivalTimerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:000}";
        }
    }
    public void IncrementActiveMeteors()
    {
        activeMeteors++;
        UpdateActiveMeteorsText();
    }

    public void DecrementActiveMeteors()
    {
        activeMeteors--;
        if (activeMeteors < 0) activeMeteors = 0;
        UpdateActiveMeteorsText();
    }

    private void UpdateDestroyedText()
    {
        if (meteorsDestroyedText != null)
            meteorsDestroyedText.text = $"Meteors Destroyed: {destroyedMeteors}";
    }

    private void UpdateActiveMeteorsText()
    {
        if (activeMeteorsText != null)
            activeMeteorsText.text = $"Meteors Incoming: {activeMeteors}";
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null) {
            gameOverPanel.SetActive(true);
            gameOverActive = true;
        }

        if (restartButton != null)
            restartButton.SetActive(true);

        gameOverActive = true;

        Time.timeScale = 0f; // Freeze game
    }
    public void RestartGame()
    {
        Time.timeScale = 1f; // Unfreeze time
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (pausePanel != null)
            pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }
    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}