using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{

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
        HandleInputDrag();
        HandleInputRelease();
    }

    private void HandleInputDrag()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
            {
                if (hit.collider == null)
                {
                    currentItem = null;
                    return;
                }

                if (hit.collider.GetComponent<Item>() == null)
                {
                    currentItem = null;
                    return;
                }

                currentItem = hit.collider.GetComponent<Item>();
            }
        }
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
            currentItem = null; // Clear the current item after selection
        }
    }
}
