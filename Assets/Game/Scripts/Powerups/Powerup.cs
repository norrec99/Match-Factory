using TMPro;
using UnityEngine;

public abstract class Powerup : MonoBehaviour
{
    [SerializeField] private EPowerupType powerupType;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private GameObject adIcon;

    public EPowerupType PowerupType => powerupType;

    public void UpdateVisuals(int amount)
    {
        adIcon.SetActive(amount <= 0);
        
        amountText.gameObject.SetActive(amount > 0);
        amountText.text = amount.ToString();
    }
}
