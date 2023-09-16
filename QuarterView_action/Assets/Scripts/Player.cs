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
    bool isFire;
    bool isReload;
    bool reloading;
    bool fireReady=true;
    float fireDelay;
    public Camera followCamera;


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
    Weapon equipWeapon;

    public GameObject[] Weapons;
    public bool[] gotWeapons;
    int weaponIdx;

    public GameObject[] gotGrenades;
    public int grenade;

    public int hearts;
    public int ammo;
    public int coin;
   

    public int max_heart;
    public int max_ammo;
    public int max_coin;
    public int max_grenade;


   

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
        Attack();
        Reload();
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
        isFire = Input.GetButton("Fire1");
        isReload=Input.GetButtonDown("Reload");


        isKey1 = Input.GetButtonDown("Swap1");
        isKey2 = Input.GetButtonDown("Swap2");
        isKey3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        movVec = new Vector3(h_input, 0, v_input);

        if (isDodge)
            movVec = dodgeVec;

        if (!fireReady||reloading)
            movVec = Vector3.zero;
        transform.position += movVec * speed * ((isWalk) ? 0.3f : 1f) * Time.deltaTime;

       
        anim.SetBool("IsRun", movVec != Vector3.zero);
        anim.SetBool("IsWalk", isWalk);
    }

    void Turn()
    {
        //Rotate a player to the moving direction (by keyboard)
        transform.LookAt(transform.position + movVec);

        //Rotate a player to the cursor direction
        Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;

        if(Physics.Raycast(ray, out rayHit, 100))
        {
            if (isFire)
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
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
            equipWeapon = Weapons[activeWeapon].GetComponentInChildren<Weapon>();

            if (prevWeapon!=-1)
                Weapons[prevWeapon].SetActive(false);
            prevWeapon = activeWeapon;


            anim.SetTrigger("DoSwap");
            isSwap = true;
            Invoke("ExitSwap", 0.5f);

        }
    }

    private void Attack()
    {
        if (equipWeapon == null)
            return;

       
        fireDelay += Time.deltaTime;
        fireReady = fireDelay > equipWeapon.rate;

        if(fireReady && isFire && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type==Weapon.Type.Melee?"DoSwing":"DoShot");
            fireDelay = 0;
        }
    }

    private void Reload()
    {
        if (equipWeapon == null)
            return;

        if (equipWeapon.type == Weapon.Type.Melee)
            return;

        if (ammo <= 0)
            return;

        if (isReload && !isSwap && !isFire && !isJump && !isDodge)
        {
            reloading = true;
            anim.SetTrigger("DoReload");
            Invoke("ReloadOut", 3f);
        }
    }

    void ReloadOut()
    {
        int re_ammo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = re_ammo;
        ammo -= re_ammo;
        reloading = false;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item"|| other.tag == "Coin")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type) 
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > max_ammo)
                        ammo = max_ammo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > max_coin)
                        coin = max_coin;
                    break;
                case Item.Type.Grenade:
                    if (grenade >= max_grenade)
                        return;
                    gotGrenades[grenade].SetActive(true);
                    grenade += item.value;
                   
                    break;
              
                case Item.Type.Heart:
                    hearts += item.value;
                    if (hearts > max_heart)
                        hearts = max_heart;
                    break;



            }


            Destroy(other.gameObject);
        }
    }
}
