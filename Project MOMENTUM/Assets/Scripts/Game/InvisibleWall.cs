using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
    [SerializeField] private bool disableRendererOnStart = true;
    private MeshRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();

        if (disableRendererOnStart)
            _renderer.enabled = false;
    }
}
