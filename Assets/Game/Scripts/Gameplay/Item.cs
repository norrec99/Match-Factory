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
    [SerializeField] private Sprite itemIcon;

    private Material baseMaterial;

    public ItemType ItemType => itemType; // Property to access the item type

    private ItemSpot itemSpot; // Reference to the item spot where this item is placed
    public ItemSpot ItemSpot => itemSpot; // Property to access the item spot
    public Sprite ItemIcon => itemIcon; // Property to access the item icon

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
    public void EnablePhysics()
    {
        // Logic for deselecting the item
        if (rb != null)
        {
            rb.isKinematic = false; // Re-enable physics interactions when selected
            itemCollider.enabled = true; // Re-enable collider to allow interactions
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
    public void UnassignItemSpot()
    {
        itemSpot = null; // Clear the item spot reference
    }
    public void ApplyRandomForce(float magnitude = 5f)
    {
        if (rb != null)
        {
            Vector3 randomDirection = Random.onUnitSphere; // Get a random direction
            rb.AddForce(randomDirection * magnitude, ForceMode.VelocityChange);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f); // Draw a wire sphere around the item for visualization
    }
}
