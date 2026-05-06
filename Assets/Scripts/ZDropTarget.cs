using UnityEngine;
using UnityEngine.Events;

public class ZDropTarget : MonoBehaviour
{
    public string expectedItemID;
    
    public UnityEvent onCorrectDrop; 
    public UnityEvent onIncorrectDrop;

    public void TriggerSuccess()
    {
        onCorrectDrop.Invoke();
    }

    public void TriggerFailure()
    {
        onIncorrectDrop.Invoke();
    }
}