using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKArm : MonoBehaviour
{
    public Camera mainCamera;

    public GameObject[] limbs;
    public float[] limbAccel;

    public Vector3 endpoint;

    public Vector2[] jointLimitsZ;
    public Vector2[] jointLimitsX;

    public bool[] hip;

    public bool instant = false;

    public Vector3 goal;

    public float goalTheta;
    public float armTheta;

    public Vector2 hipLimits;

    public float length;

    float elapsedTime = 0f;
    public float duration = 2f;

    public bool mouse = false;

    public Transform raycaster;

    public AudioSource audioPlayer;

    public Transform goalDraw;

    public bool spider = false;

    public int jointLimits = 0;

    // Start is called before the first frame update
    void Start()
    {
        raycaster.position = new Vector3(GetEndpoint().x, raycaster.position.y, GetEndpoint().z + 3f);
        goal = GetEndpoint();
    }

    // Update is called once per frame
    void Update()
    {
        if(mouse)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


                //Initialise the enter variable
                float enter = 0.0f;

                if (new Plane(-mainCamera.transform.forward, transform.position).Raycast(ray, out enter))
                {
                    //Get the point that is clicked
                    Vector3 hitPoint = ray.GetPoint(enter);

                    //Move goal to the point where you clicked along plane
                    goal = hitPoint;
                }
            }
        }

        if (!spider)
        {
            if(Input.GetKeyDown(KeyCode.A))
            {
                jointLimits = (jointLimits + 1)%3;
                if(jointLimits == 0)
                {
                    for (int i = 0; i < jointLimitsZ.Length; i++)
                    {
                        jointLimitsZ[i] = Vector2.zero;
                    }
                }
                else if(jointLimits == 1)
                {
                    for (int i = 0; i < jointLimitsZ.Length; i++)
                    {
                        jointLimitsZ[i] = new Vector2(360, 45);
                    }
                }
                else if (jointLimits == 2)
                {
                    for (int i = 0; i < jointLimitsZ.Length; i++)
                    {
                        jointLimitsZ[i] = new Vector2(360, 90);
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        Solve(); 
        if(!spider)
        {
            goalDraw.position = goal;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(goal, 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(GetEndpoint(), 0.5f);
        Gizmos.DrawRay(raycaster.position, Vector3.down * 1000);
    }

    

    void Solve()
    {
        Vector3 startToGoal, startToEndEffector;
        float dotProd, angleDiff;
        float newAngle = 0f;

        float maxIts = 5;

        // simple hip joint at base

        float oldYRotation = limbs[0].transform.rotation.eulerAngles.y;
        Vector3 localGoal = goal - limbs[0].transform.position;
        goalTheta = Vector3.SignedAngle(new Vector3(localGoal.x, 0, localGoal.z), Vector3.right, Vector3.up);
        Vector3 linedUpGoal = Quaternion.Euler(0, goalTheta, 0) * localGoal;
        linedUpGoal += limbs[0].transform.position;
        limbs[0].transform.rotation = Quaternion.Euler(limbs[0].transform.rotation.eulerAngles.x, 0, limbs[0].transform.rotation.eulerAngles.z);

        for (int l = limbs.Length - 1; l >= 0; l--)
        {
            //Update joint
            startToGoal = (Vector2)linedUpGoal - (Vector2)limbs[l].transform.position;
            startToEndEffector = (Vector2)GetEndpoint() - (Vector2)limbs[l].transform.position;
            dotProd = Vector2.Dot(startToGoal.normalized, startToEndEffector.normalized);
            dotProd = Mathf.Clamp(dotProd, -1, 1);

            float newXAngle = limbs[l].transform.localRotation.eulerAngles.x;
            float newYAngle = limbs[l].transform.localRotation.eulerAngles.y;
            float newZAngle = limbs[l].transform.localRotation.eulerAngles.z;

            angleDiff = Mathf.Acos(dotProd);
            // cap acceleration of joint
            angleDiff *= limbAccel[l];
            if ((startToGoal.x * startToEndEffector.y - startToEndEffector.x * startToGoal.y) < 0f)
                newAngle = angleDiff * Mathf.Rad2Deg;
            else
                newAngle = -angleDiff * Mathf.Rad2Deg;

            // Joint limit "clamping"
            newZAngle = limbs[l].transform.localRotation.eulerAngles.z + newAngle;

            if (newZAngle < 0)
                newZAngle += 360f;

            if (newZAngle < jointLimitsZ[l].x && newZAngle > jointLimitsZ[l].y)
            {
                if(newAngle < 0)
                {
                    newZAngle = jointLimitsZ[l].x;
                }
                else
                {
                    newZAngle = jointLimitsZ[l].y;
                }
            }


            limbs[l].transform.localRotation = Quaternion.Euler(newXAngle, newYAngle, newZAngle);

            // if instant is set to true will run for-loop extra times
            if (instant && l == 0)
            {
                l = limbs.Length - 1;
                maxIts--;
                if (maxIts == 0)
                {
                    break;
                }
            }
        }

        limbs[0].transform.rotation = Quaternion.Euler(limbs[0].transform.rotation.eulerAngles.x, oldYRotation, limbs[0].transform.rotation.eulerAngles.z);
        Quaternion lookQuat;
        float goalDirDiff = Vector3.SignedAngle(Quaternion.AngleAxis(-90, Vector3.up) * new Vector3(localGoal.x, 0, localGoal.z), limbs[0].transform.forward, Vector3.up);

        if (goalDirDiff < 0)
        {
            lookQuat = Quaternion.LookRotation(Quaternion.AngleAxis(-90, Vector3.up) * new Vector3(localGoal.x, 0, localGoal.z), limbs[0].transform.up);
        }
        else
        {
            lookQuat = Quaternion.LookRotation(Quaternion.AngleAxis(-90, Vector3.up) * new Vector3(localGoal.x, 0, localGoal.z), limbs[0].transform.up);
        }


        if (lookQuat.eulerAngles.y < hipLimits.x || lookQuat.eulerAngles.y > hipLimits.y)
        {
            if (goalDirDiff < 0)
            {
                lookQuat = Quaternion.LookRotation(Quaternion.AngleAxis(hipLimits.x, Vector3.up) * Vector3.forward, limbs[0].transform.up);
            }
            else
            {
                lookQuat = Quaternion.LookRotation(Quaternion.AngleAxis(hipLimits.y, Vector3.up) * Vector3.forward, limbs[0].transform.up);
            }
        }

        

        if (Mathf.Abs(goalDirDiff) > 0.1)
        {
            

            limbs[0].transform.rotation = Quaternion.Slerp(limbs[0].transform.rotation, lookQuat, elapsedTime / duration);
            if (elapsedTime >= duration)
            {
                elapsedTime = 0;
            }
            else
            {
                elapsedTime += Time.deltaTime;
            }
        }
        else
        {
            limbs[0].transform.rotation = lookQuat;
            elapsedTime = 0;
        }
    }

    public Vector3 GetEndpoint()
    {
        return limbs[limbs.Length-1].transform.position + -limbs[limbs.Length-1].transform.up * limbs[limbs.Length - 1].transform.localScale.y;
    }
}
