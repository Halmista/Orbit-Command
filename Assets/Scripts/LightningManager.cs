using System.Collections.Generic;
using UnityEngine;

public class LightningManager : MonoBehaviour
{
    public static LightningManager Instance;

    public bool isLightningMode = false;

    public Dictionary<char, Meteor> letterToMeteor = new();
    public List<Meteor> selectedMeteors = new();

    public GameObject letterPrefab;

    private List<GameObject> activeLabels = new();

    void Awake()
    {
        Instance = this;
    }
}