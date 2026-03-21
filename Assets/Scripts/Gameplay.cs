using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
    [Header("Wireframe Sphere")]
    public WireframeSphere wireframeSphere; 

    [Header("Earth Settings")]
    public GameObject earthSystem;
    public Transform earthCenter;  // center of the Earth
    public float earthRadius = 1f; // radius of the Earth
    public float maxEarthHP = 500f;  // starting HP
    public float currentEarthHP;    // current HP

    [Header("Lightning Mode")]
    public bool isLightningMode = false;

    [Header("Ultimate Pulse Settings")]
    public float ultimateChargeTime = 20f; // seconds to recharge
    public GameObject ultimateEffectPrefab; // visual effect for the pulse

    [Header("Ultimate Typing Challenge")]
    public int ultimateLetters = 6;
    public float typingTimeLimit = 5f;

    public List<Meteor> lightningTargets = new List<Meteor>();
    public Dictionary<char, Meteor> lightningLetterMap = new Dictionary<char, Meteor>();
    public List<Meteor> selectedMeteors = new List<Meteor>();

    public GameObject lightningLetterPrefab;
    private List<GameObject> activeLightningLabels = new List<GameObject>();

    private string ultimateSequence;
    private string playerInput = "";
    private bool awaitingUltimateInput = false;
    private float typingTimer;

    private bool ultimateReady = true;

    public static Gameplay Instance;

    void Update()
    {
        if (wireframeSphere == null)
        {
            Debug.LogWarning("WireframeSphere reference not set!");
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (ultimateReady && !awaitingUltimateInput)
            {
                StartUltimateTyping();
            }
        }

        if (awaitingUltimateInput)
        {
            HandleUltimateTyping();
        }

       
    }

    void Start()
    {
        currentEarthHP = maxEarthHP;
        if (UIManager.Instance != null)
        {
            
            UIManager.Instance.UpdateEarthHP(100f);
            UIManager.Instance.UpdateUltimateCharge(100f);
            
        }
    }

    void Awake()
    {
        Instance = this;
    }

    // Call this when a meteor hits Earth
    public void TakeDamage(float dmg)
    {
        currentEarthHP -= dmg;
        if (currentEarthHP < 0f) currentEarthHP = 0f;
        float hpPercent = (currentEarthHP / maxEarthHP) * 100f;

        Debug.Log($"Earth HP: {currentEarthHP}");

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateEarthHP(hpPercent);

        if (currentEarthHP <= 0f)
        {
            Debug.Log("Earth destroyed! Game Over.");

            if (UIManager.Instance != null)
                UIManager.Instance.ShowGameOver();

            if (earthCenter != null)
            {
                GameObject explosionPrefab = Resources.Load<GameObject>("EarthExplosion");
                if (explosionPrefab != null)
                    Instantiate(explosionPrefab, earthCenter.position, Quaternion.identity);

                Destroy(earthSystem);
            }

            if (SatelliteManager.Instance != null)
            {
                foreach (var sat in SatelliteManager.Instance.satellites)
                {
                    if (sat != null)
                        Destroy(sat.gameObject);
                }
            }
        }
    }

    void StartUltimateTyping()
    {
        awaitingUltimateInput = true;
        playerInput = "";
        typingTimer = typingTimeLimit;

        //ultimateSequence = GenerateRandomLetters(ultimateLetters);
        ultimateSequence = GenerateRandomLetters(GetDynamicUltimateLetters());
        
        Debug.Log("Type this to activate ultimate: " + ultimateSequence);

        // Slow motion effect
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowUltimatePrompt(ultimateSequence);
    }

    string GenerateRandomLetters(int length)
    {
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string result = "";

        for (int i = 0; i < length; i++)
        {
            result += letters[Random.Range(0, letters.Length)];
        }

        return result;
    }

    void HandleUltimateTyping()
    {
        typingTimer -= Time.deltaTime;

        if (typingTimer <= 0f)
        {
            FailUltimate();
            return;
        }

        foreach (char ch in Input.inputString)
        {
            if (!char.IsLetter(ch))
                continue;

            char c = char.ToUpper(ch);

            playerInput += c;

            if (UIManager.Instance != null)
                UIManager.Instance.UpdateUltimateInput(playerInput);

            if (!ultimateSequence.StartsWith(playerInput))
            {
                FailUltimate();
                return;
            }

            if (playerInput.Length >= ultimateSequence.Length)
            {
                awaitingUltimateInput = false;

                if (UIManager.Instance != null)
                    UIManager.Instance.HideUltimatePrompt();

                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;

                ActivateUltimatePulse();
            }
        }
    }

    void FailUltimate()
    {
        Debug.Log("Ultimate failed!");
        GameAnalyticsManager.Instance?.LogUltimateFail();
        awaitingUltimateInput = false;

        // Restore time
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        if (UIManager.Instance != null)
            UIManager.Instance.HideUltimatePrompt();

        StartCoroutine(UltimateRechargeRoutine());
    }

    private void ActivateUltimatePulse()
    {
        Debug.Log("Ultimate Pulse Activated!");
        ultimateReady = false;
        if (GameAnalyticsManager.Instance != null)
        {
            GameAnalyticsManager.Instance.LogUltimateUsed();
        }

        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.suppressUpgradePanel = true;

        // Spawn visual effect
        if (ultimateEffectPrefab != null && earthCenter != null)
            Instantiate(ultimateEffectPrefab, earthCenter.position, Quaternion.identity);

        Meteor[] meteors = FindObjectsOfType<Meteor>();

        foreach (var meteor in meteors)
            meteor.KillMeteor(false);

        StartCoroutine(FinishUltimate());
    }

    IEnumerator FinishUltimate()
    {
        yield return new WaitForSeconds(1.5f);

        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.suppressUpgradePanel = false;

            // Only trigger ONCE after ultimate
            if (UpgradeManager.Instance.HasQueuedUpgrade())
            {
                UpgradeManager.Instance.ConsumeQueuedUpgrade();
            }
        }

        StartCoroutine(UltimateRechargeRoutine());
    }

    int GetDynamicUltimateLetters()
    {
        int letters = ultimateLetters;

        float halfHP = maxEarthHP * 0.5f;

        if (currentEarthHP < halfHP)
        {
            float missing = halfHP - currentEarthHP;

            int extra = Mathf.FloorToInt(missing / 20f);

            letters += extra;
        }

        return letters;
    }

    private IEnumerator UltimateRechargeRoutine()
    {
        float timer = 0f;

        while (timer < ultimateChargeTime)
        {
            timer += Time.deltaTime;

            float percent = (timer / ultimateChargeTime) * 100f;

            if (UIManager.Instance != null)
                UIManager.Instance.UpdateUltimateCharge(percent);

            yield return null;
        }

        ultimateReady = true;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateUltimateCharge(100f);

        Debug.Log("Ultimate Pulse Ready!");
    }




}