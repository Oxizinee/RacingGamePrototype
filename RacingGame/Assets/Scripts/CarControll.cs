using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CarControll : MonoBehaviour
{
    public float gravity;
    public GameObject groundCheck;
    public GameObject start;
    [Header("Tires")]
    public GameObject[] Wheels;
    public Transform direction;

    [Header("Speed")]
    public float forwardSpeed;
    public float reverseSpeed;
    public float drifitingSpeed;
    public LayerMask groundLayer;

    [Header("Movement")]
    private Vector2 moveInput;
    private float turnInput;
    public bool isCarGrounded;
    public bool drift = false;
    public bool respawn = false;

    [Header("Drag")]
    private float normalDrag;
    public float airDrag;
    public float snapDistance = 2f;

    [Header("Sterring")]
    public float currentSpeed;
    public float maxVelocity = 250;
    public float rotationSpeed = 35;

    [Header("Swinging")]
    public bool Swinging;
    public bool IsInSwingingRadius = false;

    [Header("Flipping back up")]
    public bool isUpsideDown = false;
    public float _resetCarTimer = 3;

    public Rigidbody rb;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Respawn")
        {
            respawn = true;
        }
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        normalDrag = rb.drag;
    }
    private void Update()
    {
        // Get input from the player
        moveInput = new Vector2(Input.GetAxis("RightTriggerAxis"), Input.GetAxis("LeftTriggerAxis")); //Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        // Raycast to the ground and get normal to align car with it - normal ground Check!
           RaycastHit hit;
           isCarGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, groundLayer);


        // Snapping the car to the surface
        SnapToSurface();
       

        // Calculate Movement Direction
        moveInput *= moveInput.x > 0 ? forwardSpeed : reverseSpeed;

        //   // Calculate Drag
        rb.drag = isCarGrounded ? normalDrag : airDrag;

        FlipBackUp();
        Respawn();
    }
    private void Respawn()
    {
        if (respawn)
        {
            transform.position = start.transform.position;
            transform.rotation = start.transform.rotation;
            currentSpeed = 0;
            float t = 0;
            //if (t < 6)
            //{
            //    t += Time.deltaTime * 0.5f;
            //    currentSpeed = 0;
            //}
            respawn = false;
        }
    }
    private void FlipBackUp()
    {
        if (isUpsideDown)
        {
            _resetCarTimer -= Time.deltaTime;
            if (_resetCarTimer < 0)
            {
                if (transform.rotation.y >= 0)
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.x, -180, transform.rotation.z);
                }
                _resetCarTimer = 3;
                isUpsideDown = false;
            }
        }
    }
    private void SnapToSurface()
    {
        RaycastHit hit;

        // Raycast downwards to detect the surface below the car
        if (Physics.Raycast(transform.position, -transform.up, out hit, 0.5f, groundLayer))
        {
            // Calculate the distance between the car and the surface
            float distanceToSurface = hit.distance;

            // Snap the car to the surface if it's below the snap distance
            if (distanceToSurface < snapDistance)
            {
                groundCheck.transform.position = new Vector3(groundCheck.transform.position.x, hit.point.y, groundCheck.transform.position.z); // Snap to the surface
                transform.up = hit.normal;      // Align with the surface normal
            }
        }
    }
    private void FixedUpdate()
    {
        Move();

        RotateCar(turnInput);
        RotateWheels();

        MidAirControl();

    }
    private void MidAirControl()
    {
        if (!isCarGrounded || !respawn)
        {
            //Vector3 currentRotation = transform.rotation.eulerAngles;
            //float clampedPitch = Mathf.Clamp(currentRotation.x, -maxPitchAngle, maxPitchAngle);

            //Quaternion targetRotation = Quaternion.Euler(clampedPitch, currentRotation.y, currentRotation.z);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            rb.AddTorque(Vector3.Cross(transform.up, Vector3.up) * 5f);
        }
    }
    private void Move()
    {
        if (Swinging) return;
        if(respawn) return;

        if (isCarGrounded)
        {
            if (moveInput.x > 0)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, maxVelocity, Time.deltaTime * 0.5f);
            }
            else if (moveInput.y > 0)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, -maxVelocity / 1.75f, 1f * Time.deltaTime);
            }
            else
            {
                currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 1.5f);
            }

            rb.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);
        }

        else //add gravity
        {
            rb.AddForce(-transform.up * gravity);
        }
    }
    void RotateCar(float input)
    {
        if (respawn) return;


        if (rb.velocity.magnitude >= 0.5f)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime * input, 0);

            if (Input.GetButton("Fire2"))
            {
                drift = true;
                rotationSpeed = 85;
                currentSpeed = Mathf.Lerp(currentSpeed, drifitingSpeed, 2f * Time.deltaTime);
            }
            else
            {
                drift = false;
                rotationSpeed = 70;
            }
        }

    }
    private void RotateWheels()
    {
        if(respawn) return;


        foreach (GameObject w in Wheels)
        {
            w.transform.Rotate(0, rb.velocity.magnitude * 0.1f * Time.fixedTime, 0);
        }
    }
}


