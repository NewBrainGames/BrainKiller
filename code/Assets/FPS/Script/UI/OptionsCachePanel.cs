using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class OptionsCachePanel : MonoBehaviour
{
    public Button buttonClearDailyCache;
    
    // Start is called before the first frame update
    void Start()
    {
        if (buttonClearDailyCache == null)
        {
            buttonClearDailyCache = GameObject.Find("").GetComponent<Button>();
        }
        
        buttonClearDailyCache.onClick.AddListener((OnButtonClearDailyCacheClick));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnButtonClearDailyCacheClick()
    {
        PlayerData.Instance.ClearDailyGameRecord();
    }
}
