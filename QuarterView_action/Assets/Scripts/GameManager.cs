using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public GameObject menuPanel;
    public GameObject gamePanel;
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

    private void Awake()
    {
        maxScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore")); 
    }

    public void GameStart()
    {
        menuCamera.SetActive(false);
        gameCamera.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (isBattle)
            playTime += Time.deltaTime;
    }

    private void LateUpdate() //it's called after Update func calling
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
        bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
    }
}
