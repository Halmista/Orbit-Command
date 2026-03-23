using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public GameObject upgradePanel;

    [Header("Upgrade Buttons")]
    public UpgradeButton[] upgradeButtons;

    //private int killsSinceUpgrade = 0;
    //private int killsRequired = 20;

    public bool suppressUpgradePanel = false;
    bool upgradeQueued = false;

    private List<UpgradeOption> allUpgrades = new List<UpgradeOption>();

    void Update()
    {
        if (upgradePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && upgradeButtons.Length >= 1)
                upgradeButtons[0].Click();
            if (Input.GetKeyDown(KeyCode.Alpha2) && upgradeButtons.Length >= 2)
                upgradeButtons[1].Click();
            if (Input.GetKeyDown(KeyCode.Alpha3) && upgradeButtons.Length >= 3)
                upgradeButtons[2].Click();
        }
    }


    void Awake()
    {
        Instance = this;

        
        allUpgrades.Add(new UpgradeOption("Add Satellite", AddSatellite));
        allUpgrades.Add(new UpgradeOption("Laser Bounce +1", AddBounce));
        allUpgrades.Add(new UpgradeOption("Laser Damage +5", AddDamage));
        allUpgrades.Add(new UpgradeOption("+50 Max Earth HP", AddMaxHealth));
        allUpgrades.Add(new UpgradeOption("Heal Earth 20%", HealEarth));
        allUpgrades.Add(new UpgradeOption("Ultimate Charges Faster", FasterUltimate));
        allUpgrades.Add(new UpgradeOption("Reduced Ultimate Letters", ReduceUltimateLetters));
        allUpgrades.Add(new UpgradeOption("Increase Fire Rate", IncreaseFireRate));
    }

    

    public void ShowUpgradeChoices()
    {
        Time.timeScale = 0f;
        upgradePanel.SetActive(true);

        List<UpgradeOption> shuffled = new List<UpgradeOption>(allUpgrades);

        for (int i = 0; i < shuffled.Count; i++)
        {
            int rand = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[rand]) = (shuffled[rand], shuffled[i]);
        }

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            UpgradeOption option = shuffled[i];
            upgradeButtons[i].Setup(option);
            //GameAnalyticsManager.Instance?.LogUpgradeOffered(option.name);
        }
    }

    public void TriggerUpgrade()
    {
        // If already showing upgrade, ignore
        if (upgradePanel.activeSelf)
            return;

        if (suppressUpgradePanel)
        {
            upgradeQueued = true;
            return;
        }

        ShowUpgradeChoices();
    }
    void ResumeGame()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1f;

        // If something queued while panel was open, show next
        if (upgradeQueued)
        {
            upgradeQueued = false;
            ShowUpgradeChoices();
        }
    }

    public bool HasQueuedUpgrade()
    {
        return upgradeQueued;
    }

    public void ConsumeQueuedUpgrade()
    {
        upgradeQueued = false;
        ShowUpgradeChoices();
    }

    // ----------------------
    // UPGRADES
    // ----------------------

    public void IncreaseFireRate()
    {
        GameAnalyticsManager.Instance?.LogUpgradeUsed("FireRate");

        SatelliteShooter[] satellites = FindObjectsOfType<SatelliteShooter>();

        foreach (var sat in satellites)
        {
            sat.fireRate *= 0.85f; // 🔥 15% faster firing
            sat.fireRate = Mathf.Max(0.1f, sat.fireRate); // clamp so it doesn't go crazy
        }

        ResumeGame();
    }
    public void AddSatellite()
    {
        //GameAnalyticsManager.Instance?.LogUpgradeSelected("AddSatellite");
        GameAnalyticsManager.Instance?.LogUpgradeUsed("AddSatellite");
        SatelliteSpawner.Instance.SpawnExtraSatellite();
        ResumeGame();
    }

    public void AddBounce()
    {
        //GameAnalyticsManager.Instance?.LogUpgradeSelected("LaserBounce");
        GameAnalyticsManager.Instance?.LogUpgradeUsed("LaserBounce");
        LaserStats.bounces += 1;
        ResumeGame();
    }

    public void AddDamage()
    {
        //GameAnalyticsManager.Instance?.LogUpgradeSelected("LaserDamage");
        GameAnalyticsManager.Instance?.LogUpgradeUsed("LaserDamage");
        LaserStats.damage += 5;
        ResumeGame();
    }

    public void AddMaxHealth()
    {
        //GameAnalyticsManager.Instance?.LogUpgradeSelected("MaxHealth");
        GameAnalyticsManager.Instance?.LogUpgradeUsed("MaxHealth");
        Gameplay.Instance.maxEarthHP += 50f;
        Gameplay.Instance.currentEarthHP += 50f;

        float percent = (Gameplay.Instance.currentEarthHP / Gameplay.Instance.maxEarthHP) * 100f;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateEarthHP(percent);

        ResumeGame();
    }


    public void HealEarth()
    {
        //GameAnalyticsManager.Instance?.LogUpgradeSelected("HealEarth");
        GameAnalyticsManager.Instance?.LogUpgradeUsed("HealEarth");
        Gameplay g = Gameplay.Instance;

        float healAmount = g.maxEarthHP * 0.2f;
        g.currentEarthHP = Mathf.Min(g.currentEarthHP + healAmount, g.maxEarthHP);

        float percent = (g.currentEarthHP / g.maxEarthHP) * 100f;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateEarthHP(percent);

        ResumeGame();
    }


    public void FasterUltimate()
    {
        //GameAnalyticsManager.Instance?.LogUpgradeSelected("FasterUltimate");
        GameAnalyticsManager.Instance?.LogUpgradeUsed("FasterUltimate");
        Gameplay.Instance.ultimateChargeTime *= 0.8f;
        ResumeGame();
    }

    public void ReduceUltimateLetters()
    {
        //GameAnalyticsManager.Instance?.LogUpgradeSelected("ReduceUltimateLetters");
        GameAnalyticsManager.Instance?.LogUpgradeUsed("ReduceUltimateLetters");
        Gameplay.Instance.ultimateLetters =
            Mathf.Max(3, Gameplay.Instance.ultimateLetters - 1);

        ResumeGame();
    }
}