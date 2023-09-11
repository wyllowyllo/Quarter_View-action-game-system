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
    bool isSwap;


    bool isInteract;
    bool isKey1;
    bool isKey2;
    bool isKey3;
    int prevWeapon=-1;
    Vector3 movVec;
    Vector3 dodgeVec;
    
    public float speed;
    public float jumpPower;
    Animator anim;
    Rigidbody rigid;
    GameObject nearObject;
    GameObject equipWeapon;

    public GameObject[] Weapons;
    public bool[] gotWeapons;
    int weaponIdx;


   

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
        Interact();
        Swap();
    }

   

    void GetInput()
    {
        h_input = Input.GetAxisRaw("Horizontal");
        v_input = Input.GetAxisRaw("Vertical");

        isWalk = Input.GetButton("Walk");
        isJump = Input.GetButtonDown("Jump");
        isInteract = Input.GetButtonDown("Interaction");

        isKey1 = Input.GetButtonDown("Swap1");
        isKey2 = Input.GetButtonDown("Swap2");
        isKey3 = Input.GetButtonDown("Swap3");
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
        if (isJump && !isJumping && !isDodge&& !isSwap && movVec==Vector3.zero)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isJumping = true;
            anim.SetTrigger("DoJump");
            anim.SetBool("IsJump", true);
        }
    }

    private void Dodge()
    {
        if (isJump && !isJumping&&!isDodge && !isSwap && movVec != Vector3.zero)
        {
            dodgeVec = movVec;
            speed *= 2;
            isDodge = true;
            anim.SetTrigger("DoDodge");
            

            Invoke("ExitDodge", 0.5f);
        }
    }

    private void Interact()
    {

        if (isInteract&&nearObject != null && !isJump && !isDodge)
        {
            Item item = nearObject.GetComponent<Item>();
            weaponIdx = item.value;

            gotWeapons[weaponIdx] = true;

            Destroy(nearObject);
        }
    }

    private void Swap()
    {
        int activeWeapon = -1;
        
        if (isKey1) activeWeapon = 0;
        if (isKey2) activeWeapon = 1;
        if (isKey3) activeWeapon = 2;

        if ((isKey1 || isKey2 || isKey3)&&!isDodge)
        {

            if (!gotWeapons[activeWeapon]||activeWeapon==prevWeapon)
                return;


            Weapons[activeWeapon].SetActive(true);
            equipWeapon = Weapons[activeWeapon];

            if (prevWeapon!=-1)
                Weapons[prevWeapon].SetActive(false);
            prevWeapon = activeWeapon;


            anim.SetTrigger("DoSwap");
            isSwap = true;
            Invoke("ExitSwap", 0.5f);

        }
    }

    void ExitDodge()
    {
        speed *= 0.5f;
        isDodge = false;
    }
    void ExitSwap()
    {
      
        isSwap = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJumping = false;
            anim.SetBool("IsJump", false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = other.gameObject;
          

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}
