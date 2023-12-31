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
    bool isThrowG;
    bool isWall;
    bool isDamage;
    bool isShop;
    bool isDead;
    bool reloading;
    bool fireReady=true;
    float fireDelay;
    public Camera followCamera;
    public GameManager gameManager;


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
    public Weapon equipWeapon;
    MeshRenderer[] meshs;

    public GameObject[] Weapons;
    public bool[] gotWeapons;
    int weaponIdx;
    public GameObject hasGrenadeObj;

    public GameObject[] gotGrenades;
    public int hasGrenade;

    public int hearts;
    public int ammo;
    public int coin;
    public int score;

    public int max_heart;
    public int max_ammo;
    public int max_coin;
    public int max_hasGrenade;


   

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();


        Debug.Log(PlayerPrefs.GetInt("MaxScore"));
        //PlayerPrefs.SetInt("MaxScore",24100);
    }
    // Update is called once per frame
    void Update()
    {

        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        ThrowG();
        Reload();
        Dodge();
        Interact();
        Swap();
        CollideWall();

    }

   

    private void FixedUpdate()
    {
        FreezeRotation();   
    }
    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
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
        isThrowG = Input.GetButtonDown("Fire2");

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

        if(!isWall && !isDead)
        transform.position += movVec * speed * ((isWalk) ? 0.3f : 1f) * Time.deltaTime;

       
        anim.SetBool("IsRun", movVec != Vector3.zero);
        anim.SetBool("IsWalk", isWalk);
    }

    void CollideWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.red);
        isWall = Physics.Raycast(transform.position, transform.forward ,5, LayerMask.GetMask("Wall"));

        
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
            if (isFire && !isDead)
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        if (isJump && !isJumping && !isDodge&& !isSwap && !isShop && !isDead && movVec==Vector3.zero)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isJumping = true;
            anim.SetTrigger("DoJump");
            anim.SetBool("IsJump", true);
        }
    }

    private void Dodge()
    {
        if (isJump && !isJumping&&!isDodge && !isSwap && !isShop && !isDead && movVec != Vector3.zero)
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

        if (isInteract&&nearObject != null && !isJump && !isDodge && !isDead)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                weaponIdx = item.value;

                gotWeapons[weaponIdx] = true;

                Destroy(nearObject);
            }
            else if (nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            }
        }
    }


    private void Swap()
    {
        int activeWeapon = -1;
        
        if (isKey1) activeWeapon = 0;
        if (isKey2) activeWeapon = 1;
        if (isKey3) activeWeapon = 2;

        if ((isKey1 || isKey2 || isKey3)&&!isDodge&&!isShop && !isDead)
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

        if(fireReady && isFire && !isDodge && !isSwap && !isShop && !isDead)
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

        if (isReload && !isSwap && !isFire && !isJump && !isDodge && !isShop && !isDead)
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

    private void ThrowG()
    {
        if (hasGrenade == 0)
            return;

        if(isThrowG && !isSwap && !isReload && !isShop && !isDead)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;

            if (Physics.Raycast(ray, out rayHit, 100))
            {
              
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 10;
                GameObject instantGrenade=Instantiate(hasGrenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back*10, ForceMode.Impulse);
            }

            hasGrenade--;
            gotGrenades[hasGrenade].SetActive(false);
        }
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
        if (other.tag == "Weapon"||other.tag=="Shop")
        {
            nearObject = other.gameObject;
          
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
        else if (other.tag == "Shop")
        {
           
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Exit();
                isShop = false;
                nearObject = null;
            
        }
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
                    if (hasGrenade >= max_hasGrenade)
                        return;
                    gotGrenades[hasGrenade].SetActive(true);
                    hasGrenade += item.value;
                   
                    break;
              
                case Item.Type.Heart:
                    hearts += item.value;
                    if (hearts > max_heart)
                        hearts = max_heart;
                    break;



            }


            Destroy(other.gameObject);
        }
        else if (other.tag == "EnemyBullet")
        {
            if (!isDamage)
            {
                if (other.gameObject != null) //데미지 입기 전 적이 죽었을 때 오류 방지
                {
                    Bullet bullet = other.GetComponent<Bullet>();
                    hearts -= bullet.damage;
                }

                bool isBossMelee=false;
                if (other.name == "Boss Melee Area")
                {
                    isBossMelee = true;
                }
                StartCoroutine(OnDamage(isBossMelee));
            }
            if (other.GetComponent<Rigidbody>() != null) //projectile only
                Destroy(other.gameObject);


        }
    }

    IEnumerator OnDamage(bool isBossMelee)
    {
        isDamage = true;
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }
        if (isBossMelee)
        {
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);
        }

        if (hearts <= 0 && !isDead)
            OnDie();
        yield return new WaitForSeconds(1.0f);

        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
        if(isBossMelee)
            rigid.velocity = Vector3.zero;
        isDamage = false;
        
    }

    void OnDie()
    {
        anim.SetTrigger("DoDie");
        isDead = true;
        gameManager.GameOver();
    }
}
