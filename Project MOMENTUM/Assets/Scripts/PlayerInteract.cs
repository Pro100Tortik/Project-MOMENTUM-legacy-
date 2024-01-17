using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactionRange = 3.0f;
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
            RaycastHit hitInfo;
            Ray ray = new Ray(_playerCamera.position, _playerCamera.forward);
            if (Physics.Raycast(ray, out hitInfo, interactionRange))
            {
                IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }   
        }
    }
}
