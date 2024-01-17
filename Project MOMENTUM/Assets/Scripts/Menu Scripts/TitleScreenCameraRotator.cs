using UnityEngine;

public class TitleScreenCameraRotator : MonoBehaviour
{
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private bool hardLookAt = false;

    private void Awake()
    {
        if (hardLookAt)
            transform.LookAt(rotatePoint);
    }

    private void Update()
    {
        transform.RotateAround(rotatePoint.position, new Vector3(0.0f, 1.0f, 0.0f), speed * Time.deltaTime);
    }
}
