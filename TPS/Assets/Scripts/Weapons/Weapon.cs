using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Weapons/Weapon",order =0)]
public class Weapon : ScriptableObject {
    public string m_Id;
    public IKPosition m_HandIK;
    public GameObject m_ModelPrefab;

    public float m_FireRate = .1f;
    public int m_MagazineAmmo = 30;
    public int maxAmmo = 160;
    public bool m_OnIdleDisableLeftHand;
    public int m_WeaponType;
    public AnimationCurve m_RecoilY;
    public AnimationCurve m_RecoilZ;

}
