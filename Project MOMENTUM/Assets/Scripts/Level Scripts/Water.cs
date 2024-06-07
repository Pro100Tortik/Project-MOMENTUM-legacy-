using UnityEngine;
using UnityEngine.Rendering;

namespace ProjectMOMENTUM
{
    [RequireComponent(typeof(Volume))]
    public sealed class Water : TriggerZoneAbstract
    {
        private void Awake()
        {
            GetComponent<Volume>().isGlobal = false;
        }

        protected override void OnEnter(Collider other) { }

        protected override void OnExit(Collider other) { }

        protected override void OnStay(Collider other) { }
    }
}