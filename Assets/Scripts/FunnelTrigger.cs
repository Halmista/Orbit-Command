using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunnelTrigger : MonoBehaviour
{
    [SerializeField] int stepNumber;
    [SerializeField] string stepName;

    public void TrackFinishFunnel()
    {
        GameAnalyticsManager.instance.FunnelFinished(stepNumber, stepName);
    }
}
