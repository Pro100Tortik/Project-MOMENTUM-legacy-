using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public UnityEvent OnEnter;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnEnter.Invoke();
            gameObject.SetActive(false);
        }
    }
}
