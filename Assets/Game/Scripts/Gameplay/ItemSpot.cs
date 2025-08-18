using System;
using UnityEngine;
using DG.Tweening;

public class ItemSpot : MonoBehaviour
{
    [SerializeField] private Transform itemParent;

    private Item item; // Reference to the item currently in this spot
    public Item Item => item; // Property to access the item in this spot

    private Sequence sequence;

    public void SetItem(Item newItem)
    {
        item = newItem; // Set the item in this spot
        item.transform.SetParent(itemParent); // Set the item's parent to this item spot

        item.SetItemSpot(this); // Set the item spot reference in the item
    }

    public void ClearItem()
    {
        item = null; // Clear the item reference
    }

    public bool IsEmpty()
    {
        return item == null; // Check if the item spot is empty
    }

    public void PlayBumpAnimation(float duration = 0.1f)
    {
        if (itemParent == null) return;

        sequence?.Kill(true);
        sequence = DOTween.Sequence();
        Vector3 startPos = itemParent.localPosition;
        float bumpHeight = -0.03f; // How much to bump up (customize as needed)
        sequence.Append(itemParent.DOLocalMoveY(startPos.y + bumpHeight, duration).SetEase(Ease.OutQuad));
        sequence.Append(itemParent.DOLocalMoveY(startPos.y, duration).SetEase(Ease.InQuad));
    }
}
