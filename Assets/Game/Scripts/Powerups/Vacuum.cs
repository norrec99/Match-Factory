using System;
using UnityEngine;

public class Vacuum : Powerup
{
    [SerializeField] private Animator animator;

    public static Action OnVacuumStarted;

    private void TriggerPowerUpStarted()
    {
        OnVacuumStarted?.Invoke();
    }

    public void Play()
    {
        animator.Play("Activate");
    }
}
