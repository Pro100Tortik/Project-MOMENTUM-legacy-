using UnityEngine;

namespace ProjectMOMENTUM
{
    [RequireComponent(typeof(RendererDisabler))]
    public abstract class TriggerZoneAbstract : MonoBehaviour
    {
        [SerializeField] protected bool isOneUse = false;
        [SerializeField] protected bool detectPlayersOnly = true;
        protected Collider triggerCollider;
        private string _playerTag = "Player";

        private void Awake()
        {
            if (!TryGetComponent(out triggerCollider))
            {
                Debug.LogError($"No collider was found on gameobject: {gameObject.name}.");
                return;
            }
            triggerCollider.isTrigger = true;
        }

        private bool CanActivate(Collider other)
        {
            if (detectPlayersOnly && !other.CompareTag(_playerTag))
                return false;

            if (isOneUse)
                gameObject.SetActive(false);

            return true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!CanActivate(other))
                return;

            OnEnter(other);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!CanActivate(other))
                return;

            OnStay(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!CanActivate(other))
                return;

            OnExit(other);
        }

        protected abstract void OnEnter(Collider other);
        protected abstract void OnStay(Collider other);
        protected abstract void OnExit(Collider other);
    }
}
