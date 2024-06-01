using UnityEngine;

public class DecalSpawner : MonoBehaviour
{
    [SerializeField] private GameObject decal;
    [SerializeField] private float existTime = 20.0f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
            return;

        if (collision.collider.CompareTag("Enemy"))
            return;

        GameObject decalObj = Instantiate(decal, collision.contacts[0].point + collision.contacts[0].normal * 0.01f, 
            Quaternion.FromToRotation(Vector3.back, collision.contacts[0].normal));

        Destroy(decalObj, existTime);
    }
}
