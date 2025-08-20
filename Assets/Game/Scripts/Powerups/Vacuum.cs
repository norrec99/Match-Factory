using System;
using UnityEngine;

public class Vacuum : Powerup
{
    [SerializeField] private Animator animator;

    public static Action OnVacuumStarted;
    public static Action OnVacuumEnded;

    private void TriggerPowerUpStarted()
    {
        OnVacuumStarted?.Invoke();
    }
    private void TriggerPowerUpEnded()
    {
        OnVacuumEnded?.Invoke();
    }

    public void Play()
    {
        animator.Play("Activate");
    }
}
