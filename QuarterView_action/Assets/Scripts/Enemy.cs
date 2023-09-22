using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    BoxCollider collid;
    Material material;
    public BoxCollider meleeArea;
    public Transform target;
    NavMeshAgent nav;
    Animator anim;

    bool isAttack;
    bool isChase;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        collid = GetComponent<BoxCollider>();
        material = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("StartChase", 2.0f);
    }


    private void FixedUpdate()
    {
        FreezeVelocity();
        Targeting();
    }

    private void Targeting()
    {
        float targetRadius = 1.5f;
        float targetRange = 3f;

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
            targetRadius,
            transform.forward,
            targetRange,
            LayerMask.GetMask("Player"));

        if (rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }

    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);
      
        yield return new WaitForSeconds(0.2f);
        meleeArea.enabled = true;
       
        yield return new WaitForSeconds(1f);
        meleeArea.enabled = false ;

        yield return new WaitForSeconds(1f);
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }
    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void StartChase()
    {
        anim.SetBool("isWalk", true);
        isChase = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (nav.enabled) {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
           
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Melee")
        {
            Weapon weapon = other.gameObject.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.gameObject.transform.position;
            StartCoroutine(OnDamage(reactVec, false));

        }
        else if (other.gameObject.tag == "Bullet")
        {
            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.gameObject.transform.position;
            StartCoroutine(OnDamage(reactVec, false));
        }

        
    
    Debug.Log("cur health: " + curHealth);
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = explosionPos - transform.position;
        StartCoroutine(OnDamage(reactVec,true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        material.color = Color.red;

        yield return new WaitForSeconds(0.1f);


        if (curHealth > 0)
        {
            material.color = Color.white;
        }
        else
        {

            if (isGrenade)
            {
               
                reactVec = reactVec.normalized;
                reactVec += Vector3.up*3;
                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }

            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");
            material.color = Color.gray;
            gameObject.layer = 11;
            Destroy(gameObject, 4.0f);
        }
        
    }
   
}
