using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public enum Type { Melee, Range}
    public Type type;
    public int damage;
    public float rate;
    public int curAmmo;
    public int maxAmmo;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;


    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletcasePos;
    public GameObject bulletcase;


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

        else if (type == Type.Range&&curAmmo>0)
        {
            curAmmo--;
            StartCoroutine("Shot"); 
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

    IEnumerator Shot()
    {
        //bullet move
        GameObject bulletInstance = Instantiate(bullet, bulletPos.position, bullet.transform.rotation);
        Rigidbody bulletrigid = bulletInstance.GetComponent<Rigidbody>();
        bulletrigid.velocity = bulletPos.forward * 50;

        yield return null;

        //empty cartridge droppin
        GameObject caseInstance = Instantiate(bulletcase, bulletcasePos.position, bulletcase.transform.rotation);
        Rigidbody caserigid = caseInstance.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletcasePos.forward * Random.Range(-3, -2) + Vector3.up;
        caserigid.AddForce(caseVec,ForceMode.Impulse);
        caserigid.AddTorque(bulletcasePos.up * Random.Range(2, 3), ForceMode.Impulse);
    }


}
