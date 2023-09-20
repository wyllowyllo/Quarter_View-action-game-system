using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject mesh;
    public GameObject explosion;
    public Rigidbody rigid;

    private void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {

        yield return new WaitForSeconds(3.0f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        mesh.SetActive(false);
        explosion.SetActive(true);

        RaycastHit[] hitObjs = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("Enemy"));


        foreach(RaycastHit hitObj in hitObjs)
        {
           
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);   
        }

        Destroy(gameObject, 5.0f);
    }
}
