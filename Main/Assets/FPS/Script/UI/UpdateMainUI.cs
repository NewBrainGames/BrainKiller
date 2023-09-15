using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpdateMainUI : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject UserExperience;
    private GameObject UserLevel;
    private GameObject UserExpSlider;
    void Start()
    {
        GameObject.Find("TextName").GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("username", "Guest");
        //GameObject.Find("TextLevel").GetComponent<TextMeshProUGUI>().text = "0";
        UserExperience = GameObject.Find("Slider/Text_Value");
        UserLevel = GameObject.Find("UserLevel/TextLevel");
        UserExpSlider = GameObject.Find("UserLevel_Info/Slider");
    }

    // Update is called once per frame
    void Update()
    {
        int score = PlayerData.Instance.GetExperience();
        UserExperience.GetComponent<TextMeshProUGUI>().text = (score % 400).ToString() + "/400";
        UserLevel.GetComponent<TextMeshProUGUI>().text = (score / 400).ToString();
        UserExpSlider.GetComponent<Slider>().value = (score % 400) / 400f;
    }
}
