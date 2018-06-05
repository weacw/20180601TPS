using UnityEngine;
[System.Serializable]
public class InputVariables
{
    public float m_Horizontal;
    public float m_Vertical;
    public float m_MovementAmount;
    public Vector3 m_RotateDirection;
    public Vector3 m_MoveDirection;
    public Vector3 m_AimPosition;
}

[System.Serializable]

public class ControllerStatus
{
    public bool m_OnGround;
    public bool m_IsAiming;
}

[System.Serializable]
public class CameraValues
{
    public float m_NormalX;
    public float m_NormalY;
    public float m_NormalZ;
    public float m_AdaptSpeed;
    public float m_CamMoveSpeed;
    public float m_TurnSmooth = .1f;
    public float m_Y_Rotate_Speed;
    public float m_X_Rotate_Speed;
    public float m_MinAngle;
    public float m_MaxAngle;

    public float m_AimX;
    public float m_AimZ;
}

[System.Serializable]
public class RuntimeWeapon
{
    public int m_CurAmmo;
    public int m_CurCarrying;
    public GameObject m_Instance;
    public WeaponHook m_WeaponHook;
    public Weapon m_WeaponActual;
}

[System.Serializable]
public class WeaponManager
{
    public string m_MainWeaponId;
    public string m_SecondWeaponID;

    public RuntimeWeapon m_MainWeapon;
    public RuntimeWeapon m_SecondWeapon;
}