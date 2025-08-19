using System.Collections.Generic;
using UnityEngine;

public struct ItemMergeData
{
    public ItemType ItemType;
    public List<Item> Items;

    public ItemMergeData(Item firstItem)
    {
        ItemType = firstItem.ItemType;

        Items = new List<Item>();
        AddItem(firstItem);
    }

    public void AddItem(Item item)
    {
        Items.Add(item);
    }
    public void RemoveItem(Item item)
    {
        Items.Remove(item);
    }

    public bool CanMergeItems()
    {
        // Check if the number of items is sufficient for merging
        return Items.Count >= 3;
    }
}
