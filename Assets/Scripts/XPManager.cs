using UnityEngine;

public class XPManager : MonoBehaviour
{
    public static XPManager Instance;

    public float currentXP = 0f;
    public float xpToNextLevel = 30f;
    public int level = 1;

    public float xpGrowth = 1.35f; // how fast levels scale

    void Awake()
    {
        Instance = this;
    }

    public void AddXP(float amount)
    {
        currentXP += amount;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateXPBar(currentXP / xpToNextLevel);

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;

        currentXP -= xpToNextLevel;

        xpToNextLevel *= xpGrowth;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateXPBar(currentXP / xpToNextLevel);

        UpgradeManager.Instance.ShowUpgradeChoices();
    }
}