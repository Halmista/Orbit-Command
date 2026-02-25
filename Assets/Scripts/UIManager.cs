using UnityEngine;
using TMPro;

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
    private int destroyedMeteors;
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
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);


        // Hide GameOver text initially
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
    }
    public void ShowGameOver()
    {
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(true);
    }
    void Start()
    {
        destroyedMeteors = 0;
        activeMeteors = 1;

        //UpdateEarthHP(100f);
        UpdateUltimateCharge(100f);
        UpdateDestroyedText();
        UpdateActiveMeteorsText();
    }

    public void UpdateEarthHP(float hp)
    {
        if (earthHPText != null)
            earthHPText.text = $"Earth HP: {hp:F0}";
    }

    public void IncrementDestroyedMeteors()
    {
        destroyedMeteors++;
        UpdateDestroyedText();
    }

   

    public void UpdateUltimateCharge(float percent)
    {
        if (ultimateChargeText != null)
            ultimateChargeText.text = $"ULTIMATE: {percent:F0}%";
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