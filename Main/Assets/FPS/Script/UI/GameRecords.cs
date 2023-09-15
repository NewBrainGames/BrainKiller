using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameRecords : MonoBehaviour
{
    // back button control
    public GameObject homePanel;
    private Button buttonBack;
    
    public GameObject dailyRecordContent;
    public GameObject dailyRecordItem;

    private TextMeshProUGUI TotalCheckingPoints;
    private TextMeshProUGUI TotalGeneratedCheckingPoints;
    private TextMeshProUGUI TotalSwcLength;
    
    // Start is called before the first frame update
    void Start()
    {
        buttonBack = GameObject.Find("Button_Back").GetComponent<Button>();
        buttonBack.onClick.AddListener((OnBtnBackClick));

        TotalCheckingPoints = DeepFindChild(transform, "TotalCP").GetComponent<TextMeshProUGUI>();
        TotalGeneratedCheckingPoints = DeepFindChild(transform, "TotalGP").GetComponent<TextMeshProUGUI>();
        TotalSwcLength = DeepFindChild(transform, "TSL").GetComponent<TextMeshProUGUI>();
        
        InitDailyGameRecords();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        
    }

    public void InitDailyGameRecords()
    {
        List<Record> records = PlayerData.Instance.GetDailyGameRecord();
        if (records == null)
        {
            Debug.Log("record is empty");
        }else
        {
            foreach (var record in records)
            {
                GameObject dRecordItem = Instantiate(dailyRecordItem, dailyRecordContent.transform, true);
                TMP_Text infoText = null;
                TMP_Text scoreText = null;
                
                
                Image icon = null;
                List<GameObject> rankStars = new List<GameObject>();

                for (int i = 0; i < dRecordItem.transform.childCount; i++)
                {
                    var childObject = dRecordItem.transform.GetChild(i).gameObject;
                    Console.WriteLine(childObject.name);
                    Debug.Log(childObject.name);
                    if (childObject.name == "Text_Info")
                    {
                        infoText = childObject.GetComponent<TMP_Text>();
                    } else if (childObject.name == "Icon")
                    {
                        icon = childObject.GetComponent<Image>();
                    } else if (childObject.name == "Text_Score")
                    {
                        scoreText = childObject.GetComponent<TMP_Text>();
                    } else if (childObject.name == "RankStar")
                    {
                        for (int j = 0; j < childObject.transform.childCount; j++)
                        {
                            rankStars.Add(childObject.transform.GetChild(j).gameObject);
                        }
                    }
                }

                if (infoText != null)
                {
                    // infoText.text = "Check Branching Points\n<size=26><color=#BEB5B6>Image Id: 001</color></size>";
                    infoText.text = record.GameMode + "\n" + "<size=26><color=#BEB5B6>Image Id: " + record.ImageId + "</color></size>";
                }
                else
                {
                    Debug.Log("infoText not found");
                }
                
                if (scoreText != null)
                {
                    scoreText.text = "Score: " + record.Score;
                } else
                {
                    Debug.Log("scoreText not found");
                }

                if (TotalCheckingPoints != null)
                {
                    TotalCheckingPoints.text = record.TotalCheckingPoints.ToString();
                }
                else
                {
                    Debug.Log("TotalCheckingPointsText not found");
                }

                if (TotalSwcLength != null)
                {
                    TotalSwcLength.text = record.TotalSwcLength.ToString();
                }
                else
                {
                    Debug.Log("TotalSwcLengthText not found");
                }

                if (TotalGeneratedCheckingPoints != null)
                {
                    TotalGeneratedCheckingPoints.text = record.TotalGeneratedCheckingPoints.ToString();
                }
                else
                {
                    Debug.Log("TotalGeneratedCheckingPoints text not found");
                }

                // stars
                if (record.Score >= 60 && record.Score < 75)
                {
                    rankStars[0].SetActive(false);
                    rankStars[1].SetActive(false);
                } else if (record.Score >= 75 && record.Score < 90)
                {
                    rankStars[0].SetActive(false);
                }
                
                dRecordItem.SetActive(true);
            }
            
            
            
        }

        
    }


    
    void OnBtnBackClick()
    {
        GameObject panelRecords = GameObject.Find("Panel_GameRecords");
        panelRecords.SetActive(false);
        if (homePanel != null)
        {
            homePanel.SetActive(true);
        }
    }

    static public Transform DeepFindChild(Transform root, string childName)
    {
        Transform result = null;
        result = root.Find(childName);
        if (result == null)
        {
            foreach (Transform trs in root)
            {
                result = DeepFindChild(trs, childName);
                if (result != null)
                {
                    return result;
                }

            }
        }

        return result;
    }
}
