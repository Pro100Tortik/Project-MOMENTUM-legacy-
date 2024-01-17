using UnityEngine;

public abstract class WeaponAbstract : MonoBehaviour
{
    public abstract WeaponDataSO WeaponData();

    public abstract bool WasDualWielded();
}
