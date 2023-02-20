using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
public class Main : MonoBehaviour
{
    [SerializeField] private Button StartLogin;

    [SerializeField] private Button StartSignUp;

    [SerializeField] private Button Start_Login_Guest;

    [SerializeField] private GameObject SignUpPanel;

    [SerializeField] private GameObject LoginPanel;

    [SerializeField] private GameObject MainPanel;

    // Start is called before the first frame update
    void Start()
    {

        MainPanel.SetActive(true);
        SignUpPanel.SetActive(false);
        LoginPanel.SetActive(false);
        StartLogin.onClick.AddListener(Click_Login);
        StartSignUp.onClick.AddListener(Click_SignUp);
        Start_Login_Guest.onClick.AddListener(Click_DirectEntry);

    }
    public void Click_Login()
    {
        // ActivateButton(false);
        MainPanel.SetActive(false);
        SignUpPanel.SetActive(false);
        if (LoginPanel != null)
        {
            LoginPanel.SetActive(true);
        }
    }

    public void Click_SignUp()
    {
        MainPanel.SetActive(false);
        // ActivateButton(false);
        LoginPanel.SetActive(false);
        if (SignUpPanel != null)
        {
            SignUpPanel.SetActive(true);
        }
        
    }

    public void Click_DirectEntry()
    {
        // isGuest = true;
        
        // SceneManager.LoadScene("001",LoadSceneMode.Single);
    }

    // private void ActivateButton(bool toggle)
    // {
    //     StartLogin.interactable = toggle;
    //     StartSignUp.interactable = toggle;
    // }
}
