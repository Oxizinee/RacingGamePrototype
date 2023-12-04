using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckUpsideDown : MonoBehaviour
{
    private CarControll _carControll;
    private void Awake()
    {
        _carControll = GetComponentInParent<CarControll>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Floor")
            _carControll.isUpsideDown = true;
    }

}
