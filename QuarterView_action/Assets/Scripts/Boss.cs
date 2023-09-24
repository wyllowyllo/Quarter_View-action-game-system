using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;

    Vector3 lookVec;
    Vector3 tauntVec;
    public bool isLook;

    private void Awake()
    {
        base.Awake();
        isLook = true;
        nav.isStopped = true; //일단 실행 x
        StartCoroutine(Think());
    }

    private void Update()
    {
        if (isDead)
        {
            StopAllCoroutines(); //죽었다면, 모든 코루틴 멈춤
            return;
        }
        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            lookVec = new Vector3(h, 0, v) * 5.0f;

            transform.LookAt(target.position + lookVec);
        }
        else
        {
            nav.SetDestination(tauntVec);
        }
    }
    //보스 행동 패턴
    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        int ran = Random.Range(0, 5);
        switch (ran)
        {
            //미사일
            case 0:            
            case 1:
                StartCoroutine(MissileShot());
                break;

            //바위 던지기
            case 2:
            case 3:
                StartCoroutine(RockShot());
                break;
            
            //점프 공격
            case 4:
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");
        

        yield return new WaitForSeconds(0.2f);
        GameObject missileObjA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        BossMissile missileLogicA = missileObjA.GetComponent<BossMissile>();
        missileLogicA.target = target;

        yield return new WaitForSeconds(0.3f);
        GameObject missileObjB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        BossMissile missileLogicB = missileObjB.GetComponent<BossMissile>();
        missileLogicB.target = target;

        yield return new WaitForSeconds(2f);

        StartCoroutine(Think());
    }
    IEnumerator RockShot()
    {
        anim.SetTrigger("doBigShot");
        isLook = false;
        GameObject rockObj = Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);

        isLook = true;
        StartCoroutine(Think());
    }
    IEnumerator Taunt()
    {
        anim.SetTrigger("doTaunt");
        tauntVec = target.position + lookVec;
        isLook = false;
        collid.enabled = false;
        nav.isStopped = false;

        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;


        yield return new WaitForSeconds(1f);
        isLook = true;
        collid.enabled = true;
        nav.isStopped = true;
        StartCoroutine(Think());
    }
}
