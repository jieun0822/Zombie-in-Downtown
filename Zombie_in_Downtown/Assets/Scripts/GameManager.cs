using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameCam;
    public GameObject menuCam;

    public GameObject playerObject;
    public Player player;

    public Enemy boss;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;
    public GameObject parkZone;
    public GameObject resetPos;

    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int bossCnt;

    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
    public GameObject manualPanel;
    public Text maxScoreTxt;
    public Text scoreTxt;
    public Text stageTxt;
    public Text playTimeTxt;
    public Text playerHealthTxt;
    public Text playerAmmoTxt;
    public Text playerGranadeTxt;
    public Text playerCoinTxt;
    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weapon4Img;
    public Text enemyTxt;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;
    public GameObject lightGroup;
    public Text curScoreText;
    public Text bestText;
    public GameObject npcGroup;

    public GameObject startButton;
    public GameObject NameGroup;
    public InputField nameInput;
    private string _userName;
    public string userName
    {
        get
        {
            if (_userName == string.Empty)
                _userName = PlayerPrefs.GetString("playerName", "Player");
            return _userName;
        }
        set
        {
           
            _userName = value;
            PlayerPrefs.SetString("playerName", _userName);
        }
    }

    bool firstStageStart = true;//처음에만 울음소리 한번 들리게.
    bool isMenual;
    //public VideoPlayer introVideo;

    public AudioSource menuSound;
    public AudioSource stageSound;
    public AudioSource idleSound_zombie;
    public AudioSource laughSound_boss;
    public AudioSource overSound;
    public AudioSource townSound;

    void Awake()
    {
        if (playerObject == null)
        {
            playerObject = GameObject.FindWithTag("Player");
            player = playerObject.GetComponent<Player>();
            playerObject.SetActive(true);
            Rigidbody rigid = playerObject.GetComponent<Rigidbody>();
            //rigid.FreezePosition = false;
        }

        enemyList = new List<int>();
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

        if (PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore", 0);
        PlayerPrefs.SetInt("Stage", 0);
        PlayerPrefs.SetString("playerName", "Player");

        //introVideo.Play();

        //if (SaveSystem.isFileExist())
        //{
        //    LoadPlayer();
        //}

        //DontDestroyOnLoad(gameObject);
    }
    /*
    public void SaveScene()
    {
        int activeScene = SceneManager.GetActiveScene().buildIndex;

        PlayerPrefs.SetInt("ActiveScene", activeScene);
    }

    public void LoadScene()
    {
        int activeScene = PlayerPrefs.GetInt("ActiveScene");

        //SceneManager.LoadScene(activeScene);

        //Note: In most cases, to avoid pauses or performance hiccups while loading,
        //you should use the asynchronous version of the LoadScene() command which is: LoadSceneAsync()

        //Loads the Scene asynchronously in the background
        StartCoroutine(LoadNewScene(activeScene));
    }

    IEnumerator LoadNewScene(int sceneBuildIndex)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex);
        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress < 0.9f)
        {
            yield return null;
        }

        asyncOperation.allowSceneActivation = true;
    }
    */
    
    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void BackScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        player.name = data.name;
        player.score = data.score;
        stage = data.stage;

        player.ammo = data.ammo;
        player.coin = data.coin;
        player.health = data.health;
        player.hasGranades = data.hasGranades;

        player.hasWeapons[0] = data.hasWeapons[0];
        player.hasWeapons[1] = data.hasWeapons[1];
        player.hasWeapons[2] = data.hasWeapons[2];
    }

    public void GameStart()
    {
        menuSound.Stop();
        townSound.Play();

        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        //if (townSound.isPlaying) townSound.Stop();
        //if (stageSound.isPlaying) stageSound.Stop();
        //overSound.Play();

        //gamePanel.SetActive(false);
        //overPanel.SetActive(true);
        //curScoreText.text = scoreTxt.text;

        //int maxScore = PlayerPrefs.GetInt("MaxScore");
        //if (player.score > maxScore)
        //{
        //    bestText.gameObject.SetActive(true);
        //    PlayerPrefs.SetInt("MaxScore", player.score);
        //}
    }

    public void Restart()
    {
        //if(overSound.isPlaying) overSound.Stop();
        Destroy(playerObject);
        SceneManager.LoadScene(3);
    }

    public void StageStart()
    {
        RenderSettings.fogDensity = 0.03f;

        if (townSound.isPlaying) townSound.Stop();
        stageSound.Play();

        if (player.stage % 5 == 0) StartCoroutine(bossSound());
        else StartCoroutine(zombieSound());

        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);
        parkZone.SetActive(false);
        npcGroup.SetActive(false);
        lightGroup.SetActive(false);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(true);

        isBattle = true;
        StartCoroutine(InBattle());
    }

    IEnumerator zombieSound()
    {
        yield return new WaitForSeconds(1f);
        if (firstStageStart) idleSound_zombie.Play();
        firstStageStart = false;
    }

    IEnumerator bossSound()
    {
        yield return new WaitForSeconds(1f);
        if (firstStageStart) laughSound_boss.Play();
        firstStageStart = false;
    }

    public void StageEnd()
    {
        RenderSettings.fogDensity = 0.013f;

        townSound.Play();
        stageSound.Stop();
        if (idleSound_zombie.isPlaying) idleSound_zombie.Stop();
         firstStageStart = true;

        player.transform.position = resetPos.transform.position;

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);
        parkZone.SetActive(true);
        npcGroup.SetActive(true);
        lightGroup.SetActive(true);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        isBattle = false;
        player.stage++;
    }

    IEnumerator InBattle()
    {
        if (player.stage % 5 == 0)
        {
            bossCnt++;
            int ranZone = Random.Range(0, 4);
            GameObject instantEnemy = Instantiate(enemies[3], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.setPlayer(player);
            enemy.target = player.transform;
            enemy.manager = this;
            boss = instantEnemy.GetComponent<Enemy>();
        }
        else
        {
            for (int index = 0; index < player.stage; index++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);

                switch (ran)
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }
            }

            while (enemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4);
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.setPlayer(player);
                enemy.target = player.transform;
                enemy.manager = this;
                enemyList.RemoveAt(0);
                yield return new WaitForSeconds(4f);
            }
        }

        while (enemyCntA + enemyCntB + enemyCntC +bossCnt > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(4f);

        boss = null;
        StageEnd();
    }

    void Update()
    {
        if (isBattle)
            player.playTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.F1))
            isMenual = !isMenual;

        if (isMenual) manualPanel.SetActive(true);
        else manualPanel.SetActive(false);
    }

    void LateUpdate()
    { 
        //상단 UI
        scoreTxt.text = string.Format("{0:n0}",player.score);
        stageTxt.text = "STAGE " + player.stage;

        int hour = (int)(player.playTime / 3600);
        int min = (int)((player.playTime - hour * 3600) / 60);
        int second = (int)(player.playTime % 60);
        playTimeTxt.text = string.Format("{0:00}", hour) + ":" 
            + string.Format("{0:00}", min) + ":" 
            + string.Format("{0:00}", second);

        //플레이어 UI
        playerHealthTxt.text = player.health + " / " + player.maxHealth;
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);

        if (player.equipWeapon == null)
            playerAmmoTxt.text = "- / " + player.ammo;
        else if (player.equipWeapon.type == Weapon.Type.Melee)
            playerAmmoTxt.text = "- / " + player.ammo;
        else
            playerAmmoTxt.text = player.equipWeapon.curAmmo + " / " + player.ammo;

        if (player.hasGranades > 0)
            playerGranadeTxt.text = player.hasGranades + " / " + player.maxHasGranades;
        else
            playerGranadeTxt.text = "- / " + player.maxHasGranades;
       
        //무기 UI
        weapon1Img.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weapon4Img.color = new Color(1, 1, 1, player.hasGranades>0 ? 1 : 0);
        
        //몬스터 UI
        enemyTxt.text = "x "+ (enemyCntA + enemyCntB + enemyCntC).ToString();

        //보스 체력 UI
        if (boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
        }
        else 
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200;
        }

        //GameOver
        curScoreText.text = scoreTxt.text;
    }

    public void SetUserName()
    {
        userName = nameInput.text;
    }

    public void StartButton_active()
    {
        NameGroup.SetActive(false);
        startButton.SetActive(true);

        player.SetPlayerName();
    }

    public void gotoTown()
    {
        SceneManager.LoadScene(1);
    }
}

