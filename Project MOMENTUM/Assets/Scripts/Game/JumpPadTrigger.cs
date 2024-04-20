using UnityEngine;

public class JumpPadTrigger : MonoBehaviour
{
    [SerializeField] private float power = 30.0f;
    [SerializeField] private Rigidbody lastRB = null;

    private void OnTriggerEnter(Collider other)
    {
        lastRB = other.GetComponent<Rigidbody>();
        if (lastRB != null && !lastRB.isKinematic )
        {
            lastRB.velocity = new Vector3(lastRB.velocity.x, 0, lastRB.velocity.z);
            lastRB.AddForce(Vector3.up * power, ForceMode.VelocityChange);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (lastRB != null)
            lastRB = null;
    }
}
