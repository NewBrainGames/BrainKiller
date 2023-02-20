using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevel : MonoBehaviour
{
    public Text gameLevelText;
    public Text hintsLabel;
    
    // Start is called before the first frame update
    void Start()
    {
        gameLevelText = GameObject.Find("TextLevel").GetComponent<Text>();
        hintsLabel = GameObject.Find("HintsLabel").GetComponent<Text>();
        int savedLevelId = PlayerData.Instance.GetDenLevelId();
        Debug.Log("level-ui, player saved id" + savedLevelId);
        if (savedLevelId == -1)
        {
            savedLevelId = 0;
        }
        
        if (gameLevelText != null)
        {
            gameLevelText.text = "Level-" + (savedLevelId + 1).ToString();
        }
        else
        {
            
            gameLevelText.text = "Level-" + (savedLevelId + 1).ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateGameLevel(int level)
    {
        if (gameLevelText != null)
        {
            gameLevelText.text = "Level-" + (level + 1).ToString();
        }
        else
        {
            Debug.Log("game level not found");
        }
    }

    public void setLabel(string s)
    {
        gameLevelText.text = s;
    }

    public void setHintsLabel(string s)
    {
        hintsLabel.text = s;
    }
}
