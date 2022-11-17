using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public Vector2 camSpeed;
    CinemachineFreeLook freeCam;
    public Camera mainCam;
    public bool clickToMove;
    //public GameObject plane;

    // Start is called before the first frame update
    void Awake()
    {
        freeCam = GetComponent<CinemachineFreeLook>();
        freeCam.m_XAxis.m_MaxSpeed = 0f;
        freeCam.m_YAxis.m_MaxSpeed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //plane.transform.rotation = Quaternion.LookRotation(-mainCam.transform.up, -mainCam.transform.forward);
        if (Input.GetMouseButton(1))
        {
            freeCam.m_XAxis.m_MaxSpeed = camSpeed.x;
            freeCam.m_YAxis.m_MaxSpeed = camSpeed.y;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            freeCam.m_XAxis.m_MaxSpeed = 0f;
            freeCam.m_YAxis.m_MaxSpeed = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
