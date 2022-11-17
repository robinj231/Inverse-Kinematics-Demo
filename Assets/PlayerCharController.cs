using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharController : MonoBehaviour
{
    Rigidbody rb;
    public Transform cam;
    public IKArm[] arms;

    IKArm curLeg;

    public float speed;

    public float turnSmoothTime;
    float turnSmoothVelocity;

    public float legDist;

    public bool spider = true;

    Vector3 dir;

    int curLegs;

    public LayerMask groundLayer;

    void Awake()
    {
        curLegs = 0;
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        Physics.Raycast(transform.position, Vector3.down, out grounded, Mathf.Infinity, groundLayer);
    }
    RaycastHit grounded;
    void Update()
    {
        if(spider)
        {
            dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }

        if ((new Vector2(arms[curLegs].GetEndpoint().x, arms[curLegs].GetEndpoint().z) - new Vector2(arms[curLegs].raycaster.position.x, arms[curLegs].raycaster.position.z)).magnitude > 3.5f)
        {
            RaycastHit hit;
            if (Physics.Raycast(arms[curLegs].raycaster.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                arms[curLegs].audioPlayer.Play();
                arms[curLegs].goal = hit.point;
            }

            if (Physics.Raycast(arms[curLegs + 2].raycaster.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                arms[curLegs + 2].audioPlayer.Play();
                arms[curLegs + 2].goal = hit.point;
            }
            curLegs = 1 - curLegs;
        }

        Physics.Raycast(transform.position, Vector3.down, out grounded, Mathf.Infinity, groundLayer);
    }

    private void FixedUpdate()
    {
        if(spider)
        {
            if (dir.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                rb.rotation = Quaternion.Euler(0f, angle, 0f);
                rb.velocity = (Quaternion.Euler(0f, angle, 0) * Vector3.forward).normalized * speed;
            }
        }
        
        Debug.Log(grounded.distance);
        if (grounded.distance > 1.65f)
            rb.velocity -= new Vector3(0, 9.8f, 0);
    }
}
