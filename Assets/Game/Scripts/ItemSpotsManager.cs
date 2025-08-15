using System;
using UnityEngine;

public class ItemSpotsManager : MonoBehaviour
{
    [SerializeField] private Transform itemSpotParent;
    [SerializeField] private Vector3 itemLocalPosition;
    [SerializeField] private Vector3 itemLocalScale;


    private void Awake()
    {
        ListenEvents();
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

        item.transform.SetParent(itemSpotParent); // Set the item's parent to the item spot

        item.transform.localPosition = itemLocalPosition; // Set the local position of the item
        item.transform.localScale = itemLocalScale; // Set the local scale of the item

        item.DisablePhysics(); // Call the Select method on the item when selected
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
