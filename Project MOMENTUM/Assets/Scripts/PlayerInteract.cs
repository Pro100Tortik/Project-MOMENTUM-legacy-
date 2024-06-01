using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float interactionRange = 3.0f;
    [SerializeField] private float interactionCheckRadius = 0.2f;
    [SerializeField] private LayerMask interactableLayer;
    private Transform _playerCamera;

    private void Awake() => _playerCamera = transform;

    private void Update()
    {
        if (!player.CanReadInputs())
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
