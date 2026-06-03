using UnityEngine;
using UnityEngine.Events;

public class ZDropTarget : MonoBehaviour
{
    [Header("Drop Settings")]
    public string expectedItemID;

    [Header("Feedback")]
    public Renderer feedbackRenderer;
    public Color normalColor = Color.white;
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;

    [Header("Events")]
    public UnityEvent onCorrectDrop;
    public UnityEvent onIncorrectDrop;

    private Material feedbackMaterial;

    void Start()
    {
        if (feedbackRenderer == null)
        {
            feedbackRenderer = GetComponentInChildren<Renderer>();
        }

        if (feedbackRenderer != null)
        {
            feedbackMaterial = feedbackRenderer.material;
            normalColor = feedbackMaterial.color;
        }
    }

    public void TriggerSuccess()
    {
        if (feedbackMaterial != null)
        {
            feedbackMaterial.color = correctColor;
        }

        onCorrectDrop.Invoke();
    }

    public void TriggerFailure()
    {
        if (feedbackMaterial != null)
        {
            feedbackMaterial.color = incorrectColor;
        }

        onIncorrectDrop.Invoke();

        Invoke(nameof(ClearFeedback), 0.5f);
    }

    public void ClearFeedback()
    {
        if (feedbackMaterial != null)
        {
            feedbackMaterial.color = normalColor;
        }
    }
}


