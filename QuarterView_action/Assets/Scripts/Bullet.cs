using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3.0f);
        }
       
    }

    private void OnTriggerEnter(Collider other)
    {
         if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
        }
    }
}
