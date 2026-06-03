using System.Collections;
using UnityEngine;

public class SuperCityManager : MonoBehaviour
{
    [Header("Game State")]
    public int currentPhase = 0;

    [Header("City Layer")]
    public GameObject[] cityAnalogies;

    [Header("Placement Layer")]
    public GameObject motherboardBase;
    public GameObject[] hardwareModels;

    [Header("Audio")]
    public AudioSource explanationAudioSource;
    public AudioClip[] explanationClips;

    [Header("Transition Settings")]
    public float pauseBeforeAudio = 0.5f;
    public float pauseAfterAudio = 0.5f;
    public float motherboardSlideDuration = 1.5f;
    public float motherboardStartYOffset = -5f;

    private Vector3 motherboardFinalPosition;
    private bool phaseTransitionRunning = false;

    void Start()
    {
        if (motherboardBase != null)
        {
            motherboardFinalPosition = motherboardBase.transform.position;
        }

        HideAll();
        StartAnalogyPhase(0);
    }

    private void HideAll()
    {
        if (motherboardBase != null)
        {
            motherboardBase.SetActive(false);
        }

        foreach (var analogy in cityAnalogies)
        {
            if (analogy != null)
            {
                analogy.SetActive(false);
            }
        }

        foreach (var model in hardwareModels)
        {
            if (model != null)
            {
                model.SetActive(false);
            }
        }
    }

    public void StartAnalogyPhase(int phaseIndex)
    {
        currentPhase = phaseIndex;
        phaseTransitionRunning = false;

        if (cityAnalogies[currentPhase] != null)
        {
            cityAnalogies[currentPhase].SetActive(true);
        }
    }

    public void OnAnalogySolved()
    {
        if (phaseTransitionRunning)
        {
            return;
        }

        StartCoroutine(AnalogySolvedRoutine());
    }

    private IEnumerator AnalogySolvedRoutine()
    {
        phaseTransitionRunning = true;

        Debug.Log("Correct analogy selected. Starting explanation sequence.");

        yield return new WaitForSeconds(pauseBeforeAudio);

        if (explanationAudioSource != null &&
            explanationClips != null &&
            currentPhase < explanationClips.Length &&
            explanationClips[currentPhase] != null)
        {
            explanationAudioSource.clip = explanationClips[currentPhase];
            explanationAudioSource.Play();

            yield return new WaitForSeconds(explanationClips[currentPhase].length);
        }

        yield return new WaitForSeconds(pauseAfterAudio);

        if (cityAnalogies[currentPhase] != null)
        {
            cityAnalogies[currentPhase].SetActive(false);
        }

        if (motherboardBase != null)
        {
            motherboardBase.SetActive(true);

            Vector3 startPosition = motherboardFinalPosition + new Vector3(0, motherboardStartYOffset, 0);
            motherboardBase.transform.position = startPosition;

            yield return StartCoroutine(SlideMotherboard(startPosition, motherboardFinalPosition));
        }

        if (hardwareModels[currentPhase] != null)
        {
            hardwareModels[currentPhase].SetActive(true);
        }

        phaseTransitionRunning = false;
    }

    private IEnumerator SlideMotherboard(Vector3 startPosition, Vector3 endPosition)
    {
        float elapsedTime = 0f;

        while (elapsedTime < motherboardSlideDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / motherboardSlideDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            motherboardBase.transform.position = Vector3.Lerp(startPosition, endPosition, t);

            yield return null;
        }

        motherboardBase.transform.position = endPosition;
    }

    public void OnHardwarePlaced()
    {
        Debug.Log("Hardware placed correctly for phase " + currentPhase);

        // add next phases here:
        // currentPhase++;
        // StartAnalogyPhase(currentPhase);
    }
}


