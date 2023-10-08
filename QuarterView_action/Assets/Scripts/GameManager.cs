using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject menuCamera;
    public GameObject gameCamera;
    public Player player;
    public Boss boss;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyNumA;
    public int enemyNumB;
    public int enemyNumC;
    public int enemyNumD;

    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject stagePortal;
    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList;


    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;

    public Text maxScoreText;

    public Text scoreText;
    public Text playTimeText;
    public Text stageText;
    public Text playerHealthText;
    public Text playerAmmoText;
    public Text playerCoinText;
    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weaponRImg;
    public Text enemyAText;
    public Text enemyBText;
    public Text enemyCText;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;

    public Text bestText;
    public Text curScoreText;

    private void Awake()
    {
        maxScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
        enemyList = new List<int>();

        if (!PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore", 0);
        
    }

    public void GameStart() // it's called when the start_button (which is in the menuPaenl) is pushed.
    {
        menuCamera.SetActive(false);
        gameCamera.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        player.gameObject.SetActive(true);
        
    }

    public void GameOver()
    {
        curScoreText.text = scoreText.text;
        gamePanel.SetActive(false);
        overPanel.SetActive(true);

        int max = PlayerPrefs.GetInt("MaxScore");
        if (max < player.score)
        {
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }

       
    }

    public void ReStart()
    {
        SceneManager.LoadScene(0);
    }
    public void StageStart()
    {
        isBattle = true;
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        stagePortal.SetActive(false);

        foreach(Transform zone in enemyZones)
            zone.gameObject.SetActive(true);

        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {
        player.transform.position = Vector3.up * 0.8f;

        isBattle = false;
        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        stagePortal.SetActive(true);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        stage++;
    }

    IEnumerator InBattle()
    {
        if (stage % 5 == 0)
        {
            enemyNumD++;
            GameObject bossObj = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            boss = bossObj.GetComponent<Boss>();
            boss.target = player.transform;
            boss.gamaManager = this;
        }
        else
        {
            for (int index = 0; index < stage; index++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);

                switch (ran)
                {
                    case 0:
                        enemyNumA++;
                        break;
                    case 1:
                        enemyNumB++;
                        break;
                    case 2:
                        enemyNumC++;
                        break;
                }
            }

            while (enemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4);
                GameObject enemyObj = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);

                Enemy enemyLogic = enemyObj.GetComponent<Enemy>();
                enemyLogic.target = player.transform;
                enemyLogic.gamaManager = this;
                enemyList.RemoveAt(0);
                yield return new WaitForSeconds(4.0f);
            }
        }

        while (enemyNumA + enemyNumB + enemyNumC + enemyNumD > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(4.0f);
        StageEnd();
        
    }

    private void Update()
    {
        if (isBattle)
            playTime += Time.deltaTime;
    }

    private void LateUpdate() //it's called after the call of Update func
    {
        //Stage UI(Stage, Time, Score)
        scoreText.text = string.Format("{0:n0}", player.score);
        stageText.text = "STAGE "+stage.ToString();

        int hour =(int) (playTime / 3600);
        int minute = (int)((playTime - (hour * 3600)) / 60);
        int sec = (int)(playTime % 60);
        playTimeText.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", minute) + ":" + string.Format("{0:00}", sec) + ":";

        //State UI(health, ammo, coin)
        playerHealthText.text = player.hearts + " / " + player.max_heart;
        playerCoinText.text = string.Format("{0:n0}", player.coin); 
        if (player.equipWeapon == null)
            playerAmmoText.text = "- / " + player.ammo;
        else if (player.equipWeapon.type == Weapon.Type.Melee)
            playerAmmoText.text = "- / " + player.ammo;
        else
            playerAmmoText.text = player.equipWeapon.curAmmo + " / " + player.ammo;

        //Weapon UI
        weapon1Img.color = new Color(1, 1, 1, player.gotWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.gotWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.gotWeapons[2] ? 1 : 0);
        weaponRImg.color = new Color(1, 1, 1, player.hasGrenade>0 ? 1 : 0);

        //Enemy num UI
        enemyAText.text = "x "+enemyNumA.ToString();
        enemyBText.text = "x " + enemyNumB.ToString();
        enemyCText.text = "x " + enemyNumC.ToString();

        //Boss Health bar UI
        if (boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * -30;
            bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 300;
        }
    }
}
