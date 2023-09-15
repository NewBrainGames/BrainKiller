using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManagerPractice : MonoBehaviour
{
    // Start is called before the first frame update
    public SwcGenerator_Practice SwcPrac;
    // public SwcGenerator SwcGen;
    public Weapon_Controller WeaCtrl;
    private List<GameObject> showTable = new List<GameObject>();
    public int count = 10;
    private int score;
    public GameObject enemy;
    public GameObject appearEffect;
    public bool enemyAppear = false;
    public GameObject coinList;

    public enum Mode
    {
        PRACTICE,   //��ϰģʽ
        ARCADE      //�ֻ�ģʽ
    };

    public Mode gameMode;
    void Start()
    {
        gameMode = Mode.PRACTICE;
        score = 0;
        //appearEffect = GameObject.FindGameObjectWithTag("Appear");

    }

    public struct PlayerInfo
    {
        public GameObject Player;
        public string name;
        public float HP;
    }

    public struct EnemyInfo
    {
        public GameObject Enemy;
        public string name;
        public float HP;
    }

    private List<PlayerInfo> PlayerList = new List<PlayerInfo>();
    private List<EnemyInfo> EnemyList = new List<EnemyInfo>();
    
    public enum BattleObject
    {
        PLAYER,
        ENEMY
    };

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(gameMode);
        switch (gameMode)
        {
            case Mode.PRACTICE:
                {
                    practice();
                    break;
                }
            case Mode.ARCADE:
                {
                    Battle001();
                    break;
                }
        }

    }

    void practice()
    {
        //// todo: change game logic
        List<RaycastHit> HitList = WeaCtrl.hitTarget();
        foreach (RaycastHit hit in HitList)
        {
            score++;
            // SwcPrac.desBpSwc(hit.collider.gameObject);
            // check if is fakeBranchingPoint first, f it's fake then punish player else add scores
            //    int result = SwcPrac.checkForFakeBp(hit.collider.gameObject);
            //    switch (result)
            //    {
            //       case 1: // hit real branching point
            //           //Debug.Log("you just hit a real bp");
            //           score++;
            //           break;
            //       case 0:
            //           break;
            //       case -1: // hit fake branching point
            //           //Debug.Log("you just hit a fake bp");
            //           score--;
            //           break;
            //    }
            //    if (result != 0)
            //    {
            //        SwcPrac.desBpSwc(hit.collider.gameObject);
            //    }
        }

        WeaCtrl.clearHitList();
    }

    void Battle001()
    {
        Invoke("EnemyAwake",10);
        Debug.Log("--------Enemy Awake Invoke---------");
        
        //Debug.Log("6666");
        List<RaycastHit> HitList = WeaCtrl.hitTarget();
        foreach (RaycastHit hit in HitList)
        {
            if (hit.collider.gameObject.CompareTag("Bp"))
            {
                // SwcGen.desBpSwc(hit.collider.gameObject);
                score++;
            }
            if (hit.collider.gameObject.name == "Slave(Clone)")
            {
                Destroy(hit.collider.gameObject);
            }

        }

        WeaCtrl.clearHitList();
    }
    public int getScore()
    {
        return score;
    }

    public void EnemyAwake()
    {
        // Debug.Log("--------Enemy Awake Call---------");
        // Debug.Log(enemy.gameObject);
        // Debug.Log(enemyAppear);
        // if(enemy.gameObject&&enemyAppear == false)
        // {
        //     enemy.gameObject.SetActive(true);
        //     enemyAppear = true;
        //     GameObject newEffect = Instantiate(appearEffect, enemy.transform.position, appearEffect.transform.rotation);
        //     Destroy(newEffect, 1);
        // }
        
    }

    public void PlayerLogin(GameObject Player,string name = "NULL")
    {
        PlayerInfo nplayer = new PlayerInfo();
        nplayer.Player = Player;
        nplayer.name = name;
        nplayer.HP = 100;
        PlayerList.Add(nplayer);
    }

    public void PlayerLogout(GameObject Player)
    {
        foreach(PlayerInfo p in PlayerList)
        {
            if (p.Player == Player)
            {
                PlayerList.Remove(p);
                break;
            }
        }
    }

    public void EnemyLogin(GameObject Enemy, float HP,string name = "NULL")
    {
        EnemyInfo nenemy = new EnemyInfo();
        nenemy.Enemy = Enemy;
        nenemy.name = name;
        nenemy.HP = HP;
        EnemyList.Add(nenemy);
    }

    public void EnemyLogout(GameObject Enemy)
    {
        foreach(EnemyInfo e in EnemyList)
        {
            if (e.Enemy == Enemy)
            {
                EnemyList.Remove(e);
                break;
            }
        }
    }

    public void Damage(GameObject O,BattleObject BO, float damage)
    {
        // switch (BO)
        // {
        //     case BattleObject.PLAYER:
        //         for(int i = 0; i < PlayerList.Count; i++)
        //         {
        //             if (PlayerList[i].Player == O)
        //             {   
        //                 PlayerInfo p = PlayerList[i];
        //                 p.Player.GetComponent<Health_Controller>().Damage(damage);
        //                 //Debug.Log("666");
        //                 p.HP = p.Player.GetComponent<Health_Controller>().getHP();
        //                 break;
        //             }
        //         }
        //         break;
        //     case BattleObject.ENEMY:
        //         for (int i = 0; i < EnemyList.Count; i++)
        //         {
        //             if (EnemyList[i].Enemy == O)
        //             {
        //                 EnemyInfo e = EnemyList[i];
        //                 e.Enemy.GetComponent<Health_Controller>().Damage(damage);
        //                 e.HP = e.Enemy.GetComponent<Health_Controller>().getHP();
        //                 break;
        //             }
        //         }
        //         break;
        // }
        switch (BO)
        {
            case BattleObject.PLAYER:
                for(int i = 0; i < PlayerList.Count; i++)
                {
                    if (PlayerList[i].Player == O)
                    {   
                        PlayerInfo p = PlayerList[i];
                        p.Player.GetComponent<AttributeController>().Damage(damage);
                        //Debug.Log("666");
                        p.HP = p.Player.GetComponent<AttributeController>().GetHP();
                        break;
                    }
                }
                break;
            case BattleObject.ENEMY:
                for (int i = 0; i < EnemyList.Count; i++)
                {
                    if (EnemyList[i].Enemy == O)
                    {
                        EnemyInfo e = EnemyList[i];
                        e.Enemy.GetComponent<AttributeController>().Damage(damage);
                        e.HP = e.Enemy.GetComponent<AttributeController>().GetHP();
                        break;
                    }
                }
                break;
        }
        switch (BO)
        {
            case BattleObject.PLAYER:
                for(int i = 0; i < PlayerList.Count; i++)
                {
                    if (PlayerList[i].Player == O)
                    {   
                        PlayerInfo p = PlayerList[i];
                        p.Player.GetComponent<AttributeController>().Damage(damage);
                        //Debug.Log("666");
                        p.HP = p.Player.GetComponent<AttributeController>().GetHP();
                        break;
                    }
                }
                break;
            case BattleObject.ENEMY:
                for (int i = 0; i < EnemyList.Count; i++)
                {
                    if (EnemyList[i].Enemy == O)
                    {
                        EnemyInfo e = EnemyList[i];
                        e.Enemy.GetComponent<AttributeController>().Damage(damage);
                        e.HP = e.Enemy.GetComponent<AttributeController>().GetHP();
                        break;
                    }
                }
                break;
        }
    }

    public float GetHP(GameObject O, BattleObject BO)
    {
        switch (BO)
        {
            case BattleObject.PLAYER:
                foreach(PlayerInfo p in PlayerList)
                {
                    if (p.Player == O)
                    {
                        return p.HP;
                    }
                }
                break;
            case BattleObject.ENEMY:
                foreach (EnemyInfo e in EnemyList)
                {
                    if (e.Enemy == O)
                    {
                        return e.HP;
                    }
                }
                break;
        }
        return -1f;
    }

    public void toggleRawImage()
    {
        SwcPrac.toggleRAWImage();
    }
}
