using System.Collections;
using UnityEngine;

public class GibsManager : MonoBehaviour
{
    public static GibsManager Instance;
    [SerializeField] private GameObject[] gibs;
    [SerializeField] private int amount = 100;
    private GameObjectPool _gibsPool;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        _gibsPool = new GameObjectPool(gibs[Random.Range(0, gibs.Length - 1)], amount);
    }

    public void SpawnGib(Vector3 position, float launchPower, float duration)
    {
        GameObject gib = _gibsPool.Get();

        //gib.transform.position = position;

        Rigidbody rb = gib.GetComponent<Rigidbody>();
        rb.position = position;
        Vector3 dir = new Vector3(Random.Range(-1f, 1f), 
            Random.Range(0.8f, 2), Random.Range(-1f, 1f)) * 0.7f;

        rb.velocity += Random.Range(1, 4) * (dir * launchPower);
        //rb.AddForce(Random.Range(1, 4) * (dir * launchPower * 0.8f), ForceMode.Impulse);

        StartCoroutine(DisableGib(gib, duration));
    }

    private IEnumerator DisableGib(GameObject effect, float duration)
    {
        yield return new WaitForSeconds(duration);
        _gibsPool.Return(effect);
    }
}
