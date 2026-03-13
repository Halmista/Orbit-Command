using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public List<UpgradeSO> allUpgrades;

    void Awake()
    {
        Instance = this;
    }

    public List<UpgradeSO> GetRandomUpgrades(int count)
    {
        List<UpgradeSO> choices = new();

        while (choices.Count < count)
        {
            UpgradeSO rand = allUpgrades[Random.Range(0, allUpgrades.Count)];

            if (!choices.Contains(rand))
                choices.Add(rand);
        }

        return choices;
    }

   /* public void ApplyUpgrade(UpgradeSO upgrade)
    {
        switch (upgrade.type)
        {
            case UpgradeSO.UpgradeType.LaserDamage:
                LaserStats.damage += upgrade.value;
                break;

            case UpgradeSO.UpgradeType.LaserBounce:
                LaserStats.bounces += (int)upgrade.value;
                break;

            case UpgradeSO.UpgradeType.FireRate:
                SatelliteStats.fireRate *= 0.9f;
                break;

            case UpgradeSO.UpgradeType.SatelliteCount:
                SatelliteSpawner.Instance.SpawnExtraSatellite();
                break;
        }
    }*/
}