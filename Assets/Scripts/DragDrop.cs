using UnityEngine;
using zSpace.Core.Input;

public class DragDrop : MonoBehaviour
{
    public string targetZoneName = "DropZone";
    private bool _isDragging = false;
    private bool _isCorrect = false;
    private Vector3 _startPosition;
    private Transform _pointerTransform;

    void Start()
    {
        _startPosition = transform.position;
        ZPointer pointer = FindObjectOfType<ZPointer>();
        if (pointer != null) _pointerTransform = pointer.transform;
    }

    void Update()
    {
        ZPointer pointer = FindObjectOfType<ZPointer>();
        if (pointer == null) return;

        bool buttonDown = pointer.GetButton(0); 

        if (buttonDown && !_isDragging)
        {
            RaycastHit hit;
            if (Physics.Raycast(pointer.transform.position, pointer.transform.forward, out hit))
            {
                if (hit.transform == this.transform)
                {
                    _isDragging = true;
                    transform.SetParent(pointer.transform);
                    transform.localPosition = new Vector3(0, 0, 5.0f);
                }
            }
        }
        else if (!buttonDown && _isDragging)
        {
            _isDragging = false;
            transform.SetParent(null);

            if (_isCorrect)
            {
                GameObject zone = GameObject.Find(targetZoneName);
                if (zone) transform.position = zone.transform.position;
            }
            else
            {
                transform.position = _startPosition;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == targetZoneName) _isCorrect = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == targetZoneName) _isCorrect = false;
    }
}