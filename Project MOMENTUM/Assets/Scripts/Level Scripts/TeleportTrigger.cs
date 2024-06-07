using UnityEngine;

namespace ProjectMOMENTUM
{
    public class TeleportTrigger : TriggerZoneAbstract
    {
        [SerializeField] private Transform teleportPosition;
        [SerializeField] private bool resetVelocity = true;
        [SerializeField] private bool overridePitch = true;
        [SerializeField] private float yawRotation = 0f;
        private CameraController _camera;

        private void Start() => _camera = CameraController.Instance;

        private void OnDrawGizmos()
        {
            if (teleportPosition == null)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(teleportPosition.position, new Vector3(0.7f, 1.8f, 0.7f));
            Gizmos.color = Color.red;
            yawRotation = yawRotation.Wrap(-180, 180);
            teleportPosition.rotation = Quaternion.Euler(new Vector3(0, yawRotation, 0));
            Gizmos.DrawRay(teleportPosition.position, teleportPosition.forward);
        }

        protected override void OnEnter(Collider other) 
        {
            if (teleportPosition == null)
                return;

            other.transform.position = teleportPosition.position;

            if (resetVelocity && other.TryGetComponent(out Rigidbody rb))
                rb.velocity = Vector3.zero;

            if (overridePitch) 
                _camera.RotateCamera(yawRotation, 0);
            else
                _camera.RotateCamera(yawRotation);
        }

        protected override void OnExit(Collider other) { }

        protected override void OnStay(Collider other) { }
    }
}
