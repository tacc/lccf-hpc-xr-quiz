using UnityEngine;
using UnityEngine.EventSystems;
using zSpace.Core.EventSystems;
using zSpace.Core.Input;


public class ZDraggableItem :
    ZPointerInteractable, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Answer Setup")]
    public string itemID;

    [Header("Effects")]
    public GameObject heatsinkSmoke;


    [Header("Drag Plane")]
    public Transform PlaneQuadTransform;


    private Vector3 originalPosition;
    private Quaternion originalRotation;


    private Vector3 initialGrabOffset = Vector3.zero;
    private bool originalIsKinematic = false;


    private ZDropTarget currentHoverTarget;
    private OrbitAroundTarget orbitScript;


    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;


        orbitScript = GetComponent<OrbitAroundTarget>();
    }


    public override ZPointer.DragPolicy GetDragPolicy(ZPointer pointer)
    {
        return ZPointer.DragPolicy.LockToCustomPlane;
    }


    public override Plane GetDragPlane(ZPointer pointer)
    {
        if (PlaneQuadTransform != null)
        {
            return new Plane(
                PlaneQuadTransform.forward,
                PlaneQuadTransform.position);
        }


        return base.GetDragPlane(pointer);
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        ZPointerEventData pointerEventData = eventData as ZPointerEventData;


        if (pointerEventData == null ||
            pointerEventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }


        if (orbitScript != null)
        {
            orbitScript.enabled = false;
        }


        Pose pose = pointerEventData.Pointer.EndPointWorldPose;


        initialGrabOffset =
            Quaternion.Inverse(transform.rotation) *
            (transform.position - pose.position);


        Rigidbody rb = GetComponent<Rigidbody>();


        if (rb != null)
        {
            originalIsKinematic = rb.isKinematic;
            rb.isKinematic = true;
        }


        pointerEventData.Pointer.CapturePointer(gameObject);
    }


    public void OnDrag(PointerEventData eventData)
    {
        ZPointerEventData pointerEventData = eventData as ZPointerEventData;


        if (pointerEventData == null ||
            pointerEventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }


        Pose pose = pointerEventData.Pointer.EndPointWorldPose;


        transform.position =
            pose.position +
            (transform.rotation * initialGrabOffset);
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        ZPointerEventData pointerEventData = eventData as ZPointerEventData;


        if (pointerEventData == null ||
            pointerEventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }


        pointerEventData.Pointer.CapturePointer(null);


        Rigidbody rb = GetComponent<Rigidbody>();


        if (rb != null)
        {
            rb.isKinematic = originalIsKinematic;
        }


        if (currentHoverTarget != null)
        {
            if (currentHoverTarget.expectedItemID == itemID)
            {
                currentHoverTarget.TriggerSuccess();

                if (heatsinkSmoke != null)
                {
                    heatsinkSmoke.SetActive(false);
                }

                gameObject.SetActive(false);
            }
            else
            {
                currentHoverTarget.TriggerFailure();
                ResetPosition();
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        ZDropTarget zone = other.GetComponent<ZDropTarget>();


        if (zone != null)
        {
            currentHoverTarget = zone;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        ZDropTarget zone = other.GetComponent<ZDropTarget>();


        if (zone != null && currentHoverTarget == zone)
        {
            currentHoverTarget = null;
        }
    }


    public void SnapToTarget(Transform targetTransform)
    {
        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;


        if (orbitScript != null)
        {
            orbitScript.enabled = false;
        }


        Destroy(this);
    }


    public void ResetPosition()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;


        if (orbitScript != null)
        {
            orbitScript.enabled = true;
        }
    }
}


