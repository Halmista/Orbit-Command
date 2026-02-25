using System.Collections.Generic;
using UnityEngine;

public class SatelliteManager : MonoBehaviour
{
    public static SatelliteManager Instance;

    public List<SatelliteShooter> satellites = new();

    void Awake()
    {
        Instance = this;
    }

    public void RegisterSatellite(SatelliteShooter sat)
    {
        satellites.Add(sat);
    }

    public SatelliteShooter GetNearestIdleSatellite(Vector3 targetPos)
    {
        float closestDist = Mathf.Infinity;
        SatelliteShooter closest = null;

        foreach (var sat in satellites)
        {
            if (sat.IsBusy()) continue;

            float dist = Vector3.Distance(sat.transform.position, targetPos);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = sat;
            }
        }

        return closest;
    }
}