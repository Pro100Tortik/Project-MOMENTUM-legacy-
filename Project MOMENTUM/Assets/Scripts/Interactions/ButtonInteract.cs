using UnityEngine;
using UnityEngine.Events;

public class ButtonInteract : MonoBehaviour, IInteractable
{
    public UnityEvent OnInteract;
    [SerializeField] private bool canInteractByShooting = true;

    public bool CanInteractByShooting()
    {
        return canInteractByShooting;
    }

    public void Interact()
    {
        OnInteract.Invoke();
    }
}
