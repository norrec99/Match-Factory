using System;
using System.Collections.Generic;
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
        throw new NotImplementedException();
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
