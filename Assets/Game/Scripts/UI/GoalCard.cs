using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalCard : MonoBehaviour
{
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Image checkmarkImage;

    public void SetGoalCard(int amount)
    {
        amountText.text = amount.ToString();
    }

    public void UpdateGoalCard(int amount)
    {
        amountText.text = amount.ToString();
    }

    public void CompleteGoalCard()
    {
        checkmarkImage.gameObject.SetActive(true);
        amountText.gameObject.SetActive(false);
    }
}
