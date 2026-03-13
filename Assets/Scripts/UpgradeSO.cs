using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/Upgrade")]
public class UpgradeSO : ScriptableObject
{
    public string upgradeName;
    [TextArea]
    public string description;

    public enum UpgradeType
    {
        LaserDamage,
        LaserBounce,
        FireRate,
        SatelliteCount,
        ChainLightning
    }

    public UpgradeType type;

    public float value;
}