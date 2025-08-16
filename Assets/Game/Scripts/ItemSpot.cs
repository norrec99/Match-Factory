using System;
using UnityEngine;

public class ItemSpot : MonoBehaviour
{
    private Item item; // Reference to the item currently in this spot
    public Item Item => item; // Property to access the item in this spot

    public void SetItem(Item newItem)
    {
        item = newItem; // Set the item in this spot
        item.transform.SetParent(transform); // Set the item's parent to this item spot

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
}
