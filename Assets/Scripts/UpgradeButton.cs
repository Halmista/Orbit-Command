using UnityEngine;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;

    UpgradeSO upgrade;

    public void Setup(UpgradeSO data)
    {
        upgrade = data;
        titleText.text = data.upgradeName;
        descText.text = data.description;
    }

    public void ChooseUpgrade()
    {
       // UpgradeManager.Instance.ApplyUpgrade(upgrade);
        Time.timeScale = 1f;
        transform.parent.gameObject.SetActive(false);
    }
}