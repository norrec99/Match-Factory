using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Layer Mask")]
    [SerializeField] private LayerMask itemLayerMask; // Layer mask to filter raycast hits for items

    private float rayDistance = 100f; // Distance for raycasting
    private Camera mainCamera;

    private Item currentItem;

    public static Action<Item> OnItemSelected;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGame())
        {
            return; // Skip input handling if not in game state
        }
        HandleInputDrag();
        HandleInputRelease();
    }

    private void HandleInputDrag()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, itemLayerMask))
            {
                if (hit.collider == null)
                {
                    DeselectCurrentItem();
                    return;
                }

                if (!hit.collider.transform.parent.TryGetComponent(out Item item))
                {
                    DeselectCurrentItem();
                    return;
                }

                DeselectCurrentItem();

                currentItem = item;
                currentItem.Select(); // Call the Select method on the item
            }
        }
    }

    private void DeselectCurrentItem()
    {
        if (currentItem != null)
        {
            currentItem.Deselect(); // Deselect the item if it exists
        }

        currentItem = null;
    }

    private void HandleInputRelease()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (currentItem == null)
            {
                return; // No item to release
            }

            OnItemSelected?.Invoke(currentItem);
            DeselectCurrentItem();
        }
    }
}
