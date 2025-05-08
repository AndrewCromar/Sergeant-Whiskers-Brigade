using UnityEngine;

public class SideCollisionIgnore : MonoBehaviour
{
    private Collider2D platformCollider;  // The main platform collider
    private Collider2D otherCollider;    // The collider of the object entering/exiting
    private bool isObjectInTrigger = false;  // Flag for object in the trigger

    private void Awake()
    {
        // Get the platform's main collider
        platformCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Disable collision with the platform when entering the side trigger
        if (platformCollider != null && other != platformCollider)
        {
            Physics2D.IgnoreCollision(platformCollider, other, true);
        }
        
        // Set flag that the object is inside the trigger
        isObjectInTrigger = true;
        otherCollider = other;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Set flag that the object has left the trigger
        isObjectInTrigger = false;
        otherCollider = null;

        // If the object has left the trigger, check if it is still on the platform
        if (platformCollider != null && other != platformCollider)
        {
            // Re-enable collision when the object exits the trigger and is no longer inside the platform bounds
            if (!platformCollider.bounds.Intersects(other.bounds))
            {
                Physics2D.IgnoreCollision(platformCollider, other, false);
            }
        }
    }

    private void Update()
    {
        // If the object is not in the trigger and is still overlapping the platform collider, we can re-enable collision
        if (!isObjectInTrigger && otherCollider != null)
        {
            // Check if the object is still on the platform
            if (platformCollider != null && !platformCollider.bounds.Intersects(otherCollider.bounds))
            {
                // If it's completely out of the platform, re-enable the collision
                Physics2D.IgnoreCollision(platformCollider, otherCollider, false);
            }
        }
    }
}
