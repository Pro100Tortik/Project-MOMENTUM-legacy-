using UnityEngine;

public class DoorScript : MonoBehaviour, IInteractable
{
    [SerializeField] private float degrees = 90.0f;
    [SerializeField] private float openSpeed = 3.0f;
    [SerializeField] private float openHeight = 2.75f;
    [SerializeField] private bool openToPlayer = false;
    [SerializeField] private bool isOpened = false;
    [SerializeField] private bool canBeOpenedOnce = false;
    [SerializeField] private bool isLocked = false;
    [SerializeField] private bool canBeInteractedByShooting = false;
    [SerializeField] private bool slideDoor = false;

    private Quaternion _doorOriginRotation;
    private Vector3 _doorOriginPosition;

    private void Awake()
    {
        _doorOriginRotation = transform.localRotation;
        _doorOriginPosition = transform.localPosition;
    }

    public void Interact()
    {
        if (isLocked)
            return;

        //if (canBeOpenedOnce)
        //{
        //    if (!isOpened)
        //        _audioManager.PlayInteractSound(doorSound);
        //}
        //else
        //    _audioManager.PlayInteractSound(doorSound);
        isOpened = canBeOpenedOnce ? true : !isOpened;
    }

    public void OpenOnlyButtons() => isOpened = canBeOpenedOnce ? true : !isOpened;

    private void Update() => MoveDoor();

    private void MoveDoor()
    {
        if (!isOpened)
        {
            if (!slideDoor)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, _doorOriginRotation, openSpeed * Time.deltaTime);
            }
            else
            {
                transform.localPosition = Vector3.Slerp(transform.localPosition, _doorOriginPosition, openSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (!slideDoor)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, 
                    _doorOriginRotation * Quaternion.Euler(0, openToPlayer ? degrees : -degrees, 0), openSpeed * Time.deltaTime);
            }
            else
            {
                transform.localPosition = Vector3.Slerp(transform.localPosition, _doorOriginPosition + (Vector3.up * openHeight), openSpeed * Time.deltaTime);
            }
        }
    }

    public bool CanInteractByShooting() => canBeInteractedByShooting;
}
