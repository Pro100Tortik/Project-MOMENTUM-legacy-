using Unity.VisualScripting;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Rigidbody playerRB;
    [SerializeField] private float maxRange = 30;
    [SerializeField] private LayerMask layer;
    public bool IsGrapling { get; private set; } = false;
    private AttractionPoint _hookPoint = null;
    private GameObject hookPoint;
    private LineRenderer _rope;
    private float _distance;
    private float speed = 0.995f;

    private void Awake()
    {
        hookPoint = new GameObject();
        _hookPoint = hookPoint.AddComponent<AttractionPoint>();
        hookPoint.SetActive(false);
        _rope = GetComponent<LineRenderer>();
        _rope.enabled = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ShootRope();
            IsGrapling = true;
            _rope.enabled = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            speed = 0.995f;
            IsGrapling = false;
            hookPoint.SetActive(false);
            _rope.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        Vector3 vec = playerRB.velocity * Time.fixedDeltaTime;
        if (IsGrapling)
        {
            _hookPoint.Attract(playerRB);
            _distance = Vector3.Distance(transform.position, hookPoint.transform.position) * speed;
            ApplyTensionForce(vec, transform.position + vec);
        }
    }

    private void LateUpdate()
    {
        _rope.SetPosition(0, transform.position + Vector3.down * 0.5f);
    }

    private void ShootRope()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRange, layer))
        {
            hookPoint.SetActive(true);
            hookPoint.transform.position = hit.point;
            _rope.SetPosition(1, hit.point);
        }
        else
        {
            IsGrapling = false;
        }
    }

    private void ApplyTensionForce(Vector3 curr_velo_upf, Vector3 test_pos)
    {
        Vector3 normalized = (test_pos - hookPoint.transform.position).normalized;
        Vector3 a = normalized * _distance + hookPoint.transform.position;
        Vector3 a2 = a - transform.position;
        Vector3 a3 = a2 - curr_velo_upf;
        Vector3 force = playerRB.mass * (a3 / Time.fixedDeltaTime);
        playerRB.AddForce(force, ForceMode.Impulse);
    }
}
