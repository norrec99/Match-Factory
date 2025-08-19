using System;
using UnityEngine;

public class Vacuum : MonoBehaviour
{
    public static Action OnVacuumStarted;

    private void TriggerPowerUpStarted()
    {
        OnVacuumStarted?.Invoke();
    }
}
