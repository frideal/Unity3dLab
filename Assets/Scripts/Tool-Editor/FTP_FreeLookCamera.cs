using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace FastToolsPackage
{
    public class FTP_FreeLookCamera : MonoBehaviour
    {
        [SerializeField]
        private float mouseSensitivity = 100.0f;
        [SerializeField]
        private float clampAngle = 80.0f;

        private Vector3 m_CameraCurrentRot;
        private Quaternion m_rot;
        [Header("Press Key to Exit Free Look Camera")]
        public KeyCode m_exitKey = KeyCode.Escape;
        [Header("Press Key to Enter Free Look Camera")]
        public KeyCode m_enterKey = KeyCode.E;

        private bool m_working = false;

        private void LateUpdate()
        {
            if (m_working)
            {
                float fh = Input.GetAxis("Mouse X");
                float fv = Input.GetAxis("Mouse Y");

                m_CameraCurrentRot.y += fh * mouseSensitivity * Time.deltaTime;
                m_CameraCurrentRot.x -= fv * mouseSensitivity * Time.deltaTime;

                m_CameraCurrentRot.x = Mathf.Clamp(m_CameraCurrentRot.x, -clampAngle, clampAngle);
                transform.localEulerAngles= m_CameraCurrentRot;
            }

            if (Input.GetKeyDown(m_exitKey))
            {
                m_working = false;
            }
            else if (Input.GetKeyDown(m_enterKey))
            {
                m_working = true;
                m_CameraCurrentRot = transform.localEulerAngles;
#if UNITY_EDITOR
#if UNITY_2018
                m_CameraCurrentRot = UnityEditor.TransformUtils.GetInspectorRotation(transform);
#else
                // get right euler angle
                Vector3 vect3 = Vector3.zero;
                MethodInfo mth = typeof(Transform).GetMethod("GetLocalEulerAngles", BindingFlags.Instance | BindingFlags.NonPublic);
                PropertyInfo pi = typeof(Transform).GetProperty("rotationOrder", BindingFlags.Instance | BindingFlags.NonPublic);
                object rotationOrder = null;
                if (pi != null)
                {
                    rotationOrder = pi.GetValue(transform, null);
                }
                if (mth != null)
                {
                    object retVector3 = mth.Invoke(transform, new object[] { rotationOrder });
                    vect3 = (Vector3)retVector3;
                    Debug.Log("Get Inspector Euler:" + vect3);
                    m_CameraCurrentRot = vect3;
                    // UnityEditor.TransformUtils.GetInspectorRotation(transform);
                }
#endif
#endif
            }
        }
    }
}
