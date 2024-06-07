using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float interactionRange = 3.0f;
    [SerializeField] private float interactionCheckRadius = 0.2f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask obstacleMask;
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
        Ray ray = new Ray(_playerCamera.position, _playerCamera.forward);

        if (Physics.SphereCast(ray, interactionCheckRadius, 
            out hitInfo, interactionRange, interactableLayer, QueryTriggerInteraction.Ignore))
        {
            if (Physics.Linecast(_playerCamera.position, hitInfo.point, obstacleMask, QueryTriggerInteraction.Ignore))
                return;

            if (hitInfo.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact();
            }
        }
    }
}
