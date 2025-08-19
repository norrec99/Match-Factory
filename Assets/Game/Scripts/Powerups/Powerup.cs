using UnityEngine;

public abstract class Powerup : MonoBehaviour
{
    [SerializeField] private EPowerupType powerupType;

    public EPowerupType PowerupType => powerupType;
}
