using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class IOManager : MonoBehaviour
{
    string Str;
    Vector2 v2;
    bool IsShow;

    //public RawImage img;

    void Start()
    {
        IsShow = false;
        Str = "";
        v2 = Vector2.zero;
    }


    void Update()
    {
        //�������˸��ʱ��ʾ�����ؿ���̨
        if (Input.GetKey(KeyCode.Backspace))
            IsShow = !IsShow;
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            DateTime NowTime = DateTime.Now.ToLocalTime();
            //ScreenCapture.CaptureScreenshot("E:/A_unity/" + NowTime.ToString("yyyy-MM-dd HH-mm-ss") + ".jpg");
            ScreenCapture.CaptureScreenshot(Application.dataPath + "/Resources/Screenshot_"+ NowTime.ToString("yyyy_MM_dd_HH_mm_ss") + ".jpg");
            //img.texture = Resources.Load<Texture>("Screenshot");
        }
    }


    //���ű�����ʱע�����̨��Ϣ�����ί��
    void OnEnable()
    {
        Application.logMessageReceived += Application_logMessageReceived;
    }


    //���ű�����ʱȡ������̨��Ϣ�����ί��
    void OnDisable()
    {
        Application.logMessageReceived -= Application_logMessageReceived;
    }


    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        //�������̨����Ϣ
        Str += condition + "\n" + stackTrace + "\n---------------------------------------------------------------\n";
    }


    void OnGUI()
    {
        //���ƿ���̨����
        if (IsShow)
        {
            v2 = GUILayout.BeginScrollView(v2, GUILayout.MinWidth(Screen.width - 5), GUILayout.MaxHeight(400));
            GUILayout.TextArea(Str, GUILayout.MinWidth(Screen.width - 100));
            GUILayout.EndScrollView();
        }
    }
}
