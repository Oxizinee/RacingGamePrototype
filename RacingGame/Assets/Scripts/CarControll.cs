using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CarControll : MonoBehaviour
{
    // Settings
    public float MoveSpeed = 50;
    public float MaxSpeed = 15;
    public float Drag = 0.98f;
    public float SteerAngle = 20;
    public float Traction = 1;
    public float RotationSpeed = 20;

    // Variables
    private Vector3 MoveForce;

    public Transform followTransform;
    public GameObject CenterAim;

    private CarInput _carInput; 
    private void Awake()
    {
        _carInput = GetComponent<CarInput>();
    }
    void FixedUpdate()
    {

        // Moving
        MoveForce += transform.forward * MoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        transform.position += MoveForce * Time.deltaTime;

        // Steering
        float steerInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * steerInput * MoveForce.magnitude * SteerAngle * Time.deltaTime);

        // Drag and max speed limit
        MoveForce *= Drag;
        MoveForce = Vector3.ClampMagnitude(MoveForce, MaxSpeed);

        // Traction
        Debug.DrawRay(transform.position, MoveForce.normalized * 3);
        Debug.DrawRay(transform.position, transform.forward * 3, Color.blue);
        MoveForce = Vector3.Lerp(MoveForce.normalized, transform.forward, Traction * Time.deltaTime) * MoveForce.magnitude;
    }
    private void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        if (_carInput.IsAiming)
        {
            CenterAim.SetActive(true);
            followTransform.transform.localEulerAngles = new Vector3(-mousePosition.y * RotationSpeed, mousePosition.x * RotationSpeed, followTransform.transform.localEulerAngles.z); 
        }
        else
        {
            CenterAim.SetActive(false);
            followTransform.transform.localEulerAngles = new Vector3(0,0,0);
        }
    }
}
