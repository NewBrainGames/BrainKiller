using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading;
using Unity.VisualScripting;

public class SceneFadeInOut : MonoBehaviour
{

    public float fadeSpeed = 1f;
    public static bool CoinAndNextImage = false;
    public static bool flag = false;
    //public bool sceneStarting = true;
    
    public static bool changeLight = false;
    public static bool timeRestart = false;

    private Image rawImage;
    private int time = 120;
    //private int distance = 100;
    private GameObject player;
    //private int num = 0;
    //private Vector3 pos;
    private Vector3 doorPos;
    private float m_timer = 0;
    //private Transform di;
   
    private float timer = 0;
    private bool executed = false;
    public SwcGenerator_Practice SwcPrac;
    public BattleManager bm;
    private bool posted = false;
    private bool startChange = false;
    private TimeCounter tc;

    private ChangeSkyBox csb;

    void Awake()
    {
        rawImage = GetComponent<Image>();
    }

    void Start()
    {
        player = GameObject.Find("Player");
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        doorPos = new Vector3(26.46f, 1.25f, 3.89f);
        csb = GameObject.Find("Main Camera").GetComponent<ChangeSkyBox>();
        tc = GameObject.Find("Canvas").GetComponent<TimeCounter>();
    }

    void Update()
    {

        time = TimeCounter.totalTime;//这是静态的 可以全局访问
        if ((player.transform.position.x > 66f && player.transform.position.x < 70f && player.transform.position.z > 61f && player.transform.position.z < 80f && DoorAnim.doorAni == true) || time == 0)
        {
            changeToNextScene();
        }
        else
        {
            StartScene();
        }

        if (startChange)
        {
            changeToNextScene();
        }

        if (time <= 3)
        {
            startChange = true;
            tc.setTime(600);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            if (GameConfig.Instance && GameConfig.Instance.GetGameMode() == Mode.Axon)
            {
                startChange = true;
                // tc.setTime(600);
            }
        }
    }
    
    

    private void changeToNextScene()
    {
        if (!posted)
        {
            bm.PostAnnotation();
            posted = true;
        }
        EndScene();
        //调用接口加载下一个场景，并且将玩家位置初始化到门
        // reset time
        tc.setTime(600);
        if (executed == false)
        {
            timer += Time.deltaTime;
            if (timer >= 1)
            {
                //gm.CreateCoin(pdoor.transform.position);
                changeLight = true;
                csb.setSkyBox(); 
                ChuShiHua();
                timer = 0;

            }
        } 
        
        
    }

    private void FadeToClear()
    {
        rawImage.color = Color.Lerp(rawImage.color, Color.clear, fadeSpeed * Time.deltaTime);
    }

    private void FadeToBlack()
    {
        rawImage.color = Color.Lerp(rawImage.color, Color.black, fadeSpeed * Time.deltaTime);
    }

    void StartScene()
    {
        FadeToClear();
    }

    void EndScene()
    {
        rawImage.enabled = true;
        FadeToBlack();


    }

    void ChuShiHua()
    {
        CoinAndNextImage = true;
        m_timer += Time.time;
        if (m_timer >= 2)
        {
            Debug.Log("ChuShiHua");
            GameObject swcGenerator = GameObject.Find("SwcGenerator");
            SwcPrac = swcGenerator.GetComponent<SwcGenerator_Practice>();
            if (!SwcPrac.isSwcShow())
            {
                SwcPrac.toggleRAWSwc();
            }
            SwcPrac.nextImage();
            Invoke("enablePost", 3.0f);
            
            m_timer = 0;
            Vector3 pos1 = player.transform.position;
            pos1.x = 67.59f;
            pos1.y = 1.28f;
            pos1.z = 32.78f;
            player.transform.position = pos1;
            DoorAnim.doorAni = false;
        }

        startChange = false;
    }

    private void enablePost()
    {
        posted = false;
    }


    void backToMenu()
    {
        EndScene();
        Application.Quit();
    }

}




