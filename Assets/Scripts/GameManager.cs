using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /* 状態・変数 */
    // util
    public static GameManager instance;
    public DBManager dbmanager;
    public Transform spawnPoint;
    public float maxSpawnPointX;
    public GameObject plane;
    

    // startMenu
    public GameObject startMenuPanel;
    public Text highScoreText;
    public Text playerNameText;
    public GameObject playerNameInputPanel;
    public InputField playerNameInputField;
    public Button confirmNewNameButton;
    public GameObject cannotUseThatNameText;

    // game
    public GameObject enemy;
    public GameObject notEnemy;
    public GameObject gamePanel;
    public Text scoreText;
    public Text levelText;
    public Text levelUpText;

    // endMenu
    public GameObject endMenuPanel;
    public Text endScoreText;
    public Text newRecordText;

    // ranking
    public GameObject rankingPanel;

    // data
    bool playingGame;
    bool DisplayingRanking;

    public static Color32 colorObj;
    string playerName = "";
    int score = 0;
    int highScore = 0;
    int level = 1;

    

    /* Awake */
    private void Awake()
    {
        if (instance == null) { instance = this; }
    }


    /* Start */
    void Start()
    {
        startMenuPanel.gameObject.SetActive(true);
        playerNameInputPanel.gameObject.SetActive(false);
        gamePanel.gameObject.SetActive(false);
        endMenuPanel.gameObject.SetActive(false);
        rankingPanel.gameObject.SetActive(false);
        levelUpText.gameObject.SetActive(false);
        newRecordText.gameObject.SetActive(false);
        cannotUseThatNameText.gameObject.SetActive(false);
        //playerNameInputField = playerNameInputField.GetComponent<InputField>();
        if (PlayerPrefs.HasKey("highScore"))
        {
            highScore = PlayerPrefs.GetInt("highScore");
            highScoreText.text = "High Score: " + highScore.ToString();
        }
        else { highScoreText.text = "High Score: 0"; };
        if (PlayerPrefs.HasKey("playerName"))
        {
            playerNameText.text = "Name: " + PlayerPrefs.GetString("playerName");

        }
        else{ playerNameInputPanel.gameObject.SetActive(true); }
    }


    /* プレイヤー名テキスト押下 */
    public void NameEdit()
    {
        playerNameInputPanel.gameObject.SetActive(true);
    }

    /* プレイヤー名選択中 */
    public void InputText()
    {
        dbmanager.PlayerNameIsOK(playerNameInputField.text);
    }

    /* プレイヤ名確定ボタン押下 */
    public void ConfirmNewName()
    {
        PlayerPrefs.SetString("playerName", playerNameInputField.text);
        playerNameText.text = "name: " + PlayerPrefs.GetString("playerName");
        PlayerPrefs.SetInt("highScore", 0);
        SceneManager.LoadScene(0);
    }

    /* ゲームスタートボタン */
    public void GameStart()
    {
        if (!playingGame && !DisplayingRanking)
        {
            startMenuPanel.gameObject.SetActive(false);
            gamePanel.gameObject.SetActive(true);
            scoreText.text = "score: " + score.ToString();
            levelText.text = "level: " + level.ToString();
            playingGame = true;
            StartCoroutine("SpawnEnemies");

        }
    }


    /* 壁生成コルーチン */
    IEnumerator SpawnEnemies()
    {
        while (playingGame)
        {
            yield return new WaitForSeconds(2.0f);
            // 速度変更
            SpeedChange();
            // 壁着色
            ColorChange();
            // 壁発生
            Spawn();
        }
    }


    /* 速度変更 */
    public void SpeedChange()
    {
        // スピード取得
        float speed = DifficultyManage(level)[1];
        Debug.Log(speed);

        // スピードの付与
        enemy.GetComponent<Enemy>().speed = speed;
        notEnemy.GetComponent<NotEnemy>().speed = speed;
    }


    /* 壁着色 */
    public void ColorChange()
    {
        // 色彩変化度数取得
        int colorDiff = DifficultyManage(level)[0];
        Debug.Log(colorDiff);

        // 4壁の着色
        colorObj = new Color32((byte)Random.Range(5, 250),
                                   (byte)Random.Range(5, 250),
                                   (byte)Random.Range(5, 250), 1);
        enemy.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", colorObj);

        // 1壁の着色(4壁に近い色を着色)
        int rgbChoice = Random.Range(0, 3);
        if (rgbChoice == 0)
        {
            if (colorObj.r > 127) { colorObj.r = (byte)(colorObj.r - colorDiff); }
            else { colorObj.r = (byte)(colorObj.r + colorDiff); }
        }
        else if (rgbChoice == 1)
        {
            if (colorObj.g > 127) { colorObj.g = (byte)(colorObj.g - colorDiff); }
            else { colorObj.g = (byte)(colorObj.g + colorDiff); }
        }
        else
        {
            if (colorObj.b > 127) { colorObj.b = (byte)(colorObj.b - colorDiff); }
            else { colorObj.b = (byte)(colorObj.b + colorDiff); }
        }
        notEnemy.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", colorObj);
    }


    /* 壁発生 */
    public void Spawn()
    {

        float[] posArray = new float[5] { (float)-2, (float)-1, (float)0, (float)1, (float)2 };
        float[] posArrayRandom = posArray.OrderBy(i => System.Guid.NewGuid()).ToArray();

        for (int i = 0; i < posArrayRandom.Length; i++)
        {
            if (i == 0)
            {
                Vector3 notEnemySpawnPos = spawnPoint.position;
                notEnemySpawnPos.x = posArrayRandom[i];
                Instantiate(notEnemy, notEnemySpawnPos, Quaternion.identity);
            }
            else
            {
                Vector3 enemySpawnPos = spawnPoint.position;
                enemySpawnPos.x = posArrayRandom[i];
                Instantiate(enemy, enemySpawnPos, Quaternion.identity);
            }
        }
    }


    /* スコアアップ */
    public void ScoreUp()
    {
        score = score + 1;
        scoreText.text = "score: " + score.ToString();

    }


    /* EndMenu */
    public void DisplayEndMenu()
    {
        playingGame = false;
        DisplayingRanking = true;
        gamePanel.gameObject.SetActive(false);
        endMenuPanel.gameObject.SetActive(true);
        endScoreText.text = "Your score: " + score.ToString();
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("highScore", highScore);
            dbmanager.SaveHighScore(PlayerPrefs.GetString("playerName"), highScore);
            newRecordText.gameObject.SetActive(true);
        }
    }


    /* EndMenuでRESTARTを選択したとき */
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }


    /* EndMenuでRANKINGを選択したとき */
    public void DisplayRanking()
    {
        plane.gameObject.SetActive(false);
        startMenuPanel.gameObject.SetActive(false);
        endMenuPanel.gameObject.SetActive(false);
        rankingPanel.gameObject.SetActive(true);
        dbmanager.LoadTop30Score();
        
    }


    /* レベル管理 */
    public void LevelManage()
    {
        if (score % 25 == 0 && score != 0)
        {
            level++;
            levelText.text = "level: " + level.ToString();
            levelUpText.gameObject.SetActive(true);
        }
        else
        {
            levelUpText.gameObject.SetActive(false);
        }
    }


    /* 難易度管理 */
    public int[] DifficultyManage(int level)
    {
        int colorDiff;
        int speed;

        if (level <= 5)
        {
            colorDiff = 85 - (level * 5);
            speed = -24;
        }
        else if (5 < level && level <= 10)
        {
            colorDiff = 60 - ((level - 5) * 4);
            speed = -26;
        }
        else if (10 < level && level <= 15)
        {
            colorDiff = 40 - ((level - 10) * 3);
            speed = -28;
        }
        else if (15 < level && level <= 20)
        {
            colorDiff = 25 - ((level - 15) * 2);
            speed = -30;
        }
        else if (20 < level && level <= 25)
        {
            colorDiff = 15 - ((level - 20) * 1);
            speed = -32;
        }
        else if (25 < level && level <= 30)
        {
            colorDiff = 10;
            speed = -34;
        }
        else
        {
            colorDiff = 8;
            speed = -36;
        }

        return new int[2] { colorDiff, speed };
    }
}

