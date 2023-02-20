using System;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Net;
using System.Net.Mail;
using UnityEngine.SceneManagement;


//using System;

public class Login : MonoBehaviour
{
    
    [SerializeField] private string url = "http://139.155.28.154:26000/dynamic/user/login";
    [SerializeField] private TMP_InputField usernameInputFiled;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TextMeshProUGUI alertText;
    [SerializeField] private Button loginButton;

    [SerializeField] private Button signUpButton;
    
    [SerializeField] private Button closeDialogButton;
    

    [SerializeField] private Button StartLogin;

    [SerializeField] private Button StartSignUp;

    [SerializeField] private Button Start_Login_Guest;
    [SerializeField] private GameObject SignUpPanel;
    [SerializeField] private GameObject LoginPanel;

    [SerializeField] private GameObject MainPanel;
    
    public static bool isGuest = false;


    // void start()
    // {
    //     // MainPanel.SetActive(true);
    //     // LoginPanel.SetActive(false);
    //     // SignUpPanel.SetActive(false);
    //     
    //     // StartLogin= transform.Find("Button_Login__Group").Find("Button_Login").GetComponent<Button>();
    //     // // StartLogin = GameObject.Find("Button_Login").GetComponent<Button>();
    //     // StartLogin.onClick.AddListener((OnBtnStartLogin));
    //     //
    //     // StartSignUp = GameObject.Find("Button_SignUp").GetComponent<Button>();
    //     // StartSignUp.onClick.AddListener((OnBtnStartSignup));
    //     //
    //     // Start_Login_Guest = GameObject.Find("Button_Login_Guest").GetComponent<Button>();
    //     // Start_Login_Guest.onClick.AddListener((OnBtnGuest));
    //     
    // }

    public void OnBtnStartLogin()
    {
        MainPanel.SetActive(false);
        SignUpPanel.SetActive(false);
        if (LoginPanel != null)
        {
            LoginPanel.SetActive(true);
        }
    }
    
    public void OnBtnStartSignup()
    {
        MainPanel.SetActive(false);
        LoginPanel.SetActive(false);
        if (SignUpPanel != null)
        {
            SignUpPanel.SetActive(true);
        }
        
    }
    
    public void OnBtnGuest()
    {

        Debug.Log("Start Guest");
        isGuest = true;
        StartCoroutine(TryLogin());
        
    }

    public void OnLoginClick()
    {
        alertText.text = "Signing in..";
        ActivateButton(false);
        isGuest = false;
        StartCoroutine(TryLogin());
    }

    public void OnSignUpClick()
    {
        
        LoginPanel.SetActive(false);
        SignUpPanel.SetActive(true);
    }

    public void OnCloseDialogClick()
    {
        // ActivateButton(true);
        MainPanel.SetActive(true);
        SignUpPanel.SetActive(false);
        LoginPanel.SetActive(false);
        
    }
    
    
    public class UserInfo 
    {
        public string name;
        public string passwd;

    }



    private IEnumerator TryLogin()
    {
        UserInfo userinfo = new UserInfo();

        if (isGuest == false)
        {
            string username = usernameInputFiled.text;
            string password = passwordInputField.text;

            if (username == null || username.Contains("@"))
            {
                alertText.text = "Invalid username";
                loginButton.interactable = true;
                yield break;
            }

            if (password == null || password.Length < 5)
            {
                alertText.text = "Invalid password";
                loginButton.interactable = true;
                yield break;
            }

            userinfo.name = username;
            userinfo.passwd = password;
            
        }
        else
        {
            Debug.Log("isGuest==true");
            userinfo.name = "zx";
            userinfo.passwd = "123456";
            // isGuest = false;
            
        }

        string jsonData = JsonUtility.ToJson(userinfo);
        string finalData = "{\"user\":" + jsonData + "}";
        Debug.Log(finalData);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(finalData);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                LoggedInUser loggedinuser = JsonUtility.FromJson<LoggedInUser>(request.downloadHandler.text);
                alertText.text = "welcome";
                PlayerPrefs.SetString("username", userinfo.name);
                PlayerPrefs.SetString("password", userinfo.passwd);
                ActivateButton(false);
                SceneManager.LoadScene("Main", LoadSceneMode.Single);
                //if (response.code == 200)
                //{

                //    ActivateButton(false);
                //    //LoggedInUser loggedInUser = JsonUtility.FromJson<LoggdInUser>(request.downloadHandler.text);
                //    //alertText.text = "Welcome" + loggedInUser.nickName;

                //}
                //else
                //{
                //    switch (response.code)
                //    {
                //        case 503:
                //            alertText.text = "Invalid credentials";
                //            ActivateButton(true);
                //            break;
                //        default:
                //            alertText.text = "Corruption detected";
                //            ActivateButton(false);
                //            break;
                //    }

                //}
            }
            else
            {
                alertText.text = "Error connecting ...";
                ActivateButton(true);
            }
        }
 

        //UnityWebRequest request = UnityWebRequest.Post(url, jsonData);

        ////var handler = request.SendWebRequest();
        //yield return request.SendWebRequest();


        //float startTime = 0.0f;
        //while (!handler.isDone)
        //{
        //    startTime += Time.deltaTime;
        //    if (startTime > 100.0f)
        //    {
        //        break;
        //    }
        //    yield return null;
        //}
        
        //Debug.Log($"{username}:{password}");
        yield return null;

    }
    private void ActivateButton(bool toggle)
    {
        loginButton.interactable = toggle;
        signUpButton.interactable = toggle;

    }
    
}
