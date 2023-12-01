using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    public float speed = 2500;
    public float rotationStrenght = 0.2f;
    public Transform direction;
    public GameObject[] Wheels;

    private Rigidbody rb;
    private float maxVelocity = 50;
    private float verticalInput, horizontalInput;
    //private float maxWheelAngle = 0.4f;
    public float wheelAngle = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        MoveForward();
        RotateWheels();
        RotateCar();
    }

    private void RotateCar()
    {
        if (rb.velocity.magnitude >= 0.5f)
        {
            transform.Rotate(0, 30f * Time.deltaTime * horizontalInput, 0);
        }
    }

    private void RotateWheels()
    {
        foreach (GameObject w in Wheels)
        {
            w.transform.Rotate(0, rb.velocity.magnitude * 0.1f * Time.fixedTime, 0);
        }
    }

    private void MoveForward()
    {
        if (rb.velocity.magnitude >= maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }

        rb.AddForce(verticalInput * rb.transform.forward * rb.mass * speed * Time.fixedDeltaTime, ForceMode.Force);
    }
}
