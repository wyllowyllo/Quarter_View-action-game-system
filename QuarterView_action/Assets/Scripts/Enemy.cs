using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    BoxCollider collid;
    Material material;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        collid = GetComponent<BoxCollider>();
        material = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Melee")
        {
            Weapon weapon = other.gameObject.GetComponent<Weapon>();
            curHealth -= weapon.damage;

        }
        else if (other.gameObject.tag == "Bullet")
        {
            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            curHealth -= bullet.dagame;
        }

        Vector3 reactVec = transform.position - other.gameObject.transform.position;
      
        StartCoroutine(OnDamage(reactVec));
    
    Debug.Log("cur health: " + curHealth);
    }

    IEnumerator OnDamage(Vector3 reactVec)
    {
        material.color = Color.red;

        yield return new WaitForSeconds(0.1f);


        if (curHealth > 0)
        {
            material.color = Color.white;
        }
        else
        {
            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec*5, ForceMode.Impulse);

            material.color = Color.gray;
            gameObject.layer = 11;
            Destroy(gameObject, 4.0f);
        }
        
    }
   
}
