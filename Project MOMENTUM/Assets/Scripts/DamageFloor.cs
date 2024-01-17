using UnityEngine;

public class DamageFloor : MonoBehaviour
{
    [SerializeField] private GameObject attacker;
    [SerializeField] private int damage = 5;
    [SerializeField] private float damageSpeed = 1;
    private float _timer;

    private void Update()
    {
        if (_timer < 0)
            _timer = 0;
        else
            _timer -= Time.deltaTime;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.GetComponent<IDamagable>() != null)
        {
            if (_timer <= 0)
            {
                collision.transform.GetComponent<IDamagable>().Damage(attacker, damage);
                _timer = damageSpeed;
            }
        }
    }
}
