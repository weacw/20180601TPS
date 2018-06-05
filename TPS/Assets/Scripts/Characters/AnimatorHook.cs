using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHook : MonoBehaviour
{
    public Transform m_LeftHandTarget;
    private Animator m_Aniamtor;
    private StateManager m_StateManager;

    private Transform m_Shoulder;
    private Transform m_AimPivot;
    private Transform m_RightHandTarget;

    private Vector3 m_LookDirection;

    private float m_BodyWeight = .3f;
    private float m_RightHandWeight;
    private float m_LookatWeight;
    private float m_leftHandWeight;
    private RuntimeWeapon m_CurWeapon;
    private bool m_OnIDleDisableLeftHand;

    public void Init(StateManager _stateManager)
    {
        m_StateManager = _stateManager;
        m_Aniamtor = m_StateManager.m_Aniamtor;

        m_Shoulder = m_Aniamtor.GetBoneTransform(HumanBodyBones.RightShoulder);
        m_AimPivot = new GameObject("Aim pivot").transform;
        m_AimPivot.transform.SetParent(m_StateManager.m_Transform);

        m_RightHandTarget = new GameObject("Right hand target").transform;
        m_RightHandTarget.SetParent(m_AimPivot);
        m_StateManager.m_Input.m_AimPosition = m_StateManager.m_Transform.position + transform.forward * 15;
        m_StateManager.m_Input.m_AimPosition.y += 1.5f;
    }

    private void OnAnimatorMove()
    {
        m_LookDirection = m_StateManager.m_Input.m_AimPosition - m_AimPivot.position;
        HandlerShoulder();
    }
    private void OnAnimatorIK(int layerIndex)
    {
        HandlerWeights();
        m_Aniamtor.SetLookAtWeight(m_LookatWeight, m_BodyWeight, 1, 1, 1);
        m_Aniamtor.SetLookAtPosition(m_StateManager.m_Input.m_AimPosition);

        if (m_LeftHandTarget != null)
            UpdateIK(AvatarIKGoal.LeftHand, m_LeftHandTarget, m_leftHandWeight);
        UpdateIK(AvatarIKGoal.RightHand, m_RightHandTarget, m_RightHandWeight);
    }

    private void HandlerShoulder()
    {
        HandlerShoulderPosition();
        HandlerShoulderRotation();
    }

    private void HandlerShoulderRotation()
    {
        Vector3 tmp_TargetDir = m_LookDirection;
        if (tmp_TargetDir == Vector3.zero)
            tmp_TargetDir = Vector3.forward;

        Quaternion tmp_Target = Quaternion.LookRotation(tmp_TargetDir);
        m_AimPivot.rotation = Quaternion.Slerp(m_AimPivot.rotation, tmp_Target, m_StateManager.m_Delta * 15);
    }

    private void HandlerShoulderPosition()
    {
        m_AimPivot.position = m_Shoulder.position;
    }

    private void HandlerWeights()
    {
        float tmp_LookatWeight = 0;
        float tmp_RightHandWeight = 0;

        if (m_StateManager.m_Status.m_IsAiming)
        {
            tmp_RightHandWeight = 1;
            m_BodyWeight = .4f;
        }
        else
            m_BodyWeight = .4f;

        //m_leftHandWeight = m_LeftHandTarget == null ? 0 : 1;
        if (m_LeftHandTarget != null)
            m_leftHandWeight = 1;
        else
            m_leftHandWeight = 0;

        Vector3 tmp_LookatDirection = m_StateManager.m_Input.m_AimPosition - m_StateManager.m_Transform.position;
        float tmp_Angle = Vector3.Angle(m_StateManager.m_Transform.forward, tmp_LookatDirection);

        tmp_LookatWeight = tmp_Angle > 76 ? 0 : 1;
        if (tmp_Angle > 45)
            tmp_LookatWeight = 0;

        if(!m_StateManager.m_Status.m_IsAiming)
        {
            if (m_OnIDleDisableLeftHand)
                m_leftHandWeight = 0;
        }

        m_LookatWeight = Mathf.Lerp(m_LookatWeight, tmp_LookatWeight, m_StateManager.m_Delta * 3);
        m_RightHandWeight = Mathf.Lerp(m_RightHandWeight, tmp_RightHandWeight, m_StateManager.m_Delta * 10);
    }

    private void UpdateIK(AvatarIKGoal _goal, Transform _trans, float _weight)
    {
        m_Aniamtor.SetIKPositionWeight(_goal, _weight);
        m_Aniamtor.SetIKRotationWeight(_goal, _weight);
        m_Aniamtor.SetIKPosition(_goal, _trans.position);
        m_Aniamtor.SetIKRotation(_goal, _trans.rotation);
    }

    internal void EquipWeapon(RuntimeWeapon _runtimeWeapon)
    {
        Weapon tmp_Weapon = _runtimeWeapon.m_WeaponActual;
        m_LeftHandTarget = _runtimeWeapon.m_WeaponHook.m_LeftHandIK;

        m_RightHandTarget.localPosition = _runtimeWeapon.m_WeaponActual.m_HandIK.m_Position;
        m_RightHandTarget.localEulerAngles = _runtimeWeapon.m_WeaponActual.m_HandIK.m_Rotation;

        m_OnIDleDisableLeftHand = _runtimeWeapon.m_WeaponActual.m_OnIdleDisableLeftHand;
        m_CurWeapon = _runtimeWeapon;
    }
}
