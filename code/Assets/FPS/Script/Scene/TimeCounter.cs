using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeCounter : MonoBehaviour
{
    public GameObject timeText;
    public static int totalTime = 600;
    public GameObject Scores;

    private GameObject player;
    public float damage = 0;
    private BattleManager bm;
    private Weapon_Controller wc;
    // Start is called before the first frame update
    private TextMeshProUGUI score1;
    private TextMeshProUGUI time1;
    private SwcGenerator_Practice swcGenPrac;

    public bool run;
    void Start()
    {
        score1 = transform.Find("Status_Gold").Find("Score").GetComponent<TextMeshProUGUI>();
       
        time1 = transform.Find("Status_Time").Find("Time").GetComponent<TextMeshProUGUI>();

        swcGenPrac = GameObject.Find("SwcGenerator").GetComponent<SwcGenerator_Practice>();

        player = GameObject.Find("Player");
 
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        StartCoroutine(TimeCounter1());
        // InvokeRepeating("reduceTime",0.0f,1.0f);
        wc = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Weapon_Controller>();
    }


    IEnumerator TimeCounter1()
    {
        while (totalTime >= 0)
        {
            time1.text = totalTime.ToString() + "s";
            totalTime--;
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void setTime(int t)
    {
        totalTime = t;
    }

    public void onClickExit()
    {
        Application.Quit();
    }


}
