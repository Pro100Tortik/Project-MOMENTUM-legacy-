using UnityEngine;
using UnityEngine.Events;

namespace ProjectMOMENTUM
{
    public sealed class CustomTrigger : TriggerZoneAbstract
    {
        [SerializeField] private UnityEvent OnTriggerEnter;
        [SerializeField] private UnityEvent OnTriggerExit;

        protected override void OnEnter(Collider other) => OnTriggerEnter?.Invoke();

        protected override void OnExit(Collider other) => OnTriggerExit?.Invoke();

        protected override void OnStay(Collider other) { }
    }
}
