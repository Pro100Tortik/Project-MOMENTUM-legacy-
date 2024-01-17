using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    [SerializeField] private Vector3 size = Vector3.one, position;
    [SerializeField] private Vector2 rotation;
    private CameraController controller;

    private void Awake()
    {
        controller = FindObjectOfType<CameraController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = position;
            other.GetComponent<Rigidbody>().velocity = Vector3.zero;
            controller.RotateCamera(rotation.x, rotation.y);
        }
        else
        {
            other.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(position, size);
    }
}
