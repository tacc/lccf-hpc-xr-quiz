using UnityEngine;
using zSpace.Core.Input;

public class ZDraggableItem : MonoBehaviour
{
    public string itemID;
    private Vector3 originalPosition;
    private Transform originalParent;
    private bool isDragging = false;
    private ZPointer stylusPointer;

    // This remembers which drop zone the cube is hovering over
    private ZDropTarget currentHoverTarget;

    void Start()
    {
        originalPosition = transform.position;
        originalParent = transform.parent;
        stylusPointer = FindObjectOfType<ZPointer>();
    }

    void Update()
    {
        if (stylusPointer == null) return;

        // Front button on the zSpace pen
        bool buttonDown = stylusPointer.GetButton(0);

        if (buttonDown && !isDragging)
        {
            RaycastHit hit;
            // Shoot a laser from the pen
            if (Physics.Raycast(stylusPointer.transform.position, stylusPointer.transform.forward, out hit))
            {
                if (hit.transform == this.transform)
                {
                    isDragging = true;
                    // Attach the cube to the pen
                    transform.SetParent(stylusPointer.transform, true); 
                }
            }
        }
        else if (!buttonDown && isDragging)
        {
            isDragging = false;
            transform.SetParent(originalParent);

            if (currentHoverTarget != null)
            {
                if (currentHoverTarget.expectedItemID == this.itemID)
                {
                    SnapToTarget(currentHoverTarget.transform);
                    currentHoverTarget.TriggerSuccess();
                }
                else
                {
                    currentHoverTarget.TriggerFailure();
                    ResetPosition();
                }
            }
            else
            {
                // Dropped in empty space
                ResetPosition();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ZDropTarget zone = other.GetComponent<ZDropTarget>();
        if (zone != null) currentHoverTarget = zone;
    }

    private void OnTriggerExit(Collider other)
    {
        ZDropTarget zone = other.GetComponent<ZDropTarget>();
        if (zone != null && currentHoverTarget == zone) currentHoverTarget = null;
    }

    public void SnapToTarget(Transform targetTransform)
    {
        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
        Destroy(this);
    }

    public void ResetPosition()
    {
        transform.position = originalPosition;
    }
}