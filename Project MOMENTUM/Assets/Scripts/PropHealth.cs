using UnityEngine;
using UnityEngine.Events;

public class PropHealth : MonoBehaviour, IDamagable
{
    public UnityEvent OnDestroyed;
    [SerializeField] private int health;

    public void Damage(GameObject attacker, int damage)
    {
        health -= damage; 

        if (health <= 0) 
        {
            OnDestroyed.Invoke();
            return;
        }
    }
}
