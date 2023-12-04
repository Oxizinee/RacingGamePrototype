using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleRadius : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject shower;
    public GameObject target;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.gameObject.tag == "Player")
        {
            other.gameObject.GetComponentInParent<CarControll>().IsInSwingingRadius = true;
            other.gameObject.GetComponentInParent<GrapplingHook>().Target = target; 
            shower.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.gameObject.tag == "Player")
        {
            other.gameObject.GetComponentInParent<CarControll>().IsInSwingingRadius = false;
            shower.SetActive(false);
        }
    }
    void Awake()
    {
        shower.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
