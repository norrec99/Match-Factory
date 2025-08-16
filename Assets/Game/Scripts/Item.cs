using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Properties")]
    [SerializeField] private ItemType itemType; // Type of the item]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider itemCollider;

    [Header("Item Appearance")]
    [SerializeField] private Renderer itemRenderer;
    [SerializeField] private Material outlineMaterial;

    private Material baseMaterial;

    public ItemType ItemType => itemType; // Property to access the item type

    private ItemSpot itemSpot; // Reference to the item spot where this item is placed
    public ItemSpot ItemSpot => itemSpot; // Property to access the item spot

    private void Awake()
    {
        baseMaterial = itemRenderer.material; // Store the base material for the item
    }
    public void DisablePhysics()
    {
        // Logic for selecting the item
        if (rb != null)
        {
            rb.isKinematic = true; // Prevent physics interactions when deselected
            itemCollider.enabled = false; // Disable collider to prevent further interactions
        }
    }
    public void Select()
    {
        if (outlineMaterial != null)
        {
            itemRenderer.materials = new Material[2] { baseMaterial, outlineMaterial }; // Apply the outline material
        }
    }
    public void Deselect()
    {
        if (outlineMaterial != null)
        {
            itemRenderer.materials = new Material[1] { baseMaterial }; // Revert to the base material
        }
    }
    public void SetItemSpot(ItemSpot spot)
    {
        itemSpot = spot; // Set the item spot for this item
    }
}
