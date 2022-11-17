using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FABRIKArm : MonoBehaviour
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

    public Vector3[] points;

    // Start is called before the first frame update
    void Start()
    {
        points = new Vector3[limbs.Length + 1];
        for(int i = 0; i < limbs.Length + 1; i++)
        {
            if(i != limbs.Length)
            {
                points[i] = limbs[i].transform.position;
            }
            else
            {
                points[i] = GetEndpoint();
            }
            
        }
        raycaster.position = new Vector3(GetEndpoint().x, raycaster.position.y, GetEndpoint().z + 3f);
        goal = GetEndpoint();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < points.Length; i++)
        {
            Debug.Log(i + ": " + points[i]);
            
        }
        
        if (mouse)
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
            if (Input.GetKeyDown(KeyCode.A))
            {
                jointLimits = (jointLimits + 1) % 3;
                if (jointLimits == 0)
                {
                    for (int i = 0; i < jointLimitsZ.Length; i++)
                    {
                        jointLimitsZ[i] = Vector2.zero;
                    }
                }
                else if (jointLimits == 1)
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
        if (!spider)
        {
            goalDraw.position = goal;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.color += Color.Lerp(Color.red, Color.blue, (float)i/(points.Length - 1));
            Gizmos.DrawWireSphere(points[i], 0.5f + i * 0.15f);
        }
        
        //Gizmos.DrawWireSphere(goal, 0.5f);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(GetEndpoint(), 0.5f);
        //Gizmos.DrawRay(raycaster.position, Vector3.down * 1000);
    }



    void Solve()
    {
        Vector3 startToGoal, startToEndEffector;
        float dotProd, angleDiff;
        float newAngle = 0f;

        float maxIts = 5;

        Vector3 newPoint = goal;
        float nextLength = 0;

        for (int l = points.Length - 1; l >= 0; l--)
        {

            if (l != 0)
            {
                nextLength = (points[l - 1] - points[l]).magnitude;
                Debug.Log(nextLength);
            }
                points[l] = newPoint;
            if(l != 0)
            {
                newPoint = points[l] + (points[l - 1] - points[l]).normalized * nextLength;
            }
        }

        newPoint = transform.position;
        nextLength = 0;

        for (int l = 0; l < points.Length; l++)
        {

            if (l != points.Length - 1)
            {
                nextLength = (points[l + 1] - points[l]).magnitude;
                
            }
            points[l] = newPoint;
            if (l != points.Length - 1)
            {
                newPoint = points[l] + (points[l + 1] - points[l]).normalized * nextLength;
            }
        }

        for (int l = 0; l < limbs.Length; l++)
        {
            limbs[l].transform.position = points[l];
            limbs[l].transform.LookAt(points[l + 1], transform.up);
            limbs[l].transform.RotateAround(limbs[l].transform.position, limbs[l].transform.right, -90);
        }
    }

    public Vector3 GetEndpoint()
    {
        return limbs[limbs.Length - 1].transform.position + -limbs[limbs.Length - 1].transform.up * limbs[limbs.Length - 1].transform.localScale.y;
    }
}
