using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AudioWaveRing : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Ring")]
    public int pointCount = 360;
    public float baseRadius = 1.5f;

    [Header("Audio Reaction")]
    public float audioStrength = 25f;
    public float maxWaveSize = 0.25f;
    public float attackSpeed = 40f;
    public float releaseSpeed = 12f;

    [Header("Motion")]
    public float rotateSpeed = 12f;
    public float idleWaveAmount = 0.02f;
    public float idleWaveFrequency = 5f;
    public float idleWaveSpeed = 2f;

    private LineRenderer lineRenderer;
    private float[] audioSamples = new float[512];
    private float[] currentOffsets;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = pointCount;

        currentOffsets = new float[pointCount];
    }

    void Update()
    {
        if (audioSource == null)
        {
            return;
        }

        audioSource.GetOutputData(audioSamples, 0);

        for (int i = 0; i < pointCount; i++)
        {
            float percent = (float)i / pointCount;
            float angle = percent * Mathf.PI * 2f;

            int audioIndex = Mathf.FloorToInt(percent * audioSamples.Length);
            float audioValue = Mathf.Abs(audioSamples[audioIndex]);

            float audioOffset = audioValue * audioStrength;
            audioOffset = Mathf.Clamp(audioOffset, 0f, maxWaveSize);

            float idleOffset = Mathf.Sin(
                angle * idleWaveFrequency + Time.time * idleWaveSpeed
            ) * idleWaveAmount;

            float targetOffset = audioOffset + idleOffset;

            float speed;

            if (targetOffset > currentOffsets[i])
            {
                speed = attackSpeed;
            }
            else
            {
                speed = releaseSpeed;
            }

            currentOffsets[i] = Mathf.Lerp(
                currentOffsets[i],
                targetOffset,
                Time.deltaTime * speed
            );

            float radius = baseRadius + currentOffsets[i];

            float rotatedAngle = angle + Time.time * rotateSpeed * Mathf.Deg2Rad;

            float x = Mathf.Cos(rotatedAngle) * radius;
            float y = Mathf.Sin(rotatedAngle) * radius;

            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}