using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpotsManager : MonoBehaviour
{
    [SerializeField] private Transform itemSpotsParent;
    [SerializeField] private Vector3 itemLocalPosition;
    [SerializeField] private Vector3 itemLocalScale;

    private ItemSpot[] itemSpots;

    private bool isBusy;

    private Dictionary<ItemType, ItemMergeData> itemMergeDataDictionary = new Dictionary<ItemType, ItemMergeData>();


    private void Awake()
    {
        ListenEvents();

        StoreItemSpots();
    }

    private void ListenEvents()
    {
        InputManager.OnItemSelected += OnItemClicked;
    }

    private void OnItemClicked(Item item)
    {
        if (isBusy)
        {
            Debug.LogWarning("ItemSpotsManager is busy, cannot handle item click.");
            return;
        }

        if (item == null)
        {
            Debug.LogWarning("Item is null, cannot select.");
            return;
        }

        if (!IsFreeSpotAvailable())
        {
            Debug.LogWarning("No free item spots available.");
            return;
        }

        isBusy = true;

        HandleItemClicked(item); // Handle the item click event
    }

    private void StoreItemSpots()
    {
        itemSpots = new ItemSpot[itemSpotsParent.childCount]; // Initialize the array with the number of child item spots

        for (int i = 0; i < itemSpotsParent.childCount; i++)
        {
            Transform child = itemSpotsParent.GetChild(i);
            if (child.TryGetComponent(out ItemSpot itemSpot))
            {
                itemSpots[i] = itemSpot; // Store the item spot in the array
            }
        }
    }

    private void HandleItemClicked(Item item)
    {
        if (itemMergeDataDictionary.ContainsKey(item.ItemType))
        {
            HandleItemMergeDataFound(item);
        }
        else
        {
            MoveItemToFirstFreeSpot(item); // Move the item to the first available spot
        }
    }

    private void HandleItemMergeDataFound(Item item)
    {
        ItemSpot idealSpot = GetIdealSpotForItem(item);

        itemMergeDataDictionary[item.ItemType].AddItem(item); // Add the item to the existing ItemMergeData

        TryMoveItemToIdealSpot(item, idealSpot);
    }

    private void TryMoveItemToIdealSpot(Item item, ItemSpot idealSpot)
    {
        if (!idealSpot.IsEmpty())
        {
            HandleIdealSpotOccupied(item, idealSpot);
            return;
        }

        MoveItemToSpot(item, idealSpot); // Move the item to the ideal spot
    }

    private void HandleIdealSpotOccupied(Item item, ItemSpot idealSpot)
    {
        throw new NotImplementedException();
    }

    private void MoveItemToSpot(Item item, ItemSpot targetSpot)
    {
        targetSpot.SetItem(item); // Set the item in the target item spot

        item.transform.localPosition = itemLocalPosition; // Set the local position of the item
        item.transform.localScale = itemLocalScale; // Set the local scale of the item
        item.transform.localRotation = Quaternion.identity; // Reset the rotation of the item

        item.DisablePhysics(); // Disable physics interactions for the item
        
        HandleItemReachedSpot(item); // Handle the first item reaching the spot
    }

    private void HandleItemReachedSpot(Item item)
    {
        if (itemMergeDataDictionary[item.ItemType].CanMergeItems())
        {
            MergeItems(itemMergeDataDictionary[item.ItemType]);
        }
        else
        {
            CheckForGameOver();
        }
    }

    private void MergeItems(ItemMergeData itemMergeData)
    {
        List<Item> itemsToMerge = itemMergeData.Items;

        itemMergeDataDictionary.Remove(itemMergeData.ItemType); // Remove the merge data for the dictionary

        for (int i = 0; i < itemsToMerge.Count; i++)
        {
            itemsToMerge[i].ItemSpot.ClearItem(); // Clear the item spot for each item being merged

            Destroy(itemsToMerge[i].gameObject); // Destroy the item game object
        }

        isBusy = false; // Reset the busy state after merging items
    }

    private ItemSpot GetIdealSpotForItem(Item item)
    {
        List<Item> itemsInMergeData = itemMergeDataDictionary[item.ItemType].Items;
        List<ItemSpot> itemSpotsWithSameType = new List<ItemSpot>();

        for (int i = 0; i < itemsInMergeData.Count; i++)
        {
            itemSpotsWithSameType.Add(itemsInMergeData[i].ItemSpot); // Collect item spots that have the same item type
        }

        if (itemSpotsWithSameType.Count >= 2)
        {
            itemSpotsWithSameType.Sort((a, b) => b.transform.GetSiblingIndex().CompareTo(a.transform.GetSiblingIndex())); // Sort by sibling index in descending order
        }
        int idealSpotIndex = itemSpotsWithSameType[0].transform.GetSiblingIndex() + 1; // Get the next sibling index for the ideal spot

        return itemSpots[idealSpotIndex]; // Return the item spot at the ideal index
    }

    private void MoveItemToFirstFreeSpot(Item item)
    {
        ItemSpot targetItemSpot = GetFreeSpot();

        if (targetItemSpot == null)
        {
            Debug.LogWarning("No free item spot found, cannot move item.");
            return;
        }

        CreateItemMergeData(item); // Create or update the item merge data

        targetItemSpot.SetItem(item); // Set the item in the target item spot

        item.transform.localPosition = itemLocalPosition; // Set the local position of the item
        item.transform.localScale = itemLocalScale; // Set the local scale of the item
        item.transform.localRotation = Quaternion.identity; // Reset the rotation of the item

        item.DisablePhysics(); // Disable physics interactions for the item
        
        HandleFirstItemReachedSpot(item); // Handle the first item reaching the spot
    }

    private void HandleFirstItemReachedSpot(Item item)
    {
        CheckForGameOver();
    }

    private void CheckForGameOver()
    {
        if (!IsFreeSpotAvailable())
        {
            Debug.LogWarning("Game Over: No free item spots available.");
        }
        else
        {
            isBusy = false; // Reset the busy state if there are free spots available
        }
    }

    private void CreateItemMergeData(Item item)
    {
        itemMergeDataDictionary.Add(item.ItemType, new ItemMergeData(item)); // Create a new ItemMergeData with the item
    }

    private ItemSpot GetFreeSpot()
    {
        for (int i = 0; i < itemSpots.Length; i++)
        {
            if (itemSpots[i].IsEmpty())
            {
                return itemSpots[i]; // Return the first empty item spot found
            }
        }
        return null; // Return null if no empty item spots are found
    }

    private bool IsFreeSpotAvailable()
    {
        for (int i = 0; i < itemSpots.Length; i++)
        {
            if (itemSpots[i].IsEmpty())
            {
                return true; // Return true if an empty item spot is found
            }
        }
        return false; // Return false if no empty item spots are found
    }

    private void UnsubscribeEvents()
    {
        InputManager.OnItemSelected -= OnItemClicked;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
