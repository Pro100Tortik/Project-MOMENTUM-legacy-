using System.Collections;
using UnityEngine;

public class BasicProjectileWeapon : Weapon
{
    [SerializeField] private Transform headPos;
    [SerializeField] private Transform gunpoint;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float launchForce = 30f;
    [SerializeField] private float upwardForce = 0f;

    private PoolBase<GameObject> _projectilesPool;
    public GameObject Preload() => Instantiate(projectile);
    public void GetAction(GameObject effect) => effect.SetActive(true);
    public void ReturnAction(GameObject effect) => effect.SetActive(false);

    private void Awake() => _projectilesPool = new PoolBase<GameObject>(Preload, GetAction, ReturnAction, 20);

    protected override void PrimaryAttack() => SpawnProjectile();

    private void SpawnProjectile()
    {
        GameObject projectile = _projectilesPool.Get();

        var proj = projectile.GetComponent<Projectile>();
        proj.SetAttacker(player.origin);
        proj.OnHit += ReturnRocket;

        Ray ray = new Ray(headPos.position, headPos.forward);
        Vector3 target;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, weaponData.primaryRange, levelMask, QueryTriggerInteraction.Ignore))
        {
            target = hitInfo.point;
        }
        else
        {
            target = ray.GetPoint(weaponData.primaryRange);
        }

        var rb = projectile.GetComponent<Rigidbody>();
        rb.position = gunpoint.position;
        Vector3 dir = target - gunpoint.position;
        rb.rotation = Quaternion.LookRotation(dir.normalized, projectile.transform.up);

        rb.AddForce(dir.normalized * launchForce, ForceMode.Impulse);
        rb.AddForce(headPos.up * upwardForce, ForceMode.Impulse);

        proj.CalculateVelocity(dir.normalized * launchForce + headPos.up * upwardForce);

        StartCoroutine(ReturnIfMiss(projectile, 30));
    }

    private void ReturnRocket(GameObject proj)
    {
        _projectilesPool.Return(proj);
    }

    private IEnumerator ReturnIfMiss(GameObject proj, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnRocket(proj);
    }
}
