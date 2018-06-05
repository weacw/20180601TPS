using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    public Transform m_Transform;
    public Transform m_Pivot;
    public Transform m_CamTrans;
    public Transform m_Target;

    public CameraValues m_CamSetting;

    private float m_Delta;
    private float m_MouseX;
    private float m_MouseY;
    private float m_SmoothX;
    private float m_SmoothY;
    private float m_SmoothXVelocity;
    private float m_SmoothYVelocity;
    private float m_LookAngle;
    private float m_TitlAngle;

    private StateManager m_StateManager;

    public void Init(InputHandler _input)
    {
        m_Transform = transform;
        m_StateManager = _input.m_StateManager;
        m_Target = m_StateManager.transform;
    }

    public void Tick(float _Delta)
    {
        m_Delta = _Delta;

        HandlerPosition();
        HandlerRotation();

        Vector3 tmp_TargetPosition =
       Vector3.Lerp(m_Transform.position, m_Target.position, m_Delta * m_CamSetting.m_CamMoveSpeed);
        m_Transform.position = tmp_TargetPosition;
    }

    private void HandlerPosition()
    {
        float tmp_Target_X = m_CamSetting.m_NormalX;
        float tmp_Target_Y = m_CamSetting.m_NormalY;
        float tmp_Target_Z = m_CamSetting.m_NormalZ;

        if (m_StateManager.m_Status.m_IsAiming)
        {
            tmp_Target_X = m_CamSetting.m_AimX;
            tmp_Target_Z = m_CamSetting.m_AimZ;
        }


        Vector3 tmp_PivotPosition = m_Pivot.localPosition;
        tmp_PivotPosition.x = tmp_Target_X;
        tmp_PivotPosition.y = tmp_Target_Y;

        Vector3 tmp_CamPosition = m_CamTrans.localPosition;
        tmp_CamPosition.z = tmp_Target_Z;

        float tmp_Time = m_Delta * m_CamSetting.m_AdaptSpeed;
        m_Pivot.localPosition = Vector3.Lerp(m_Pivot.localPosition, tmp_PivotPosition, tmp_Time);
        m_CamTrans.localPosition = Vector3.Lerp(m_CamTrans.localPosition, tmp_CamPosition, tmp_Time);
    }

    private void HandlerRotation()
    {
        m_MouseX = Input.GetAxis("Mouse X");
        m_MouseY = Input.GetAxis("Mouse Y");

        if (m_CamSetting.m_TurnSmooth > 0)
        {
            //Smooth 
            m_SmoothX = Mathf.SmoothDamp(m_SmoothX, m_MouseX, ref m_SmoothXVelocity, m_CamSetting.m_TurnSmooth);
            m_SmoothY = Mathf.SmoothDamp(m_SmoothY, m_MouseY, ref m_SmoothYVelocity, m_CamSetting.m_TurnSmooth);
        }
        else
        {
            m_SmoothX = m_MouseX;
            m_SmoothY = m_MouseY;
        }
        m_LookAngle += m_SmoothX * m_CamSetting.m_Y_Rotate_Speed;
        Quaternion tmp_TargetRot = Quaternion.Euler(0, m_LookAngle, 0);
        m_Transform.rotation = tmp_TargetRot;

        m_TitlAngle -= m_SmoothY * m_CamSetting.m_X_Rotate_Speed;
        m_TitlAngle = Mathf.Clamp(m_TitlAngle, m_CamSetting.m_MinAngle, m_CamSetting.m_MaxAngle);
        m_Pivot.localRotation = Quaternion.Euler(m_TitlAngle, 0, 0);

    }
}
