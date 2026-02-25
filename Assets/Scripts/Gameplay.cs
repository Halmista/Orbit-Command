using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
    [Header("Wireframe Sphere")]
    public WireframeSphere wireframeSphere; // assign your WireframeSphere

    [Header("Earth Settings")]
    public Transform earthCenter;  // center of the Earth
    public float earthRadius = 1f; // radius of the Earth
    public float earthHP = 500f;  // starting HP

    [Header("Ultimate Pulse Settings")]
    public float ultimateChargeTime = 20f; // seconds to recharge
    public GameObject ultimateEffectPrefab; // visual effect for the pulse

    private bool ultimateReady = true;

    void Update()
    {
        if (wireframeSphere == null)
        {
            Debug.LogWarning("WireframeSphere reference not set!");
            return;
        }

        // Example: Activate ultimate with the spacebar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (ultimateReady)
            {
                ActivateUltimatePulse();
            }
            else
            {
                Debug.Log("Ultimate not ready yet!");
            }
        }
    }

    void Start()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateEarthHP(earthHP);
            UIManager.Instance.UpdateUltimateCharge(100f);
        }
    }

    // Call this when a meteor hits Earth
    public void TakeDamage(float dmg)
    {
        earthHP -= dmg;
        if (earthHP < 0f) earthHP = 0f;

        Debug.Log($"Earth HP: {earthHP}");

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateEarthHP(earthHP);

        if (earthHP <= 0f)
        {
            Debug.Log("Earth destroyed! Game Over.");

            if (UIManager.Instance != null)
                UIManager.Instance.ShowGameOver();

            if (earthCenter != null)
            {
                GameObject explosionPrefab = Resources.Load<GameObject>("EarthExplosion");
                if (explosionPrefab != null)
                    Instantiate(explosionPrefab, earthCenter.position, Quaternion.identity);

                Destroy(earthCenter.gameObject);
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

    private void ActivateUltimatePulse()
    {
        Debug.Log("Ultimate Pulse Activated!");
        ultimateReady = false;

        // Spawn visual effect at Earth
        if (ultimateEffectPrefab != null && earthCenter != null)
            Instantiate(ultimateEffectPrefab, earthCenter.position, Quaternion.identity);

        // Destroy all meteors in the scene
        Meteor[] meteors = FindObjectsOfType<Meteor>();
        int destroyedByUlt = meteors.Length;
        foreach (var meteor in meteors)
        {
            meteor.KillMeteor(true);
        }
       
        // Start recharge
        StartCoroutine(UltimateRechargeRoutine());
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