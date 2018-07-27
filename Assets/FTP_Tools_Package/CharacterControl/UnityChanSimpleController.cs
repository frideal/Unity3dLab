using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastToolsPackage
{
    [RequireComponent(typeof(CharacterController))]
    public class UnityChanSimpleController : MonoBehaviour
    {
        private CharacterController m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera

        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

        public float m_speed = 1.0f;
        public float m_Gravity = 20f;
        public float m_jumpSpeed = 8.0f;

        public Animator contoller;
        public bool m_EnableCaptureScreenWithKeyP;
        public string m_capturePath = "C:/Users/";

        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(" Camera is null, just simple move the character.");
            }
            m_Character = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = Input.GetButtonDown("Jump");
            }
        }

        private int currentCount = 0;
        private void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (m_EnableCaptureScreenWithKeyP && Input.GetKeyDown(KeyCode.P))
            {
                string path = m_capturePath + string.Format("/{0}_{1}.png", currentCount, System.DateTime.Now.ToFileTimeUtc());
                ScreenCapture.CaptureScreenshot(path);
                Debug.Log("### Capature one shot :" + path);
            }

            // read inputs
            m_Move = Vector3.zero;
            // if (m_Character.isGrounded)
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                bool crouch = Input.GetKey(KeyCode.C);
                crouch = false;
                if (m_Cam != null)
                {
                    m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                    m_Move = v * m_CamForward + h * m_Cam.right;
                }
                else
                {
                    m_Move = v * Vector3.forward + h * Vector3.right;
                }
#if !MOBILE_INPUT
                // walk speed multiplier
                if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 2.0f;
#endif
            }
            if (Input.GetButtonDown("Jump") && m_Character.isGrounded)
            {
                m_Move.y = m_jumpSpeed;
            }

            m_Move.y -= m_Gravity * Time.deltaTime;

            m_Character.Move(m_Move * Time.deltaTime * m_speed);

            m_Jump = false;
            if (m_Move.x != 0f || m_Move.z != 0f)
            {
                contoller.SetBool("Walk", true);
                transform.forward = new Vector3(m_Move.x, 0, m_Move.z);
            }
            else
            {
                contoller.SetBool("Walk", false);
            }
        }
    }
}
