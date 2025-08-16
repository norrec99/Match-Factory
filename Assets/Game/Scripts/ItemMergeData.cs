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
        Items.Add(firstItem);
    }
}
