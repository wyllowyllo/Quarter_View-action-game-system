using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float h_input;
    float v_input;
    bool isWalk;
    bool isJump;
    bool isJumping;
    bool isDodge;
    Vector3 movVec;
    Vector3 dodgeVec;
    public float speed;
    public float jumpPower;
    Animator anim;
    Rigidbody rigid;


   

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {

        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
    }

   

    void GetInput()
    {
        h_input = Input.GetAxisRaw("Horizontal");
        v_input = Input.GetAxisRaw("Vertical");

        isWalk = Input.GetButton("Walk");
        isJump = Input.GetButtonDown("Jump");
    }

    void Move()
    {
        movVec = new Vector3(h_input, 0, v_input);

        if (isDodge)
            movVec = dodgeVec;
        transform.position += movVec * speed * ((isWalk) ? 0.3f : 1f) * Time.deltaTime;

       
        anim.SetBool("IsRun", movVec != Vector3.zero);
        anim.SetBool("IsWalk", isWalk);
    }

    void Turn()
    {
        //Rotate a player to the moving direction
        transform.LookAt(transform.position + movVec);
    }

    void Jump()
    {
        if (isJump && !isJumping && !isDodge&&movVec==Vector3.zero)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isJumping = true;
            anim.SetTrigger("DoJump");
            anim.SetBool("IsJump", true);
        }
    }

    private void Dodge()
    {
        if (isJump && !isJumping&&!isDodge && movVec != Vector3.zero)
        {
            dodgeVec = movVec;
            speed *= 2;
            isDodge = true;
            anim.SetTrigger("DoDodge");
            

            Invoke("ExitDodge", 0.5f);
        }
    }

    void ExitDodge()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJumping = false;
            anim.SetBool("IsJump", false);
        }
    }
}
