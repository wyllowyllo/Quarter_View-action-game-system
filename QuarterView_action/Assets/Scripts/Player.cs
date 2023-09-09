using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float h_input;
    float v_input;
    bool isWalk;
    Vector3 movVec;
    public float speed;
    Animator anim;



    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        h_input = Input.GetAxisRaw("Horizontal");
        v_input = Input.GetAxisRaw("Vertical");
        isWalk = Input.GetButton("Walk");
        movVec = new Vector3(h_input, 0, v_input);

        transform.position += movVec * speed *((isWalk)?0.3f:1f)*Time.deltaTime;


        anim.SetBool("IsRun", movVec != Vector3.zero);
        anim.SetBool("IsWalk", isWalk);


        //Rotate a player to the moving direction
        transform.LookAt(transform.position + movVec);
    }
}
