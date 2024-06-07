using UnityEngine;

namespace ProjectMOMENTUM
{
    public class RendererDisabler : MonoBehaviour
    {
        [SerializeField] protected bool disableRendererOnStart = true;
        private MeshRenderer _renderer;

        private void Awake()
        {
            if (!TryGetComponent(out _renderer))
            {
                Debug.LogError($"No renderer was found on gameobject: {gameObject.name}.");
                return;
            }

            if (disableRendererOnStart)
                _renderer.enabled = false;
        }
    }
}
