using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarControll))]
public class CarInput : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject aimCamera;
    public bool IsAiming = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !aimCamera.activeInHierarchy)
        {
            IsAiming = true;
            mainCamera.SetActive(false);
            aimCamera.SetActive(true);
        }

        else if (Input.GetMouseButtonUp(1) && !mainCamera.activeInHierarchy)
        {
            IsAiming = false;
            mainCamera.SetActive(true);
            aimCamera.SetActive(false);
        }
    }
}
