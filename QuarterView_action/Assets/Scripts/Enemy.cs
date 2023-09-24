using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum EnemyType { A,B,C,D};
    public EnemyType type;
    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    BoxCollider collid;
    MeshRenderer[] meshs;
    public BoxCollider meleeArea;
    public Transform target;
    NavMeshAgent nav;
    Animator anim;
    public GameObject bullet;

    bool isAttack;
    bool isChase;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        collid = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if(type!=EnemyType.D)
            Invoke("StartChase", 2.0f);
    }


    private void FixedUpdate()
    {
        FreezeVelocity();
        Targeting();
    }

    private void Targeting()
    {
        if(type != EnemyType.D)
        {
            float targetRadius = 0f;
            float targetRange = 0f;

            //타격 범위 설정
            switch (type)
            {
                case EnemyType.A:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
                case EnemyType.B:
                    targetRadius = 1f;
                    targetRange = 12f;
                    break;
                case EnemyType.C:
                    targetRadius = 0.5f;
                    targetRange = 25f;
                    break;
            }

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
        

    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);


        switch (type)
        {
            case EnemyType.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;
            case EnemyType.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;
            case EnemyType.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
                bulletRigid.velocity = transform.forward * 30;
                yield return new WaitForSeconds(2f);
                break;
        }

       
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
        if (nav.enabled&&type!=EnemyType.D) {
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
        foreach(MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;


        yield return new WaitForSeconds(0.1f);


        if (curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;
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
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
            gameObject.layer = 11;

            if(type!=EnemyType.D)
                Destroy(gameObject, 4.0f);
        }
        
    }
   
}
