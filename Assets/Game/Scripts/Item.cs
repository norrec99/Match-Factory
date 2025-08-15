using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider itemCollider;


    public void DisablePhysics()
    {
        // Logic for selecting the item
        if (rb != null)
        {
            rb.isKinematic = true; // Prevent physics interactions when deselected
            itemCollider.enabled = false; // Disable collider to prevent further interactions
        }
        Debug.Log("Item selected: " + gameObject.name);
    }
}
