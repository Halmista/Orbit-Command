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

    private List<UpgradeOption> allUpgrades = new List<UpgradeOption>();

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
    }

    /*public void RegisterMeteorKill()
    {
        killsSinceUpgrade++;

        if (killsSinceUpgrade >= killsRequired)
        {
            killsSinceUpgrade = 0;

            if (!suppressUpgradePanel)
                ShowUpgradeChoices();
        }
    }*/

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
        }
    }

    void ResumeGame()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // ----------------------
    // UPGRADES
    // ----------------------

    public void AddSatellite()
    {
        SatelliteSpawner.Instance.SpawnExtraSatellite();
        ResumeGame();
    }

    public void AddBounce()
    {
        LaserStats.bounces += 1;
        ResumeGame();
    }

    public void AddDamage()
    {
        LaserStats.damage += 5;
        ResumeGame();
    }

    public void AddMaxHealth()
    {
        Gameplay.Instance.maxEarthHP += 50f;
        Gameplay.Instance.currentEarthHP += 50f;

        float percent = (Gameplay.Instance.currentEarthHP / Gameplay.Instance.maxEarthHP) * 100f;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateEarthHP(percent);

        ResumeGame();
    }


    public void HealEarth()
    {
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
        Gameplay.Instance.ultimateChargeTime *= 0.8f;
        ResumeGame();
    }

    public void ReduceUltimateLetters()
    {
        Gameplay.Instance.ultimateLetters =
            Mathf.Max(3, Gameplay.Instance.ultimateLetters - 1);

        ResumeGame();
    }
}