using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public bool m_DebugAim;
    public StateManager m_StateManager;
    public CameraHolder m_CameraHolder;
    private float m_Vertical;
    private float m_Horizontal;
    private float m_Delta;

    private bool m_IsAim;

    private void Awake()
    {
        m_StateManager.Init();
        m_CameraHolder.Init(this);
    }

    private void Update()
    {
        m_Delta = Time.deltaTime;

        GetInput();
        UpdateStatus();
        m_StateManager.FixedTick(m_Delta);
        m_CameraHolder.Tick(m_Delta);
    }

    private void FixedUpdate()
    {
        UpdateAimPosition();
    }

    private void UpdateStatus()
    {
        m_StateManager.m_Input.m_Horizontal = m_Horizontal;
        m_StateManager.m_Input.m_Vertical = m_Vertical;
        m_StateManager.m_Input.m_MovementAmount = Mathf.Clamp01(Mathf.Abs(m_Horizontal) + Mathf.Abs(m_Vertical));
        Vector3 tmp_MoveDirection = m_CameraHolder.m_Transform.forward * m_Vertical;
        tmp_MoveDirection += m_CameraHolder.m_Transform.right * m_Horizontal;
        tmp_MoveDirection.Normalize();
        m_StateManager.m_Input.m_MoveDirection = tmp_MoveDirection;
        m_StateManager.m_Input.m_RotateDirection = m_CameraHolder.m_CamTrans.forward;
    }

    private void GetInput()
    {
        m_Horizontal = Input.GetAxis("Horizontal");
        m_Vertical = Input.GetAxis("Vertical");
        m_IsAim = Input.GetMouseButton(1);
        if (m_DebugAim) m_IsAim = true;
        m_StateManager.m_Status.m_IsAiming = m_IsAim;
    }

    private void UpdateAimPosition()
    {
        Ray tmp_Ray = new Ray(m_CameraHolder.m_CamTrans.position, m_CameraHolder.m_CamTrans.forward);
        m_StateManager.m_Input.m_AimPosition = tmp_Ray.GetPoint(30);

        RaycastHit tmp_Hit;
        if(Physics.Raycast(tmp_Ray,out tmp_Hit,100,m_StateManager.m_LayerMaskForAimCast))
        {
            m_StateManager.m_Input.m_AimPosition = tmp_Hit.point;
        }
    }
}
