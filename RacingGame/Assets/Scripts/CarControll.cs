using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CarControll : MonoBehaviour
{
    public float fwdSpeed;
    public float revSpeed;
    public float turnSpeed;
    public LayerMask groundLayer;

    private float moveInput;
    private float turnInput;
    public bool isCarGrounded;

    private float normalDrag;
    public float modifiedDrag;

    public float alignToGroundTime;

    //Swinging
    public float SwingSpeed;
    public bool Swinging;
    public bool IsInSwingingRadius = false;


    public LayerMask floorMask;
    public bool isUpsideDown = false;
    public float _resetTimer = 3;

    public Rigidbody rb;
    private void Awake()
    {
        rb.transform.parent = null;
        normalDrag = rb.drag;
    }
    private void Update()
    {
        // Get input from the player
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        float newRot = turnInput * turnSpeed * Time.deltaTime * moveInput;

        if (isCarGrounded)
            transform.Rotate(0, newRot, 0, Space.World);

        // Set Cars Position to Our Sphere
        transform.position = rb.transform.position;

        // Raycast to the ground and get normal to align car with it.
        RaycastHit hit;
        isCarGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, groundLayer);

        // Rotate Car to align with ground
        Quaternion toRotateTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotateTo, alignToGroundTime * Time.deltaTime);

        // Calculate Movement Direction
        moveInput *= moveInput > 0 ? fwdSpeed : revSpeed;

        // Calculate Drag
        rb.drag = isCarGrounded ? normalDrag : modifiedDrag;

        FlipBackUp();

    }

    private void FlipBackUp()
    {
        if (isUpsideDown)
        {
            _resetTimer -= Time.deltaTime;
            if (_resetTimer < 0)
            {
                transform.rotation = Quaternion.identity;
                _resetTimer = 3;
                isUpsideDown = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isCarGrounded)
            rb.AddForce(transform.forward * moveInput, ForceMode.Acceleration); // Add Movement
        else
            rb.AddForce(transform.up * -200f); // Add Gravity
    }

}
