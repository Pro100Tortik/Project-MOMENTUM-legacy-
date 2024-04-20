using UnityEngine;

public class AttractionPoint : MonoBehaviour
{
    public float gravity = Physics.gravity.y * 0.25f;

    public void Attract(Rigidbody rb)
    {
        Vector3 normalized = (rb.position - transform.position).normalized;
        rb.AddForce(normalized * gravity);
    }
}
