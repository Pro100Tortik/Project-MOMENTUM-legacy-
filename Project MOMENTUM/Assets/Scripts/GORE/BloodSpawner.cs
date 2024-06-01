using System.Collections.Generic;
using UnityEngine;

namespace ProjectMOMENTUM
{
    public class BloodSpawner : MonoBehaviour
    {
        [SerializeField] private ParticleSystem blood;
        [SerializeField] private ParticleSystem bloodSplash;
        [SerializeField] private ParticleSystem bloodPool;
        private ParticleDecalPool _bloodDecalPool;

        private List<ParticleCollisionEvent> _collisionEvents = new List<ParticleCollisionEvent>();

        private void Start()
        {
            _bloodDecalPool = HitEffectsLibrary.Instance.ParticleDecal;
        }

        private void OnParticleCollision(GameObject other)
        {
            ParticlePhysicsExtensions.GetCollisionEvents(blood, other, _collisionEvents);

            for (int i = 0; i < _collisionEvents.Count; i++)
            {
                _bloodDecalPool.ParticleHit(_collisionEvents[i], Color.red);
                EmitAtLocation(_collisionEvents[i]);
            }
        }

        private void EmitAtLocation(ParticleCollisionEvent particleCollision)
        {
            bloodSplash.transform.position = particleCollision.intersection;
            bloodSplash.transform.rotation = Quaternion.LookRotation(particleCollision.normal, bloodSplash.transform.up);

            bloodSplash.Emit(1);
        }
    }
}
