using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.Networking;
public class SignUp : MonoBehaviour
{

	public GameObject SignUpPanel;
	public GameObject LoginPanel;
	
	public GameObject MainPanel;

	private Button buttonSignup;
	private Button buttonLogin;

	private Button buttonClose;

	private TextMeshProUGUI alertText;
	private static string URL_REGISTER = "http://139.155.28.154:26000/dynamic/user/register";
	
	

    // Start is called before the first frame update
    void Start()
    {
	    // alertText = transform.Find("Popup_TitleBar").Find("Alert_text").GetComponent<TextMeshProUGUI>();
	    buttonSignup = GameObject.Find("Button_Blue").GetComponent<Button>();
	    buttonSignup.onClick.AddListener((OnBtnSignUpClick));
	    
	    buttonLogin = GameObject.Find("SignupText").GetComponent<Button>();
	    buttonLogin.onClick.AddListener((OnBtnLoginClick));
	    
	    buttonClose = GameObject.Find("Button_Close_Signup").GetComponent<Button>();
	    buttonClose.onClick.AddListener((OnBtnCloseClick));

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public class RegisterInfo
    {
	    public string name;
	    public string email;
	    
	    public string passwd;
	    public string nickname;
	    
    }

    void OnBtnSignUpClick()
    {
	    // alertText.text = "Signing up..";
	    StartCoroutine(trySignUp());
    }
    
    
    
    private IEnumerator trySignUp()
    {
	    
	    TMP_InputField inputEmail = GameObject.Find("InputField_Email").GetComponent(typeof(TMP_InputField)) as TMP_InputField;
	    TMP_InputField inputusername = GameObject.Find("InputField_UserName").GetComponent(typeof(TMP_InputField)) as TMP_InputField;
	    TMP_InputField inputNickname = GameObject.Find("InputField_Nickname").GetComponent(typeof(TMP_InputField)) as TMP_InputField;
	    TMP_InputField inputPasswd = GameObject.Find("InputField_Password").GetComponent(typeof(TMP_InputField)) as TMP_InputField;
	    TMP_InputField inputCheckPasswd = GameObject.Find("InputField_CheckPassword").GetComponent(typeof(TMP_InputField)) as TMP_InputField;

	    string email = inputEmail.text;
	    string username = inputusername.text;
	    string nickname = inputNickname.text;
	    string passwd = inputPasswd.text;
	    string checkPasswd = inputCheckPasswd.text;

	    if (CheckConfirm(passwd, checkPasswd))
	    {
		    RegisterInfo registerInfo = new RegisterInfo();
		    registerInfo.name = username;
		    registerInfo.email = email;
		    registerInfo.passwd = passwd;
		    registerInfo.nickname = nickname;

		    string jsonData = JsonUtility.ToJson(registerInfo);
		    Debug.Log("jsonData_register" + jsonData);
		    // Debug.Log("url" + url);
		    // Debug.Log("jsonData" + jsonData);
		    using (UnityWebRequest request = new UnityWebRequest(URL_REGISTER, "POST"))
		    {
			    byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
			    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
			    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
			    request.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
			    yield return request.SendWebRequest();


			    if (request.result == UnityWebRequest.Result.Success)
			    {

				    // alertText.text = "Register Successfully";
				    SignUpPanel.SetActive(false);
				    LoginPanel.SetActive(true);

			    }
			    else
			    {
				    // alertText.text = "Error connecting ...";
				    Debug.LogError(request.error + '\n' + request.downloadHandler.text);
			    }

		    }
	    }
	    else
	    {
		    // alertText.text="The password and confirm password must be same";
	    }
	    

	    yield return null;

    }

  

    bool CheckConfirm(string x, string y)
    {
	    if (x == y)
	    {
		    return true;
	    }
	    else
	    {
		    return false;
	    }
    }
    
    void OnBtnLoginClick()
    {
	    SignUpPanel.SetActive(false);
	    if (LoginPanel !=null) LoginPanel.SetActive(true);

    }

    void OnBtnCloseClick()
    {
	    SignUpPanel.SetActive(false);
	    if (MainPanel != null) MainPanel.SetActive(true);

    }
    
    
    
}
