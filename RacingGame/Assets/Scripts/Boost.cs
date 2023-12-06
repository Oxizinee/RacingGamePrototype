using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour
{
    private void OnCollisionStay(Collision collision)
    {
        if (collision.rigidbody.gameObject.tag == "Player")
        {
            collision.rigidbody.AddForce(transform.forward * 50, ForceMode.Impulse);
        }
    }
}
