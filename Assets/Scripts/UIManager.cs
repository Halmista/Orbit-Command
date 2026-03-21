using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public TMP_Text satelliteCountText;

    [Header("XP UI")]
    public RectTransform xpFill;
    public Image xpFillImage;


    [Header("Ultimate Typing Prompt")]
    public GameObject ultimatePromptPanel;
    public TMP_Text ultimatePromptText;
    public TMP_Text ultimateTypedText;

    [Header("Survival Timer")]
    public TMP_Text survivalTimerText;

    private float survivalTime = 0f;
    //private int lastSecond = -1;

    [Header("Tactical Bars")]
    public SegmentedBar earthHPBar;
    public SegmentedBar ultimateBar;
    public SegmentedBar xpSegmentedBar;


    [Header("Game Over Flash")]
    public float flashSpeed = 3f;
    private bool gameOverActive = false;

    public GameObject restartButton;
    public GameObject gameOverPanel;
    public GameObject ultimateOverlay;

    public int destroyedMeteors;
    //private int activeMeteors;
    [SerializeField] private int activeMeteors;
    public int ActiveMeteors => activeMeteors;

    public bool ultimateTypingActive = false;

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

        
        UpdateUltimateCharge(100f);
        UpdateDestroyedText();
        UpdateActiveMeteorsText();
        UpdateSurvivalTimer();
    }

    public void UpdateEarthHP(float hpPercent)
    {
        if (earthHPText != null)
            earthHPText.text = $"{hpPercent:F0}%";

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
            ultimateChargeText.text = $"{percent:F0}%";
        
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

    public void UpdateSatelliteCount(int count)
    {
        if (satelliteCountText != null)
            satelliteCountText.text = $"Satellites Active: {count}";
    }

    public void UpdateXPBar(float percent)
    {
        percent = Mathf.Clamp01(percent);

        
        xpFill.localScale = new Vector3(percent, 1f, 1f);

        
        xpFill.anchoredPosition = new Vector2(-(1f - percent) * 0.5f * xpFill.rect.width, 0f);

        // Gradient color
        if (xpFillImage != null)
        {
            xpFillImage.color = Color.Lerp(Color.yellow, Color.cyan, percent);
        }
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

    public void ShowUltimatePrompt(string sequence)
    {
        ultimateTypingActive = true;

        if (ultimateOverlay != null)
            ultimateOverlay.SetActive(true);

        if (ultimatePromptPanel != null)
            ultimatePromptPanel.SetActive(true);

        if (ultimatePromptText != null)
        {
            string spaced = "";
            foreach (char c in sequence)
                spaced += c + " ";

            ultimatePromptText.text = spaced.Trim();
        }

        if (ultimateTypedText != null)
            ultimateTypedText.text = "";
    }

    public void UpdateUltimateInput(string typed)
    {
        if (ultimatePromptText == null) return;

        string sequence = ultimatePromptText.text.Replace("<color=#00FFFF>", "")
                                                 .Replace("</color>", "")
                                                 .Replace(" ", "");

        string result = "";

        for (int i = 0; i < sequence.Length; i++)
        {
            char c = sequence[i];

            if (i < typed.Length)
                result += $"<color=#00FFFF>{c}</color> ";
            else
                result += c + " ";
        }

        ultimatePromptText.text = result.Trim();
    }

    public void HideUltimatePrompt()
    {
        ultimateTypingActive = false;

        if (ultimateOverlay != null)
            ultimateOverlay.SetActive(false);

        if (ultimatePromptPanel != null)
            ultimatePromptPanel.SetActive(false);
    }


    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}