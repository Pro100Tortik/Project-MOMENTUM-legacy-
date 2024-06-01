using System.Collections;
using UnityEngine;

namespace ProjectMOMENTUM
{
    public class HitEffectsLibrary : MonoBehaviour
    {
        public ParticleDecalPool ParticleDecal => particleDecal;

        [SerializeField] private ParticleDecalPool particleDecal;
        [SerializeField] private GameObject concrete;
        [SerializeField] private GameObject wood;
        [SerializeField] private GameObject glass;
        [SerializeField] private GameObject iron;
        [SerializeField] private GameObject blood;
        [SerializeField] private GameObject bulletHoleDecal;
        [SerializeField] private GameObject explosion;

        private GameObjectPool _bulletHolesPool;
        private GameObjectPool _concreteParticlesPool;
        private GameObjectPool _woodParticlesPool;
        private GameObjectPool _glassParticlesPool;
        private GameObjectPool _ironParticlesPool;
        private GameObjectPool _bloodParticlesPool;
        private GameObjectPool _explosionsPool;

        public static HitEffectsLibrary Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }

            _bulletHolesPool = new GameObjectPool(bulletHoleDecal, 500);
            _concreteParticlesPool = new GameObjectPool(concrete, 100);
            _woodParticlesPool = new GameObjectPool(wood, 100);
            _glassParticlesPool = new GameObjectPool(glass, 100);
            _ironParticlesPool = new GameObjectPool(iron, 100);
            _bloodParticlesPool = new GameObjectPool(blood, 100);
            _explosionsPool = new GameObjectPool(explosion, 50);
        }

        public void SpawnExplosion(Vector3 position, float duration)
        {
            GameObject effect = _explosionsPool.Get();
            effect.transform.position = position;
            effect.transform.rotation = Quaternion.identity;
            StartCoroutine(DisableEffect(_explosionsPool, effect, duration));
        }

        public void SpawnParticles(RaycastHit hit, float duration)
        {
            if (hit.collider.tag != "Player" && hit.collider.tag != "Enemy")
            {
                TranslateParticles(_bulletHolesPool, hit, Vector3.back, duration);
            }

            GetSurfaceParticles(hit);
        }

        private void TranslateParticles(GameObjectPool pool, RaycastHit hit, Vector3 dir, float duration)
        {
            GameObject effect = pool.Get();
            effect.transform.position = hit.point + hit.normal * 0.01f;
            effect.transform.rotation = Quaternion.FromToRotation(dir, hit.normal);
            StartCoroutine(DisableEffect(pool, effect, duration));
        }

        private IEnumerator DisableEffect(GameObjectPool pool, GameObject effect, float duration)
        {
            yield return new WaitForSeconds(duration);
            pool.Return(effect);
        }

        private void GetSurfaceParticles(RaycastHit hit)
        {
            switch (hit.collider.tag)
            {
                case "Glass":
                    TranslateParticles(_glassParticlesPool, hit, Vector3.forward, 2);
                    break;

                case "Wood":
                    TranslateParticles(_woodParticlesPool, hit, Vector3.forward, 2);
                    break;

                case "Concrete":
                    TranslateParticles(_concreteParticlesPool, hit, Vector3.forward, 2);
                    break;

                case "Ladder":
                case "Iron":
                    TranslateParticles(_ironParticlesPool, hit, Vector3.forward, 2);
                    break;

                case "Player":
                case "Enemy":
                    TranslateParticles(_bloodParticlesPool, hit, Vector3.forward, 4);
                    break;

                default:
                    return;
            }
        }
    }
}