using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [HideInInspector] public Transform m_Transform;
    [HideInInspector] public Animator m_Aniamtor;
    private Rigidbody m_Rigidbody;
    private Collider m_CharacterCollider;
    private AnimatorHook m_AniHook;

    //Object
    public GameObject m_ActiveModel;
    public ControllerStats m_Stats;
    public ControllerStatus m_Status;
    public InputVariables m_Input;
    public ResourcesManager m_ResourceManager;
    public WeaponManager m_WeaponManager;
    //enum
    public CharacterStatus m_CurStatus;
    public LayerMask m_LayerMaskForGround;
    public LayerMask m_LayerMaskForAimCast;

    public float m_Delta { get; private set; }

    public void Init()
    {
        m_ResourceManager.Init();

        m_Transform = transform;
        m_CharacterCollider = GetComponent<Collider>();

        SetupAnimator();
        SetupRigidybody();
        SetupLayerMask();

        m_AniHook = m_ActiveModel.AddComponent<AnimatorHook>();
        m_AniHook.Init(this);

        InitWeaponManager();
    }

    private void SetupLayerMask()
    {
        m_LayerMaskForGround = ~(1 << 9);
        m_LayerMaskForAimCast = ~(1 << 9 | 1 << 10);
    }

    private void SetupRigidybody()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.drag = 4;
        m_Rigidbody.angularDrag = 999;
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void SetupAnimator()
    {
        if (m_ActiveModel == null)
        {
            m_Aniamtor = GetComponentInChildren<Animator>();
            m_ActiveModel = m_Aniamtor.gameObject;
        }

        if (m_Aniamtor == null) m_Aniamtor = m_Aniamtor = m_ActiveModel.GetComponent<Animator>();
    }

    public void FixedTick(float _delta)
    {
        m_Delta = _delta;
        switch (m_CurStatus)
        {
            case CharacterStatus.Normal:
                RotationNormal();
                MovementNormal();
                HandleAnimationsNormal();
                m_Status.m_OnGround = OnGroundChecker();
                break;
            case CharacterStatus.Onair:
                break;
            case CharacterStatus.Cover:
                break;
            case CharacterStatus.Vaulting:
                break;
        }
    }

    private bool OnGroundChecker()
    {
        Vector3 tmp_OriginalPos = m_Transform.position;
        tmp_OriginalPos.y += .6f;
        Vector3 tmp_Direction = Vector3.down;
        float tmp_Distance = .7f;
        RaycastHit tmp_Hit;
        if (Physics.Raycast(tmp_OriginalPos, tmp_Direction, out tmp_Hit, tmp_Distance, m_LayerMaskForGround))
        {
            m_Transform.position = tmp_Hit.point;
            return true;
        }
        return false;
    }


    private void MovementNormal()
    {
        if (m_Input.m_MovementAmount > .5f)
            m_Rigidbody.drag = 0;
        else
            m_Rigidbody.drag = 4;

        float tmp_Speed = m_Stats.m_WalkSpeed;
        Vector3 tmp_Direction = Vector3.zero;
        tmp_Direction = m_Transform.forward * tmp_Speed * m_Input.m_MovementAmount;
        m_Rigidbody.velocity = tmp_Direction;
    }

    private void RotationNormal()
    {
        if (!m_Status.m_IsAiming)
            m_Input.m_RotateDirection = m_Input.m_MoveDirection;
        Vector3 tmp_Direction = m_Input.m_RotateDirection;
        tmp_Direction.y = 0;

        if (tmp_Direction == Vector3.zero) tmp_Direction = m_Transform.forward;

        Quaternion tmp_LookDirection = Quaternion.LookRotation(tmp_Direction);
        Quaternion tmp_TargetRot = Quaternion.Slerp(m_Transform.localRotation, tmp_LookDirection, m_Stats.m_RotationSpeed * m_Delta);
        m_Transform.rotation = tmp_TargetRot;
    }

    private void HandleAnimationsNormal()
    {
        float tmp_Anim_vertical = m_Input.m_Vertical;
        float tmp_Anim_horizontal = m_Input.m_Horizontal;
        m_Aniamtor.SetFloat("Vertical", tmp_Anim_vertical,.2f,m_Delta);
        m_Aniamtor.SetFloat("Horizontal", tmp_Anim_horizontal,.2f, m_Delta);
        m_Aniamtor.SetBool("Aim", m_Status.m_IsAiming);
    }

    #region Manager functions
    public void InitWeaponManager()
    {
        CreateRuntimeWeapon(m_WeaponManager.m_MainWeaponId, ref m_WeaponManager.m_MainWeapon);
        EquipRuntimeWeapon(m_WeaponManager.m_MainWeapon);
    }

    public void CreateRuntimeWeapon(string _id, ref RuntimeWeapon _runtimeWeapon)
    {
        Weapon tmp_Weapon = m_ResourceManager.GetWeaponById(_id);
        RuntimeWeapon tmp_RuntimeWeapon = m_ResourceManager.m_RuntimeReferences.WeaponToRuntimeWeapon(tmp_Weapon);

        GameObject go = Instantiate<GameObject>(tmp_Weapon.m_ModelPrefab);
        tmp_RuntimeWeapon.m_Instance = go;
        tmp_RuntimeWeapon.m_WeaponActual = tmp_Weapon;
        tmp_RuntimeWeapon.m_WeaponHook = go.GetComponent<WeaponHook>();
        go.SetActive(false);

        Transform tmp_RightHand = m_Aniamtor.GetBoneTransform(HumanBodyBones.RightHand);
        go.transform.SetParent(tmp_RightHand);
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.localScale = Vector3.one;

        _runtimeWeapon = tmp_RuntimeWeapon;
    }

    public void EquipRuntimeWeapon(RuntimeWeapon _runtimeWeapon)
    {
        _runtimeWeapon.m_Instance.SetActive(true);
        m_AniHook.EquipWeapon(_runtimeWeapon);
       // m_Aniamtor.SetFloat("WeaponType", _runtimeWeapon.m_WeaponActual.m_WeaponType);
    }
    #endregion
}

public enum CharacterStatus
{
    Normal, Onair, Cover, Vaulting
}
