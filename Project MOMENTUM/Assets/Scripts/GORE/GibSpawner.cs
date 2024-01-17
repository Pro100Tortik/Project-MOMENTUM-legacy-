using UnityEngine;

public class GibSpawner : MonoBehaviour
{
    [SerializeField] private Transform pos;
    [SerializeField] private int gibsAmount = 5;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float power = 5.0f;
    [SerializeField] private float existTime = 20.0f;
    private GibsManager _gibs;
    private bool _isDead;
    private bool _spawnedAll;

    public void GIB() => _isDead = true;

    private void Start() => _gibs = GibsManager.Instance;

    private void Update()
    {
        if (_spawnedAll)
            return;

        if (!_isDead) 
            return;

        _spawnedAll = true;
        for (int i =  0; i < gibsAmount; i++)
        {
            SpawnGib();
        }
    }

    private void SpawnGib()
    {
        _gibs.SpawnGib(pos == null ? transform.position + offset : pos.position, power, existTime);
    }
}
