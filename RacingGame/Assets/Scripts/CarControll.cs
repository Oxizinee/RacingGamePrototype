using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CarControll : MonoBehaviour
{
    [Header("Tires")]
    public GameObject[] Wheels;
    public Transform direction;

    public float forwardSpeed;
    public float reverseSpeed;
    public LayerMask groundLayer;

    private float moveInput;
    private float turnInput;
    public bool isCarGrounded;

    private float normalDrag;
    public float modifiedDrag;

    //Steering
    public float currentSpeed;
    public float speed = 200;
    public float maxVelocity =50;
    public float rotationSpeed = 35;

    ///Swinging
    public float SwingSpeed;
    public bool Swinging;
    public bool IsInSwingingRadius = false;

    //Fliping back up
    public bool isUpsideDown = false;
    public float _resetCarTimer = 3;

    public Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
          normalDrag = rb.drag;
    }
    private void Update()
    {
        // Get input from the player
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        currentSpeed = rb.velocity.magnitude;
        // Raycast to the ground and get normal to align car with it.
        RaycastHit hit;
        isCarGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, groundLayer);

        // Calculate Movement Direction
        moveInput *= moveInput > 0 ? forwardSpeed : reverseSpeed;

        //    // Calculate Drag
           rb.drag = isCarGrounded ? normalDrag : modifiedDrag;

        FlipBackUp();
    }

    private void FlipBackUp()
    {
        if (isUpsideDown)
        {
            _resetCarTimer -= Time.deltaTime;
            if (_resetCarTimer < 0)
            {
                transform.rotation = Quaternion.identity;
                _resetCarTimer = 3;
                isUpsideDown = false;
            }
        }
    }

    private void FixedUpdate()
    {
        MoveForward();

        RotateCar(turnInput);
        RotateWheels();

    }
    private void MoveForward()
    {
        if (Swinging) return;

        if (rb.velocity.magnitude >= maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }

        if (isCarGrounded)
        {
            rb.AddForce(moveInput * transform.forward * rb.mass * speed * Time.fixedDeltaTime, ForceMode.Force);
        }
        else
        {
            rb.AddForce(transform.up * -500f);
        }
    }
    void RotateCar(float input)
    {
        if (rb.velocity.magnitude >= 0.5f)
        {
            transform.Rotate(0, 30f * Time.deltaTime * input, 0);
        }
    }
    private void RotateWheels()
    {
        foreach (GameObject w in Wheels)
        {
            w.transform.Rotate(0, rb.velocity.magnitude * 0.1f * Time.fixedTime, 0);
        }
    }
}


