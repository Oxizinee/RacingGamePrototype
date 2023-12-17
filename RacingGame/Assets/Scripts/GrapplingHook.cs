using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public CarControll CarControll;

    public LineRenderer lineRenderer;
    public Transform gunTip, player;
    public LayerMask Grappleable;
    public GameObject Target = null;

    //Prediction
    public float predictionSphereRadius;
    public RaycastHit predictionHit;
    public Transform predictionPoint;

    //MidAirControl
    public Rigidbody rb;
    public float forwardThrustForce;
    public float upForce;

    //Swinging 
    public float maxSwingDistance = Mathf.Infinity;
    private Vector3 swingPoint;
    private SpringJoint joint;

    private void Awake()
    {
        CarControll = GetComponent<CarControll>();
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (/*Input.GetMouseButtonDown(0) && CarControll.IsInSwingingRadius || */Input.GetButtonDown("Fire1") && CarControll.IsInSwingingRadius)
        {
            StartSwing();
        }
        if (Input.GetMouseButtonUp(0) || Input.GetButtonUp("Fire1"))
        {
            StopSwing();
        }

        CheckForSwingPoints();
    }
    private void FixedUpdate()
    {
        if (CarControll.Swinging)
        {
            rb.AddForce(transform.forward * forwardThrustForce * Time.deltaTime, ForceMode.VelocityChange);
            //rb.AddForce(Vector3.up * upForce, ForceMode.Force);
            //StartCoroutine(SwingingMovement());
            // rb.AddForce(-transform.up * 90 * Time.deltaTime, ForceMode.Force);
        }
    }
    private void LateUpdate()
    {
        DrawRope();
    }

    private void DrawRope()
    {
        if (!joint) return;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, gunTip.position);
        lineRenderer.SetPosition(1, predictionHit.transform.position);
    }
    private void CheckForSwingPoints()
    {
        if (joint != null) return;

        RaycastHit sphereCastHit;
        Physics.SphereCast(gunTip.position, predictionSphereRadius, gunTip.forward,
                            out sphereCastHit, maxSwingDistance, Grappleable);

        RaycastHit raycastHit;
        Physics.Raycast(gunTip.position, gunTip.forward,
                            out raycastHit, maxSwingDistance, Grappleable);

        Vector3 realHitPoint;

        // Option 1 - Direct Hit
        if (raycastHit.point != Vector3.zero)
            realHitPoint = raycastHit.point;

        // Option 2 - Indirect (predicted) Hit
        else if (sphereCastHit.point != Vector3.zero)
            realHitPoint = sphereCastHit.point;

        // Option 3 - Miss
        else
            realHitPoint = Vector3.zero;

        // realHitPoint found
        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }
        // realHitPoint not found
        else
        {
            predictionPoint.gameObject.SetActive(false);
        }

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }
    private void StartSwing()
    {
        if (predictionHit.point == Vector3.zero) return;
        
        swingPoint = predictionHit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

        // the distance grapple will try to keep from grapple point. 
        joint.maxDistance = distanceFromPoint * 0.4f;
        joint.minDistance = distanceFromPoint * 0.25f;

        // customize values as you like
        joint.damper = 7f;

        CarControll.Swinging = true;
    }
    private void StopSwing()
    {
        CarControll.Swinging = false;

        lineRenderer.positionCount = 0;
        Destroy(joint);
        Target = null;
    }
}
