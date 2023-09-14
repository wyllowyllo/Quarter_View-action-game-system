using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public enum Type { Melee, Range}
    public Type type;
    public int damage;
    public float rate;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;



    /*
     normal function call : main routine -> sub routine -> main routine
     Co-routine : executing main and sub roution at a time

     */
    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing"); //fuction for stopping a co_routine
            StartCoroutine("Swing"); // function for execuating a co_routine
        }
    }

    //Co-routine
    /*
     IEnumerator ~ 
    { 
    yield return null;  //wait for one frame
                .
                .
                .
    yield return null;  //wait for one frame
                .
                .
                .
    yield return null;  //wait for one frame
    }
     */
    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f); //0.1√  Ω¨±‚
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }
   
   
}
