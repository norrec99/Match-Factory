using System;
using UnityEngine;

public class ItemSpotsManager : MonoBehaviour
{
    [SerializeField] private Transform itemSpotsParent;
    [SerializeField] private Vector3 itemLocalPosition;
    [SerializeField] private Vector3 itemLocalScale;

    private ItemSpot[] itemSpots;


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
        MoveItemToSpot(item); // Move the item to the first available spot
    }

    private void MoveItemToSpot(Item item)
    {
        for (int i = 0; i < itemSpots.Length; i++)
        {
            if (itemSpots[i].IsEmpty())
            {
                itemSpots[i].SetItem(item); // Set the item in the empty spot
                item.transform.localPosition = itemLocalPosition; // Set the local position of the item
                item.transform.localScale = itemLocalScale; // Set the local scale of the item
                item.transform.localRotation = Quaternion.identity; // Reset the rotation of the item
                item.DisablePhysics(); // Disable physics interactions for the item
                return;
            }
        }
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
