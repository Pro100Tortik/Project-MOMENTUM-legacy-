using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactionRange = 3.0f;
    [SerializeField] private float interactionCheckRadius = 0.2f;
    [SerializeField] private LayerMask interactableLayer;
    private Transform _playerCamera;
    private ClientStatus _status;

    private void Awake()
    {
        _playerCamera = transform;
        _status = GetComponentInParent<ClientStatus>();
    }


    private void Update()
    {
        if (_status.CurrentClientState != PlayerState.Gameplay)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {
        RaycastHit hitInfo;
        RaycastHit hit;
        Ray ray = new Ray(_playerCamera.position, _playerCamera.forward);

        Physics.Raycast(ray, out hit, interactionRange);

        if (Physics.SphereCast(ray, interactionCheckRadius, 
            out hitInfo, hit.transform != null ? hit.distance : interactionRange, 
            interactableLayer, QueryTriggerInteraction.Ignore))
        {
            IInteractable interactable;
            if (hitInfo.collider.TryGetComponent<IInteractable>(out interactable))
            {
                interactable.Interact();
            }
        }
    }
}
