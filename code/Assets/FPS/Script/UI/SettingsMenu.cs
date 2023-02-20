using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingsMenu : MonoBehaviour
{
    
    public GameObject homePanel;
    private Button buttonBack;
    // Start is called before the first frame update
    void Start()
    {
        
        buttonBack = GameObject.Find("BackButton").GetComponent<Button>();
        buttonBack.onClick.AddListener((OnBtnBackClick));
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnBtnBackClick()
    {
        GameObject panelOption = GameObject.Find("OptionsMenu");
        panelOption.SetActive(false);
        if (homePanel != null)
        {
            homePanel.SetActive(true);
        }
    }
}
