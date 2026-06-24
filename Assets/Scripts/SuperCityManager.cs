using System.Collections;
using UnityEngine;

// SuperCityManager controls changing between scenes

// Plays intro, city analogy phases, audio, city animations, sliding the 
// analogies away, and placement layers
public class SuperCityManager : MonoBehaviour
{
    [Header("Game State")]

    public int currentPhase = 0;


    [Header("Intro Layer")]

    // GameObject that contains intro scene
    public GameObject introLayer;

    public float introDuration = 15f;


    [Header("City Layer")]

    // Parent object that holds all city analogy scenes
    public GameObject cityLayer;

    // Array stores each city analogy phase
    public GameObject[] cityAnalogies;


    [Header("Broken City Effect")]

    // Connects to BrokenCityPieces script
    public BrokenCityPieces brokenCityPieces;

    [Tooltip("Only this phase uses the broken city repair effect. First phase is 0, second phase is 1, third phase is 2.")]

    // First phase uses the broken city repair animation
    public int brokenCityPhaseIndex = 0;

    // Wait for broken city repair animation before transitioning
    public bool waitForCityRepairBeforePlacement = true;

    public float holdAfterRepairDuration = 3f;

    [Tooltip("The clean/original city model that appears after the broken pieces reconnect.")]

    // Full model appears after repairs
    public GameObject repairedCityModel;

    [Tooltip("The full broken city parent object that should hide after the repaired model appears.")]

    // Parent object holding the broken city version
    public GameObject brokenPlaneObject;


    [Header("Transition Flash")]

    // Optional flash effect between analogy scene and placement scene
    public GameObject transitionFlashObject;

    public float transitionFlashDuration = 0.5f;


    [Header("Placement Layer")]

    // The parent object that holds the placement scene
    public GameObject placementLayer;

    // Stores the placement scene for each phase
    public GameObject[] placementGroups;

    [Tooltip("This phase only shows the motherboard and then automatically moves on.")]

    // Motherboard phase does not wait for user input
    public int motherboardOnlyPlacementPhaseIndex = 0;


    [Header("Motherboard-Only Timer")]

    public float motherboardOnlyDuration = 3f;


    [Header("Audio")]

    // AudioSource that plays the explanation clips
    public AudioSource explanationAudioSource;

    // Stores audio clip per phase
    public AudioClip[] explanationClips;


    [Header("Timing")]

    public float pauseBeforeAudio = 0.5f;

    public float pauseAfterAudio = 0.5f;


    [Header("Analogy Slide Transition")]

    // How far the city analogy moves upward when transitioning out
    public float analogySlideUpDistance = 5f;

    // How long the upward slide takes
    public float analogySlideUpDuration = 1.2f;


    [Header("Placement Slide Transition")]

    // How low the placement group starts before sliding into view
    public float placementSlideUpDistance = 5f;

    public float placementSlideUpDuration = 1.2f;


    // Ensurese phase transition only runs once
    private bool phaseTransitionRunning = false;

    private Vector3[] originalCityAnalogyPositions;

    // Runs once when the scene begins
    void Start()
    {
        // Stores each analogy object's starting position
        originalCityAnalogyPositions = new Vector3[cityAnalogies.Length];

        // For each city analogy
        for (int i = 0; i < cityAnalogies.Length; i++)
        {
            if (cityAnalogies[i] != null)
            {
                // Used to reset world positions when analogy starts again
                originalCityAnalogyPositions[i] = cityAnalogies[i].transform.position;
            }
        }

        // Hide everything at the very beginning
        HideAll();

        // Start the intro - glowing orb
        StartCoroutine(PlayIntroThenStartSceneOne());
    }


    // Hides every major layer and phase object
    private void HideAll()
    {
        if (introLayer != null)
        {
            introLayer.SetActive(false);
        }

        if (cityLayer != null)
        {
            cityLayer.SetActive(false);
        }

        if (placementLayer != null)
        {
            placementLayer.SetActive(false);
        }

        if (repairedCityModel != null)
        {
            repairedCityModel.SetActive(false);
        }

        if (transitionFlashObject != null)
        {
            transitionFlashObject.SetActive(false);
        }

        HideAllCityAnalogies();

        HideAllPlacementGroups();
    }


    // Handles the intro sequence
    private IEnumerator PlayIntroThenStartSceneOne()
    {
        Debug.Log("Intro started.");

        // Turn on the intro layer
        if (introLayer != null)
        {
            introLayer.SetActive(true);
        }

        // How long the intro stays visible
        yield return new WaitForSeconds(introDuration);

        // Hide the intro afterwards
        if (introLayer != null)
        {
            introLayer.SetActive(false);
        }

        // Show the city layer to start the first analogy
        if (cityLayer != null)
        {
            cityLayer.SetActive(true);
        }

        Debug.Log("Intro finished. Starting first analogy.");
        
        StartAnalogyPhase(0);
    }


    // Starts a specific analogy phase
    public void StartAnalogyPhase(int phaseIndex)
    {
        // Check if the requested phase is valid
        if (cityAnalogies == null || phaseIndex < 0 || phaseIndex >= cityAnalogies.Length)
        {
            Debug.LogWarning("Invalid city analogy phase: " + phaseIndex);
            return;
        }

        currentPhase = phaseIndex;

        // Reset this to false so this new phase can transition later
        phaseTransitionRunning = false;

        // Make the city layer visible
        if (cityLayer != null)
        {
            cityLayer.SetActive(true);
        }

        // Hide placement layer
        if (placementLayer != null)
        {
            placementLayer.SetActive(false);
        }

        // Make sure the transition flash is off at the start of the phase
        if (transitionFlashObject != null)
        {
            transitionFlashObject.SetActive(false);
        }

        // Prevents two analogy scenes from being visible at the same time
        HideAllCityAnalogies();

        HideAllPlacementGroups();

        if (repairedCityModel != null)
        {
            repairedCityModel.SetActive(false);
        }

        // If this is the broken city phase, show the broken city object
        if (brokenPlaneObject != null)
        {
            brokenPlaneObject.SetActive(IsBrokenCityPhase());
        }

        GameObject currentAnalogy = cityAnalogies[currentPhase];

        if (currentAnalogy != null)
        {
            // Reset the analogy back to its original position
            currentAnalogy.transform.position = originalCityAnalogyPositions[currentPhase];

            currentAnalogy.SetActive(true);

            // Turn on all children under analogy
            foreach (Transform child in currentAnalogy.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        
        Debug.Log("Starting analogy phase " + currentPhase);
    }


    // Hides every city analogy in the cityAnalogies array
    private void HideAllCityAnalogies()
    {
        if (cityAnalogies == null)
        {
            return;
        }

        foreach (GameObject analogy in cityAnalogies)
        {
            if (analogy != null)
            {
                analogy.SetActive(false);
            }
        }
    }


    // Hides every placement group in the placementGroups array
    private void HideAllPlacementGroups()
    {
        if (placementGroups == null)
        {
            return;
        }

        foreach (GameObject group in placementGroups)
        {
            if (group != null)
            {
                group.SetActive(false);
            }
        }
    }


    // Checks if the current phase should use the broken city repair effect
    private bool IsBrokenCityPhase()
    {
        return currentPhase == brokenCityPhaseIndex && brokenCityPieces != null;
    }


    // Checks if the current placement phase should be motherboard-only
    private bool IsMotherboardOnlyPlacementPhase()
    {
        return currentPhase == motherboardOnlyPlacementPhaseIndex;
    }


    // Called when the user selects the correct answer in the city analogy layer
    public void OnAnalogySolved()
    {
        if (phaseTransitionRunning)
        {
            return;
        }

        StartCoroutine(AnalogySolvedRoutine());
    }


    // Controls what happens after the user solves an analogy
    //
    // Marks transition as running, repairs the city (if applicable), plays
    // explanation audio, slides analogy away, play transition flash, starts
    // placement layer
    private IEnumerator AnalogySolvedRoutine()
    {
        phaseTransitionRunning = true;

        Debug.Log("Correct analogy selected. Starting transition.");

        // Check if the current phase uses the broken city effect
        bool useBrokenCityEffect = IsBrokenCityPhase();

        if (useBrokenCityEffect)
        {
            // Hide the orbiting answer choices
            HideCurrentPhaseAnswerChoices();

            Debug.Log("Starting broken city repair.");

            // Call BrokenCityPieces script to begin repairing the city
            brokenCityPieces.RepairCity();
        }

        yield return new WaitForSeconds(pauseBeforeAudio);

        // Check for a valid audio source and audio clip
        if (explanationAudioSource != null &&
            explanationClips != null &&
            currentPhase >= 0 &&
            currentPhase < explanationClips.Length &&
            explanationClips[currentPhase] != null)
        {
            // Put the correct phase audio clip into the AudioSource
            explanationAudioSource.clip = explanationClips[currentPhase];

            // Play the audio
            explanationAudioSource.Play();

            yield return new WaitForSeconds(explanationClips[currentPhase].length);
        }

        if (useBrokenCityEffect && waitForCityRepairBeforePlacement)
        {
            yield return new WaitForSeconds(brokenCityPieces.repairDuration);

            // Show the clean repaired city model
            if (repairedCityModel != null)
            {
                repairedCityModel.SetActive(true);
            }

            // Hide the broken city
            if (brokenPlaneObject != null)
            {
                brokenPlaneObject.SetActive(false);
            }

            if (holdAfterRepairDuration > 0f)
            {
                Debug.Log("Holding repaired city for " + holdAfterRepairDuration + " seconds.");
                yield return new WaitForSeconds(holdAfterRepairDuration);
            }
        }

        yield return new WaitForSeconds(pauseAfterAudio);

        // Slide the current analogy upward and then hide it
        yield return StartCoroutine(SlideCurrentAnalogyUp());

        // Play the flash transition
        yield return StartCoroutine(PlayTransitionFlash());

        // Start the placement layer for this phase
        yield return StartCoroutine(StartPlacementLayer());
    }

    // Slides the current analogy upward when it is done
    private IEnumerator SlideCurrentAnalogyUp()
    {
        GameObject objectToSlide = null;

        if (repairedCityModel != null && repairedCityModel.activeSelf)
        {
            objectToSlide = repairedCityModel;
        }

        // Slide the current city analogy
        else if (cityAnalogies != null &&
                 currentPhase >= 0 &&
                 currentPhase < cityAnalogies.Length)
        {
            objectToSlide = cityAnalogies[currentPhase];
        }

        if (objectToSlide == null)
        {
            yield break;
        }

        Vector3 startPosition = objectToSlide.transform.position;

        Vector3 endPosition = startPosition + new Vector3(0f, analogySlideUpDistance, 0f);

        yield return StartCoroutine(SlideObject(
            objectToSlide,
            startPosition,
            endPosition,
            analogySlideUpDuration
        ));

        // After sliding, hide the current city analogy
        if (cityAnalogies != null &&
            currentPhase >= 0 &&
            currentPhase < cityAnalogies.Length &&
            cityAnalogies[currentPhase] != null)
        {
            cityAnalogies[currentPhase].SetActive(false);
        }

        if (repairedCityModel != null)
        {
            repairedCityModel.SetActive(false);
        }
    }


    // This turns on the transition flash for a short amount of time
    private IEnumerator PlayTransitionFlash()
    {
        // Skip id no flash object is assigned
        if (transitionFlashObject == null)
        {
            yield break;
        }

        transitionFlashObject.SetActive(true);

        yield return new WaitForSeconds(transitionFlashDuration);

        transitionFlashObject.SetActive(false);
    }


    // Starts the placement layer for the current phase
    private IEnumerator StartPlacementLayer()
    {
        // Turn on the main placement layer
        if (placementLayer != null)
        {
            placementLayer.SetActive(true);
        }

        // Hide all placement groups first so only the correct one appears
        HideAllPlacementGroups();

        GameObject currentPlacementGroup = null;

        if (placementGroups != null &&
            currentPhase >= 0 &&
            currentPhase < placementGroups.Length &&
            placementGroups[currentPhase] != null)
        {
            currentPlacementGroup = placementGroups[currentPhase];
        }

        if (currentPlacementGroup == null)
        {
            Debug.LogWarning("No placement group assigned for phase " + currentPhase);
            yield break;
        }

        Vector3 finalPosition = currentPlacementGroup.transform.position;

        // Start the placement group below its final position to slide into view
        Vector3 startPosition = finalPosition + new Vector3(0f, -placementSlideUpDistance, 0f);

        // Move the placement group to the start position
        currentPlacementGroup.transform.position = startPosition;

        // Turn on the placement group
        currentPlacementGroup.SetActive(true);

        // Slide the placement group upward into its final position
        yield return StartCoroutine(SlideObject(
            currentPlacementGroup,
            startPosition,
            finalPosition,
            placementSlideUpDuration
        ));

        Debug.Log("Showing placement group for phase " + currentPhase);

        if (IsMotherboardOnlyPlacementPhase())
        {
            Debug.Log("Motherboard-only placement phase. Moving on after " + motherboardOnlyDuration + " seconds.");

            yield return new WaitForSeconds(motherboardOnlyDuration);

            OnHardwarePlaced();
        }
        else
        {
            Debug.Log("Waiting for user to correctly place the hardware.");

            // The PlacementDropTarget script will call OnHardwarePlaced() 
            // after the hardware item is placed in the correct spot
        }
    }


    // Slides any GameObject from one position to another
    //
    // obj = the object to move
    // startPosition = where the object begins
    // endPosition = where the object ends
    // duration = how long the slide takes
    private IEnumerator SlideObject(GameObject obj, Vector3 startPosition, Vector3 endPosition, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {

            elapsedTime += Time.deltaTime;

            float t = elapsedTime / duration;

            t = Mathf.SmoothStep(0f, 1f, t);

            if (obj != null)
            {
                // Lerp = "linear interpolation"
                obj.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            }

            yield return null;
        }

        // Force the object exactly to the final position
        if (obj != null)
        {
            obj.transform.position = endPosition;
        }
    }


    // Hides the draggable answer choices for the current city analogy
    // Mainly used before the broken city repair effect starts
    private void HideCurrentPhaseAnswerChoices()
    {
        if (cityAnalogies == null ||
            currentPhase < 0 ||
            currentPhase >= cityAnalogies.Length ||
            cityAnalogies[currentPhase] == null)
        {
            return;
        }

        // Find every ZDraggableItem inside the current analogy
        ZDraggableItem[] answerChoices =
            cityAnalogies[currentPhase].GetComponentsInChildren<ZDraggableItem>(true);

        foreach (ZDraggableItem choice in answerChoices)
        {
            if (choice != null)
            {
                choice.gameObject.SetActive(false);
            }
        }

        Debug.Log("Hid " + answerChoices.Length + " orbiting answer choices.");
    }


    // Called when the hardware placement is finished
    public void OnHardwarePlaced()
    {
        Debug.Log("Hardware placed correctly or timer finished for phase " + currentPhase);

        HideAllPlacementGroups();

        if (placementLayer != null)
        {
            placementLayer.SetActive(false);
        }

        currentPhase++;

        // Start next city analogy phase
        if (cityAnalogies != null && currentPhase < cityAnalogies.Length)
        {
            StartAnalogyPhase(currentPhase);
        }
        else
        {
            // The game is finished
            Debug.Log("All phases completed.");
        }
    }
}