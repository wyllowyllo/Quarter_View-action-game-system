using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;
    private void OnCollisionEnter(Collision collision)
    {
        if (!isRock&&!isMelee && collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3.0f);
        }
       
    }

    private void OnTriggerEnter(Collider other)
    {
         if (!isMelee&&other.gameObject.tag == "Wall" || other.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
        }
    }
}
