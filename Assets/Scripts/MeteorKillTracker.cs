using UnityEngine;

public class MeteorKillTracker : MonoBehaviour
{
    public static MeteorKillTracker Instance;

    int kills = 0;

    void Awake()
    {
        Instance = this;
    }

    public void AddKill()
    {
        kills++;

        if (kills % 20 == 0)
        {
            UpgradeManager.Instance.ShowUpgradeChoices();
        }
    }
}