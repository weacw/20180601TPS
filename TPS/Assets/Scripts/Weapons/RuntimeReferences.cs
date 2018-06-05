using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Single Instances/RuntimeReferences")]
public class RuntimeReferences : ScriptableObject
{
    public List<RuntimeWeapon> m_RuntimeWeapons = new List<RuntimeWeapon>();

    public void Init()
    {
        m_RuntimeWeapons.Clear();
    }

    public RuntimeWeapon WeaponToRuntimeWeapon(Weapon _weapon)
    {
        RuntimeWeapon tmp_RuntimeWeapon = new RuntimeWeapon();
        tmp_RuntimeWeapon.m_WeaponActual = _weapon;
        tmp_RuntimeWeapon.m_CurAmmo = _weapon.m_MagazineAmmo;
        tmp_RuntimeWeapon.m_CurCarrying = _weapon.maxAmmo;
        m_RuntimeWeapons.Add(tmp_RuntimeWeapon);
        return tmp_RuntimeWeapon;
    }

}
