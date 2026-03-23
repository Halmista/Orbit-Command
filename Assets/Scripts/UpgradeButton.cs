using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public TMP_Text label;
    private UpgradeOption option;

    public void Setup(UpgradeOption newOption)
    {
        option = newOption;
        label.text = newOption.name;

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => option.action.Invoke());
    }

    // ✅ Add this method to simulate a click via keyboard
    public void Click()
    {
        option?.action.Invoke();
    }
}