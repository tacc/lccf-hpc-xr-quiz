using UnityEngine;

// Goes on the correct drop area on the motherboard

// Stores expected item ID, the snap point, show or hide the glow object
// and tells SuperCityManager when the hardware's been placed correctly
public class PlacementDropTarget : MonoBehaviour
{
    [Header("Drop Settings")]

    // ID of the item that belongs on the target
    public string expectedItemID;


    [Header("Snap Point")]

    // Position and rotation where the item should snap
    public Transform snapPoint;


    [Header("Glow Object")]

    // Object that glows to show the user the correct placement spot
    public GameObject glowObject;


    [Header("Manager")]

    // Game manager, moves to the next phase
    public SuperCityManager superCityManager;


    // Prevents TriggerSuccess from running more than once
    private bool alreadyTriggered = false;


    void Start()
    {
        // Runs once when this object first becomes active

        // The glow only appears after the item is placed incorrectly
        if (glowObject != null)
        {
            glowObject.SetActive(false);
        }

        // If no snap point assigned, use drop target's Transform

        if (snapPoint == null)
        {
            snapPoint = transform;
        }

        if (superCityManager == null)
        {
            superCityManager = FindObjectOfType<SuperCityManager>();
        }
    }


    void OnEnable()
    {
        // Runs every time this object becomes active

        alreadyTriggered = false;

        if (glowObject != null)
        {
            glowObject.SetActive(false);
        }
    }


    public void ShowGlow()
    {
        // Turns on the glow object after item placed in wrong spot
        if (glowObject != null)
        {
            glowObject.SetActive(true);
        }
    }


    public void HideGlow()
    {
        // For when the item is placed correctly
        if (glowObject != null)
        {
            glowObject.SetActive(false);
        }
    }


    public void TriggerSuccess()
    {
        // Runs when the correct item gets placed here

        if (alreadyTriggered)
        {
            return;
        }

        alreadyTriggered = true;

        HideGlow();

        Debug.Log("Correct placement: " + expectedItemID);

        if (superCityManager == null)
        {
            superCityManager = FindObjectOfType<SuperCityManager>();
        }

        if (superCityManager != null)
        {
            // Move on to next phase
            superCityManager.OnHardwarePlaced();
        }
        else
        {
            // No supercity manager found - used to debug
            Debug.LogWarning("SuperCityManager could not be found for " + gameObject.name);
        }
    }
}