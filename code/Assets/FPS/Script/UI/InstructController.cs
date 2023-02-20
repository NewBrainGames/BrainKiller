using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InstructController : MonoBehaviour
{
    public Scene scene;
    public Text text;
    public Button instructBtn;
    private GameObject instructUI;
    public bool isVisible = true;
    public static bool next = true;
    public BattleManager bm;
    public Weapon_Controller wtrl;
    private bool firstEnterGame = true;
    // Start is called before the first frame update
    void Start()
    {
        scene = SceneManager.GetActiveScene();
        bm = GameObject.FindGameObjectWithTag("BM").GetComponent<BattleManager>();
        wtrl = GameObject.Find("Weapon_Root").GetComponent<Weapon_Controller>();
        instructUI = GameObject.FindGameObjectWithTag("InstructText");
        text = GameObject.FindGameObjectWithTag("InstructText").GetComponentInChildren<Text>();
        instructBtn = GameObject.FindGameObjectWithTag("InstructBtn").GetComponent<Button>();
        next = true;
        if( GameConfig.Instance != null && GameConfig.Instance.GetGameMode() == Mode.battle001)
        {
            
            AddText("Welcome to the first level\n" + "You can Press 'W/A/S/D/Space' to move and press 'Left Shift' to accelerate\n" + "Click the left mouse button to shoot and the left mouse button to aim\n" + "Press 'R' to reload bullet\n" + "Now move forward!");
        }    
        else if(GameConfig.Instance == null || GameConfig.Instance.GetGameMode() == Mode.Dendrite || GameConfig.Instance.GetGameMode() == Mode.Axon)
        {
            if (firstEnterGame)
            {
                Mode gameMode = GameConfig.Instance.GetGameMode();
                if (gameMode == Mode.Dendrite)
                {
                    // AddText("Welcome to the Dendrite Shooting Mode.Go ahead and Explore the neurons!\n" + "(Press 'F' to switch on/off the raw image) \n");
                    AddText("欢迎来到 Dendrite 射击模式 \n 按下 WASD 进行移动，按下 F 隐藏或开启原始图像 \n 按下 K 隐藏绿色神经元结构");
                }
                else if(gameMode == Mode.Axon)
                {
                    // AddText("Welcome to the Axon Shooting Mode.Go ahead and Explore the neurons!\n" + "(Press 'F' to switch on/off the raw image)\n(Press 'N' to enter next image block");
                    AddText("欢迎来到 Axon 射击模式 \n 按下 WASD 进行移动，按下 F 隐藏或开启原始图像\n 按下 K 隐藏绿色神经元结构");
                }
                firstEnterGame = false;
            }
           
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void AddText(string content)
    {
        if(next == true)
        {
            wtrl.AllowFire();
            text.gameObject.SetActive(true);
            instructBtn.gameObject.SetActive(true);
            instructUI.SetActive(true);
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            text.text = content;
            next = false;
        }
    }

    public void OnClick()
    {
        if(next == false)
        {
            text.gameObject.SetActive(false);
            instructBtn.gameObject.SetActive(false);
            instructUI.SetActive(false);
            isVisible = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;
            next = true;
            wtrl.AllowFire();
        }
        
    }


}
