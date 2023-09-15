using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeCounter001 : MonoBehaviour
{
    public GameObject timeText;
    public static int totalTime = 150;
    public GameObject Scores;
    public GameObject Damage;
    private GameObject player;
    public float damage = 0;
    private int time = 1000;
    private BattleManager bm;
    private Weapon_Controller wc;
    // Start is called before the first frame update
    private TextMeshProUGUI score1;
    private TextMeshProUGUI time1;
    private TextMeshProUGUI damage1;
    void Start()
    {
        score1 = transform.Find("Status_Gold").Find("Score").GetComponent<TextMeshProUGUI>();
        damage1 = transform.Find("Status_Energy").Find("Damage").GetComponent<TextMeshProUGUI>();
        time1 = transform.Find("Status_Time").Find("Time").GetComponent<TextMeshProUGUI>();

        player = GameObject.Find("Player");
        damage = 0;
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        StartCoroutine(TimeCounter1());
        wc = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Weapon_Controller>();
    }

    private void Update()
    {
        int score = bm.getScore();
        //int score = 100;
        score1.text = score.ToString();

       
        if (bm.gameMode == Mode.battle001)
        {

            damage = wc.damage;
            //Damage.GetComponent<TextMeshPro>().text = "Hurt Values: " + damage;
            damage1.text = damage.ToString();
        }
    }

    IEnumerator TimeCounter1()
    {
        while (totalTime >= 0)
        {
            //timeText.GetComponent<TextMeshPro>().text = totalTime.ToString() + "s";
            time1.text = totalTime.ToString() + "s";
            yield return new WaitForSeconds(1);
            totalTime--;
            // print(totalTime);
            if (totalTime == 3)
            {
                //timeText.GetComponent<TextMeshPro>().color = Color.red;
                StartCoroutine(MoveText(totalTime));
            }
            if (totalTime == 0)
            {
                time = 0;
                //timeText.GetComponent<TextMeshPro>().text = "Fighting Bosses";
                time1.text = "Fighting Bosses";
            }
            if (SceneFadeInOut.timeRestart == true)
            {

                totalTime = 150;
                //timeText.GetComponent<TextMeshPro>().color = Color.black;
                SceneFadeInOut.timeRestart = false;
            }
        }
    }

    IEnumerator MoveText(int time)
    {
        while (time >= 0)
        {
            time--;
            //timeText.transform.DOScale(5, 1).From();
            //timeText.GetComponent<Text>().DOFade(0, 1).From();
            yield return new WaitForSeconds(1);
        }
    }

    int timeOver()
    {
        return time;
    }

    public void onClickExit()
    {
        Application.Quit();
    }


}
