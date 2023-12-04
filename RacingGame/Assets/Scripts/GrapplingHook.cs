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

    //MidAirControl
    public Transform orientation;
    public Rigidbody rb;
    public float horizontalThrustForce;
    public float forwardThrustForce;
    public float extendCableSpeed;

    //Swinging 
    public float maxSwingDistance = 25;
    private Vector3 swingPoint;
    private SpringJoint joint;

    private Vector3 _currentGrapplePos;
    private void Awake()
    {
        CarControll = GetComponent<CarControll>();
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && CarControll.IsInSwingingRadius)
        {
            StartSwing();
        }
        if (Input.GetMouseButtonUp(0))
        {
            StopSwing();
        }

        if (joint != null) OdmGearMovement();
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void DrawRope()
    {
        if (!joint) return;

        _currentGrapplePos =
            Vector3.Lerp(_currentGrapplePos, swingPoint, Time.deltaTime * 8f);

        lineRenderer.SetPosition(0, gunTip.position);
        lineRenderer.SetPosition(1, _currentGrapplePos);
    }
    private void StartSwing()
    {
        CarControll.Swinging = true;

        RaycastHit hit;
        if (Physics.Raycast(gunTip.transform.position, Target.transform.position, out hit, maxSwingDistance, Grappleable))
        {
            swingPoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;

            float distance = Vector3.Distance(player.position, swingPoint);

            joint.maxDistance = distance * 0.8f;
            joint.minDistance = distance * 0.25f;

            //customize
            joint.spring = 4.5f;
            joint.damper = 7;
            joint.massScale = 4.5f;

            lineRenderer.positionCount = 2;
            _currentGrapplePos = gunTip.position;
        }
    }
    private void StopSwing()
    {
        CarControll.Swinging = false;

        lineRenderer.positionCount = 0;
        Destroy(joint);
    }

    private void OdmGearMovement()
    {
        // right
        if (Input.GetKey(KeyCode.D)) rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
        // left
        if (Input.GetKey(KeyCode.A)) rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);

        // forward
        if (Input.GetKey(KeyCode.W)) rb.AddForce(orientation.forward * horizontalThrustForce * Time.deltaTime);

        // shorten cable
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 directionToPoint = swingPoint - transform.position;
            rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
        }
        // extend cable
        if (Input.GetKey(KeyCode.S))
        {
            float extendedDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendCableSpeed;

            joint.maxDistance = extendedDistanceFromPoint * 0.8f;
            joint.minDistance = extendedDistanceFromPoint * 0.25f;
        }
    }
}
