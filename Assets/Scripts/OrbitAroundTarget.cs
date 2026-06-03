using UnityEngine;

public class OrbitAroundTarget : MonoBehaviour
{
    public Transform centerTarget;

    public float orbitRadius = 1.5f;
    public float orbitSpeed = 90f;
    public float startingAngle = 0f;

    private float angle;

    void Start()
    {
        angle = startingAngle;
        ReturnToCurrentOrbitPosition();
    }

    void Update()
    {
        angle += orbitSpeed * Time.deltaTime;

        if (angle >= 360f)
        {
            angle -= 360f;
        }

        ReturnToCurrentOrbitPosition();
    }

    public void ReturnToCurrentOrbitPosition()
    {
        if (centerTarget == null) return;

        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * orbitRadius;
        float y = Mathf.Sin(angle * Mathf.Deg2Rad) * orbitRadius;

        transform.position = centerTarget.position + new Vector3(x, y, 0);
    }
}


