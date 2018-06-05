using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Single Instances/Resources")]
public class ResourcesManager : ScriptableObject
{
    public RuntimeReferences m_RuntimeReferences;
    public Weapon[] m_All_Weapons;
    private Dictionary<string, int> m_WeaponsDict = new Dictionary<string, int>();
    public void Init()
    {
        for (int i = 0; i < m_All_Weapons.Length; i++)
        {
            if (!m_WeaponsDict.ContainsKey(m_All_Weapons[i].m_Id))
                m_WeaponsDict.Add(m_All_Weapons[i].m_Id, i);
        }
    }

    public Weapon GetWeaponById(string _id)
    {
        Weapon retVal = null;
        int index = -1;
        if (m_WeaponsDict.TryGetValue(_id, out index))
            retVal = m_All_Weapons[index];
        return retVal;
    }

}
