using UnityEngine;

namespace ProjectMOMENTUM
{
    public class ParticleDecalPool : MonoBehaviour
    {
        [SerializeField] private float minSize = 0.5f;
        [SerializeField] private float maxSize = 2f;
        [SerializeField] private int maxDecals = 300;

        private ParticleSystem _decalParticleSystem;
        private int _particleDataIndex;
        private ParticleDecalData[] _particleData;
        private ParticleSystem.Particle[] _particles;

        private void Awake()
        {
            _decalParticleSystem = GetComponent<ParticleSystem>();
            _particles = new ParticleSystem.Particle[maxDecals];
            _particleData = new ParticleDecalData[maxDecals];

            for (int i = 0; i < maxDecals; i++)
            {
                _particleData[i] = new ParticleDecalData();
            }
        }

        public void ParticleHit(ParticleCollisionEvent particleCollision, Color color)
        {
            SetParticleData(particleCollision, color);
            DisplayParticles();
        }

        private void SetParticleData(ParticleCollisionEvent particleCollision, Color color)
        {
            if (_particleDataIndex >= maxDecals)
                _particleDataIndex = 0;

            var data = _particleData[_particleDataIndex];
            data.position = particleCollision.intersection;

            Vector3 rot = Quaternion.LookRotation(-particleCollision.normal).eulerAngles;
            rot.z = Random.Range(0, 360);
            data.rotation = rot;

            data.size = Random.Range(minSize, maxSize);

            data.color = color;

            _particleDataIndex++;
        }

        private void DisplayParticles()
        {
            for (int i = 0; i < _particleData.Length; i++)
            {
                _particles[i].position = _particleData[i].position;
                _particles[i].rotation3D = _particleData[i].rotation;
                _particles[i].startSize = _particleData[i].size;
                _particles[i].startColor = _particleData[i].color;
            }

            _decalParticleSystem.SetParticles(_particles, _particles.Length);
        }
    }
}
