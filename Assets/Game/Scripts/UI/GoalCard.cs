using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GoalCard : MonoBehaviour
{
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Image checkmarkImage;
    [SerializeField] private Image itemIcon;
    [SerializeField] private GameObject cardBackFace;

    private Sequence bumpSequence;
    private Sequence completeSequence;

    public void SetGoalCard(int amount, Sprite icon)
    {
        amountText.text = amount.ToString();
        itemIcon.sprite = icon;

    }

    public void UpdateGoalCard(int amount)
    {
        amountText.text = amount.ToString();

        AnimateBumb();
    }

    public void CompleteGoalCard()
    {
        checkmarkImage.gameObject.SetActive(true);
        amountText.gameObject.SetActive(false);

        AnimateComplete();
    }

    private void AnimateBumb()
    {
        transform.localScale = Vector3.one; // Reset scale before starting the bump animation

        bumpSequence?.Kill();
        bumpSequence = DOTween.Sequence();
        bumpSequence.Append(transform.DOScale(Vector3.one * 1.1f, 0.3f))
            .Append(transform.DOScale(Vector3.one, 0.2f))
            .SetEase(Ease.OutBack);
    }

    private void AnimateComplete()
    {
        completeSequence?.Kill();
        completeSequence = DOTween.Sequence();
        completeSequence.Append(transform.DOScale(Vector3.one * 1.2f, 1f))
            .Join(transform.DORotate(new Vector3(0, 0, 45), 0.5f))
            .Join(transform.DORotate(new Vector3(0, 0, 0), 0.5f).SetDelay(0.5f))
            .AppendInterval(0.5f)
            .Append(transform.DORotate(new Vector3(0, 180, 0), 1f))
            .Append(transform.DORotate(new Vector3(0, 360, 0), 1f))
            .Append(transform.DORotate(new Vector3(0, 540, 0), 1f))
            .Append(transform.DOScale(Vector3.zero, 1f))
            .Join(transform.GetComponent<RectTransform>().DOSizeDelta(new Vector2(0, 0), 1f))
            .SetEase(Ease.OutBack);
            
        completeSequence.OnUpdate(() =>
        {
            cardBackFace.SetActive(Vector3.Dot(Vector3.forward, transform.forward) < 0); // Toggle card back face based on rotation
        });
    }
}
