using System.Collections;
using UnityEngine;

public class SuperCityManager : MonoBehaviour
{
    [Header("Game State")]
    public int currentPhase = 0;

    [Header("Intro Layer")]
    public GameObject introLayer;
    public float introDuration = 15f;

    [Header("City Layer")]
    public GameObject cityLayer;
    public GameObject[] cityAnalogies;

    [Header("Placement Layer")]
    public GameObject motherboardBase;
    public GameObject[] hardwareModels;

    [Header("Temporary Placement Timer")]
    public float placementDuration = 3f;

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

        StartCoroutine(PlayIntroThenStartSceneOne());
    }

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

        if (motherboardBase != null)
        {
            motherboardBase.SetActive(false);
        }

        if (cityAnalogies != null)
        {
            foreach (var analogy in cityAnalogies)
            {
                if (analogy != null)
                {
                    analogy.SetActive(false);
                }
            }
        }

        if (hardwareModels != null)
        {
            foreach (var model in hardwareModels)
            {
                if (model != null)
                {
                    model.SetActive(false);
                }
            }
        }
    }

    private IEnumerator PlayIntroThenStartSceneOne()
    {
        Debug.Log("Intro started.");

        if (introLayer != null)
        {
            introLayer.SetActive(true);
        }

        yield return new WaitForSeconds(introDuration);

        if (introLayer != null)
        {
            introLayer.SetActive(false);
        }

        if (cityLayer != null)
        {
            cityLayer.SetActive(true);
        }

        Debug.Log("Intro finished. Starting first analogy.");

        StartAnalogyPhase(0);
    }

    public void StartAnalogyPhase(int phaseIndex)
    {
        if (cityAnalogies == null || phaseIndex < 0 || phaseIndex >= cityAnalogies.Length)
        {
            Debug.LogWarning("Invalid city analogy phase: " + phaseIndex);
            return;
        }

        currentPhase = phaseIndex;
        phaseTransitionRunning = false;

        if (cityLayer != null)
        {
            cityLayer.SetActive(true);
        }

        if (motherboardBase != null)
        {
            motherboardBase.SetActive(false);
        }

        HideAllCityAnalogies();
        HideAllHardwareModels();

        if (cityAnalogies[currentPhase] != null)
        {
            cityAnalogies[currentPhase].SetActive(true);
        }

        Debug.Log("Starting analogy phase " + currentPhase);
    }

    private void HideAllCityAnalogies()
    {
        if (cityAnalogies == null)
        {
            return;
        }

        foreach (var analogy in cityAnalogies)
        {
            if (analogy != null)
            {
                analogy.SetActive(false);
            }
        }
    }

    private void HideAllHardwareModels()
    {
        if (hardwareModels == null)
        {
            return;
        }

        foreach (var model in hardwareModels)
        {
            if (model != null)
            {
                model.SetActive(false);
            }
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
            currentPhase >= 0 &&
            currentPhase < explanationClips.Length &&
            explanationClips[currentPhase] != null)
        {
            explanationAudioSource.clip = explanationClips[currentPhase];
            explanationAudioSource.Play();

            yield return new WaitForSeconds(explanationClips[currentPhase].length);
        }

        yield return new WaitForSeconds(pauseAfterAudio);

        if (cityAnalogies != null &&
            currentPhase >= 0 &&
            currentPhase < cityAnalogies.Length &&
            cityAnalogies[currentPhase] != null)
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

        if (hardwareModels != null &&
            currentPhase >= 0 &&
            currentPhase < hardwareModels.Length &&
            hardwareModels[currentPhase] != null)
        {
            hardwareModels[currentPhase].SetActive(true);
        }

        Debug.Log("Temporary placement layer running for " + placementDuration + " seconds.");

        yield return new WaitForSeconds(placementDuration);

        OnHardwarePlaced();
    }

    private IEnumerator SlideMotherboard(Vector3 startPosition, Vector3 endPosition)
    {
        float elapsedTime = 0f;

        while (elapsedTime < motherboardSlideDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / motherboardSlideDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            if (motherboardBase != null)
            {
                motherboardBase.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            }

            yield return null;
        }

        if (motherboardBase != null)
        {
            motherboardBase.transform.position = endPosition;
        }
    }

    public void OnHardwarePlaced()
    {
        Debug.Log("Hardware placed correctly or timer finished for phase " + currentPhase);

        if (hardwareModels != null &&
            currentPhase >= 0 &&
            currentPhase < hardwareModels.Length &&
            hardwareModels[currentPhase] != null)
        {
            hardwareModels[currentPhase].SetActive(false);
        }

        if (motherboardBase != null)
        {
            motherboardBase.SetActive(false);
        }

        currentPhase++;

        if (cityAnalogies != null && currentPhase < cityAnalogies.Length)
        {
            StartAnalogyPhase(currentPhase);
        }
        else
        {
            Debug.Log("All phases completed.");
        }
    }
}