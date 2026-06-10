using UnityEngine;

public class AudioRingPulse : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Pulse Settings")]
    public float minScale = 1f;
    public float maxScale = 1.25f;
    public float sensitivity = 100f;
    public float smoothSpeed = 8f;

    private Vector3 originalScale;
    private float[] samples = new float[128];
    private float smoothedVolume = 0f;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (audioSource == null)
        {
            return;
        }

        float volume = GetAudioVolume();

        smoothedVolume = Mathf.Lerp(
            smoothedVolume,
            volume,
            Time.deltaTime * smoothSpeed
        );

        float targetScaleMultiplier = 1f + smoothedVolume * sensitivity;
        targetScaleMultiplier = Mathf.Clamp(targetScaleMultiplier, minScale, maxScale);

        Vector3 targetScale = originalScale * targetScaleMultiplier;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * smoothSpeed
        );
    }

    private float GetAudioVolume()
    {
        audioSource.GetOutputData(samples, 0);

        float total = 0f;

        for (int i = 0; i < samples.Length; i++)
        {
            total += Mathf.Abs(samples[i]);
        }

        return total / samples.Length;
    }
}