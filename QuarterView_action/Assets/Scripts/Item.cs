using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    public enum Type {Ammo,Coin,Grenade,Heart,Weapon };
    public Type type;
    public int value;

    SphereCollider collid;
    Rigidbody rigid;


    private void Awake()
    {
        collid = GetComponent<SphereCollider>();
        rigid = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true;
            collid.enabled = false;
        }
    }

}
