using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public Scene scene;
    public GameObject Player;
    public GameObject Weapon_Root;
    public GameObject Enemy;
    public GameObject enemyReword;
    private float PlayerHP;
    private float MaxHP;
    private float bulletCount;
    public GameObject winUI;
    public GameObject gameOverUI;
    public GameObject gameIntroductionUI;
    public GameObject pauseUI;
    public GameObject centerPointUI;
    public GameObject coinStatisticsUI;
    public GameObject bagUI;
    public GameObject optionMenuUI;
    public Sprite exampleImage;
    public GameObject itemPicture;
    private BattleManager bm;
    public Text times;
    public Text scores;
    public bool firstCure = true;
    // Start is called before the first frame update
    void Start()
    {
        scene = SceneManager.GetActiveScene();
        Player = GameObject.Find("Player");
        Weapon_Root = GameObject.Find("Weapon_Root");
        if (scene.name == "001")
        {
            winUI = GameObject.Find("WinUI");
            winUI.SetActive(false);
            gameOverUI = GameObject.Find("GameOverUI");
            gameOverUI.SetActive(false);
        }
        gameIntroductionUI = GameObject.Find("GameIntroductionUI");
        gameIntroductionUI.SetActive(false);
        bagUI = GameObject.Find("Bag");
        itemPicture = GameObject.Find("ItemPicture");
        exampleImage = itemPicture.GetComponent<Image>().sprite;
        bagUI.SetActive(false);
        pauseUI = GameObject.Find("PauseUI");
        pauseUI.SetActive(false);
        coinStatisticsUI = GameObject.Find("CoinStatistics");
        centerPointUI = GameObject.Find("CenterPoint");
        optionMenuUI = GameObject.Find("OptionsMenu");
        optionMenuUI.SetActive(false);
        PlayerHP = Player.GetComponent<AttributeController>().GetHP();
        MaxHP = PlayerHP;
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        //bagUI.SetActive(true);
        //bagUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        Enemy = GameObject.Find("Enemy_02");
        PlayerHP = Player.GetComponent<AttributeController>().GetHP();
        TabClicked();
        firstCure = GameObject.Find("BattleManager").GetComponent<BattleManager>().firstCure;
        if (scene.name == "001" && firstCure == false)
        {
            Invoke("WinUI", 6);
        }
        if (scene.name == "001" )
        {
            GameOverUI();
            scores.text = (bm.getScore() * 10).ToString();
            times.text = GameObject.Find("Time").GetComponent<TextMeshProUGUI>().text;
            int n = times.text.IndexOf('s');
            int residuetime = int.Parse(times.text.Substring(0,n));
            times.text = (150 - residuetime).ToString()+"S";
        }       
        CKeyClicked();
        ESCKeyClicked();
        if (optionMenuUI.activeSelf == true || pauseUI.activeSelf == true)
        {
            centerPointUI.SetActive(false);
        }
        else
        {
            centerPointUI.SetActive(true);
        }
    }
//GameIntroductionUI Controller

    private void TabClicked()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            gameIntroductionUI.SetActive(!gameIntroductionUI.activeSelf);
            coinStatisticsUI.SetActive(!coinStatisticsUI.activeSelf);
        }
    }
//WinUI Controller
    private void WinUI()
    {
        if (Enemy != null)
        {
            Vector3 enemyPosition = GameObject.Find("Enemy_02").transform.position;
            enemyReword.GetComponent<Transform>().position = enemyPosition;
        }
        else 
        {
            enemyReword.SetActive(true);
        }
        if(Enemy == null&&GameObject.Find("Coin") == null && GameObject.Find("Coin1") == null && GameObject.Find("Coin2") == null)
        {
            Weapon_Root.GetComponent<Weapon_Controller>().AllowFire();
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            gameIntroductionUI.SetActive(false);
            coinStatisticsUI.SetActive(false);
            centerPointUI.SetActive(false);
            winUI.SetActive(true);
        }
       
    }
// GameOverUI Controller
    private void GameOverUI()
    {
        if (PlayerHP <= 0)
        {
            Weapon_Root.GetComponent<Weapon_Controller>().AllowFire();
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            gameIntroductionUI.SetActive(false);
            coinStatisticsUI.SetActive(false);
            centerPointUI.SetActive(false);
            gameOverUI.SetActive(true);
        }
    }
// GameOverUI AgainBtn Controller
    public void AgainBtnClicked()
    {
        PlayerHP = MaxHP;
        SceneFadeInOut.timeRestart = true;
        if (scene.name == "001")
        {
            SceneManager.LoadScene("001");
        }
        if (scene.name == "Practice")
        {
            SceneManager.LoadScene("Practice");
        }
        Time.timeScale = 1f;
        Weapon_Root.GetComponent<Weapon_Controller>().AllowFire();



    }
// GameOverUI QuitBtn Controller
    public void QuitBtnClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main");
        Weapon_Root.GetComponent<Weapon_Controller>().AllowFire();

    }
// BagUI Controller
    private void CKeyClicked()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Weapon_Root.GetComponent<Weapon_Controller>().AllowFire();
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

           // Weapon_Root.GetComponent<Weapon_Controller>().AllowFire();
            //if (bagUI.activeSelf == false)
            //{
            //    bulletCount = GameObject.Find("Weapon_Root").GetComponent<Weapon_Controller>().currentCount;
            //}
            //GameObject.Find("Weapon_Root").GetComponent<Weapon_Controller>().currentCount = 0;
            bagUI.SetActive(!bagUI.activeSelf);
            centerPointUI.SetActive(!centerPointUI.activeSelf);
            if(bagUI.activeSelf == false)
            {
                Time.timeScale = 1f;
                //GameObject.Find("Weapon_Root").GetComponent<Weapon_Controller>().currentCount = bulletCount;
                Cursor.visible = false;//鼠标隐藏
                Cursor.lockState = CursorLockMode.Locked;
                itemPicture.GetComponent<Image>().sprite = exampleImage;
            }
        }
    }
// BagUI CloseBtn Controller 
    public void CloseBtnClicked()
    {
        bagUI.SetActive(!bagUI.activeSelf);
        centerPointUI.SetActive(!centerPointUI.activeSelf);
        Time.timeScale = 1f;
        Weapon_Root.GetComponent<Weapon_Controller>().AllowFire();
       // GameObject.Find("Weapon_Root").GetComponent<Weapon_Controller>().currentCount = bulletCount;
        Cursor.visible = false;//鼠标隐藏
        Cursor.lockState = CursorLockMode.Locked;
    }
// PauseUI Controller
    private void ESCKeyClicked()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionMenuUI.activeSelf == true)
            {
                Weapon_Root.GetComponent<Weapon_Controller>().AllowFire();
                optionMenuUI.SetActive(false);
                Time.timeScale = 1f;
                Cursor.visible = false;//鼠标隐藏
                Cursor.lockState = CursorLockMode.Locked;
                return;
            }
            Weapon_Root.GetComponent<Weapon_Controller>().AllowFire();
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (bagUI.activeSelf == true)
            {
                bagUI.SetActive(false);
                Weapon_Root.GetComponent<Weapon_Controller>().AllowFire();
            }
            pauseUI.SetActive(!pauseUI.activeSelf);
            if (pauseUI.activeSelf == false)
            {
                Time.timeScale = 1f;
                //GameObject.Find("Weapon_Root").GetComponent<Weapon_Controller>().currentCount = bulletCount;
                Cursor.visible = false;//鼠标隐藏
                Cursor.lockState = CursorLockMode.Locked;
               
            }
        }
    }
// PauseUI  Continue Controller
    public void CotinueClicked()
    {
        Weapon_Root.GetComponent<Weapon_Controller>().AllowFire();
        //centerPointUI.SetActive(!centerPointUI.activeSelf);
        //coinStatisticsUI.SetActive(!coinStatisticsUI.activeSelf);
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;//�������
        Cursor.lockState = CursorLockMode.Locked;
    }
// PauseUI  Setting Controller
    public void SettingClicked()
    {
        pauseUI.SetActive(false);
        optionMenuUI.SetActive(true);
    }
// OptionMenuUI  BackBtn Controller
    public void BackClicked()
    {
        pauseUI.SetActive(true);
        optionMenuUI.SetActive(false);
    }
}
