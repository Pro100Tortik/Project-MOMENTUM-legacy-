using UnityEngine;
using UnityEngine.Events;

public class PropHealth : MonoBehaviour, IDamagable
{
    public UnityEvent OnDestroyed;
    [SerializeField] private float health;

    public void Damage(GameObject attacker, float damage)
    {
        health -= damage; 

        if (health <= 0) 
        {
            OnDestroyed.Invoke();
            return;
        }
    }
}
