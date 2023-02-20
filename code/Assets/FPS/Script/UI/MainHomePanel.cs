using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainHomePanel : MonoBehaviour
{
    public GameObject recordPanel;
    public GameObject panelHome;
    
    public GameObject OptionsPanel;

    private Button buttonRecord;
    private Button buttonPlayDailyMode;
    private Button buttonPlayStoryMode;

    private Button buttonSetting;

    private Button buttonQuit;

    // Start is called before the first frame update
    void Start()
    {
        buttonRecord = GameObject.Find("Button_Record").GetComponent<Button>();
        buttonRecord.onClick.AddListener((OnBtnRecordClick));
        
        buttonPlayDailyMode = GameObject.Find("Button_PlayDaily").GetComponent<Button>();
        buttonPlayDailyMode.onClick.AddListener((OnBtnPlayDailyClick));
        
        buttonPlayStoryMode = GameObject.Find("Button_PlayStory").GetComponent<Button>();
        buttonPlayStoryMode.onClick.AddListener((OnBtnPlayStoryClick));
        
        buttonSetting = GameObject.Find("Button_Top_Setting").GetComponent<Button>();
        buttonSetting.onClick.AddListener((OnBtnSettingClick));
        
        buttonQuit = GameObject.Find("Button_Top_Quitting").GetComponent<Button>();
        buttonQuit.onClick.AddListener((OnBtnQuitClick));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnBtnRecordClick()
    {

        panelHome.SetActive(false);

        if (recordPanel != null)
        {
            recordPanel.SetActive(true);
        }
    }
    
    void OnBtnPlayDailyClick()
    {
        GameConfig.Instance.SelectDendriteMode();
        SceneManager.LoadScene("Practice", LoadSceneMode.Single);
    }
    
    void OnBtnPlayStoryClick()
    {
        GameConfig.Instance.SelectAxonMode();
        SceneManager.LoadScene("Practice", LoadSceneMode.Single);
    }

    void OnBtnSettingClick()
    {
        panelHome.SetActive(false);
        if (OptionsPanel != null)
        {
            OptionsPanel.SetActive(true);
        }
    }

    void OnBtnQuitClick()
    {
        
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();

    }
}
