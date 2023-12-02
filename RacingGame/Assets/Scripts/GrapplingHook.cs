using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform gunTip, player;
    public LayerMask Grappleable;
    private CarInput _carInput;

    //swinging 
    public float maxSwingDistance = 25;
    private Vector3 swingPoint;
    private SpringJoint joint;

    private Vector3 _currentGrapplePos;
    private void Awake()
    {
        _carInput = GetComponent<CarInput>();   
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _carInput.IsAiming)
        {
            StartSwing();
        }
        if (Input.GetMouseButtonUp(0))
        {
            StopSwing();
        }
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
        RaycastHit hit;
      //  Vector3 mouseRay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxSwingDistance, Grappleable))
        {
            //Debug.DrawRay(mouseRay, Camera.main.transform.forward, Color.red);
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
        lineRenderer.positionCount = 0;
        Destroy(joint);
    }
}
