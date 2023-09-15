using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private SwcGenerator SwcGen;
    [SerializeField] private SwcGenerator_Practice SwcPrac;
    [SerializeField] private Weapon_Controller WeaponCtrl;
    [SerializeField] private InstructController ic;
    private int bpnums = 0;
    //private int count = 10;   //max number of swc showed on screen

    //hit score
    private int score;

    //
    private Dictionary<string, List<GameObject>> tagObjectMap = new Dictionary<string, List<GameObject>>();

    //
    public Mode gameMode { get; private set; }
    public GameObject player;
    public GameObject enemy;
    public GameObject appearEffect;
    public bool enemyAppear = false;
    public GameObject coinList;

    //001Instruct
    public GameObject checkPoint1;
    public bool firstCure = true;
    public bool firstEnemy = true;
    public bool firstReloadBullet = true;
    public bool firstShootSwc = true;
    public bool firstEnemyComing = true;
    public bool instructEnd = true;

    public bool firstPracticeInstruct = true;
    public bool firstPracticeSoma = true;

    void Start()
    {
        if (GameConfig.Instance)
        {
            gameMode = GameConfig.Instance.GetGameMode();
        }
        else
        {
            gameMode =  Mode.Dendrite;
        }
        player = GameObject.Find("Player");
        //if (gameMode == Mode.ARCADE)
            checkPoint1 = GameObject.Find("CheckPoint1");
        ic = GameObject.FindGameObjectWithTag("InstructText").GetComponent<InstructController>();
        mInitializeObject();

    }

    void Update()
    {
        switch (gameMode)
        {
            case Mode.Dendrite:
            case Mode.Axon:
                {
                    practice();
                    break;
                }
            case Mode.battle001:
                {
                    practice();
                    break;
                }
        }

    }
    private GameObject mGetChildGameObject(string SourceObjectName, string TargetObjectName)     //find certain child through parents
    {
        Transform[] children = GameObject.Find(SourceObjectName).GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child && child.name == TargetObjectName)
            {
                return child.gameObject;
            }

        }
        Debug.Log("SourceObjectName or TargetObjectName is not exist, please check your name.");
        return null;
    }

    private GameObject mGetParentGameObject(string SourceObjectName, string TargetObjectName)   //find certain parents through child
    {
        Transform[] parents = GameObject.Find(SourceObjectName).GetComponentsInParent<Transform>();
        foreach (Transform parent in parents)
        {
            if (parent && parent.name == TargetObjectName)
                return parent.gameObject;
        }
        Debug.Log("SourceObjectName or TargetObjectName is not exist, please check your name.");
        return null;
    }

    private GameObject mGetTopParentObject(GameObject Child)                                    //find the top parent
    {
        if (Child == null)
        {
            Debug.LogError("You must specify the child object.");
            return null;
        }
        GameObject parent = Child;
        while (Child.transform.parent)
        {
            parent = Child.transform.parent.gameObject;
        }
        return parent;
    }
    private void mInitializeObject()
    {
        score = 0;
        switch (gameMode)
        {
            case Mode.Dendrite:
            case Mode.Axon:
                if (!GameObject.Find("SwcGenerator").TryGetComponent<SwcGenerator_Practice>(out this.SwcPrac))
                {
                    Debug.Log("SwcGenerator_Practice GetComponent failed.");
                }
                break;
            case Mode.battle001:
                if (!GameObject.Find("SwcGenerator").TryGetComponent<SwcGenerator_Practice>(out this.SwcPrac))
                {
                    Debug.Log("SwcGenerator GetComponent failed.");
                }
                break;
        }      
        if (!mGetChildGameObject("Player", "Weapon_Root") || !mGetChildGameObject("Player", "Weapon_Root").TryGetComponent<Weapon_Controller>(out this.WeaponCtrl))
        {
            Debug.LogError("Weapon_Controller GetComponent failed.");
        }
    }
    void practice()
    {
        if (player.transform.position.z > checkPoint1.transform.position.z && firstPracticeInstruct == true)
        {
            // ic.AddText("Find and shoot the correct bifurcation points to score, shoot the correct but unmarked (by the viruses) bifurcation points for three consecutive times to complete the marking, and avoid shooting the wrong bifurcation points\n"
                // + "(Press 'I' to check the differences betwween this three)");

            firstPracticeInstruct = false;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            // ic.AddText("correct bifurcation points: which exist both in neurons and raw images\n" + "unmarked correct bifurcation points: which exist in raw images but not in neurons\n" + "wrong bifurcation points: which exist in neurons but not in raw images");
        }
        
        if (SwcPrac.state == SWCState.toBeExploded && firstPracticeSoma == true)
        {
            if (GameConfig.Instance) 
            {
                if (GameConfig.Instance.GetGameMode() == Mode.Dendrite)
                {
                    ic.AddText("完成标注之后，可以射击中心怪物进入下一关");
                }else if (GameConfig.Instance.GetGameMode() == Mode.Axon)
                {
                    ic.AddText("完成标注之后，按下 N 键进入下一关"); 
                }
                firstPracticeSoma = false;
            }
            
        }
    }

    void battle001()
    {
        if (player.transform.position.x > checkPoint1.transform.position.x && firstCure == true)
        {
            ic.AddText("Pick up the medical kit in front for healing");
            firstCure = false;
        }
        if(firstCure == false)
            Invoke("EnemyAwake",5);
        if (enemyAppear == true && firstEnemyComing == true)
        {
            ic.AddText("Warning! Enemies are coming! Let's go to destroy them!");
            firstEnemyComing = false;
        }


        List<RaycastHit> HitList = WeaponCtrl.hitTarget();
        if(HitList.Count>0)
            foreach (RaycastHit hit in HitList)
            {
                if (hit.collider)
                {
                    if (hit.collider.gameObject&&hit.collider.gameObject.CompareTag("Bp"))
                    {
                        
                        SwcGen.desBpSwc(hit.collider.gameObject);
                        //score++;
                        if (firstShootSwc == true && score>0)
                        {
                            //ic.AddText("You have gained power from the Branching Points! Let's go to destroy the Enemies!");
                            firstShootSwc = false;
                        }
         
                    }
                    //Debug.Log(hit.collider.gameObject);
                    //if (hit.collider.transform && hit.collider.transform.parent != null && hit.collider.transform.parent.gameObject.tag == "Slave")
                    //{
                    //    Destroy(hit.collider.transform.parent.gameObject);
                    //}
                    if (hit.collider.gameObject && hit.collider.gameObject.tag == "Slave")
                    {
                        Destroy(hit.collider.gameObject);
                    }
                }
                AttackObject(hit);
            }

        List<GameObject> GrenadeExplodeList = WeaponCtrl.GrenadeExplodeList;
        if (GrenadeExplodeList.Count > 0)
            foreach (GameObject gel in GrenadeExplodeList)
            {
                
                    if (gel && gel.CompareTag("Bp"))
                    {
                        WeaponCtrl.damage += WeaponCtrl.addDamage;
                        SwcGen.desBpSwc(gel);
                        
                        //score++;
                        if (firstShootSwc == true && score > 0)
                        {
                            ic.AddText("You have gained power from the Branching Points! Let's go to destroy the Enemies!");
                            firstShootSwc = false;
                        }

                    }

                    if (gel && gel.tag == "Slave")
                    {
                        Destroy(gel);
                    }
                
            }
                

        
        WeaponCtrl.clearHitList();
    }
    public int getScore()
    {
        return score;
    }

    public void EnemyAwake()
    {
        if(enemy.gameObject&&enemyAppear == false)
        {
            enemy.gameObject.SetActive(true);
            enemy.GetComponent<EnemyController>().PlayEnemyAudio();
            enemyAppear = true;
            GameObject newEffect = Instantiate(appearEffect, enemy.transform.position, appearEffect.transform.rotation);
            Destroy(newEffect, 1);
        }
        
    }


    public bool RoleRegister(GameObject Role)       //register the role in current game
    {
        if (!tagObjectMap.ContainsKey(Role.tag))
        {
            List<GameObject> tRoleTable = new List<GameObject>();
            tRoleTable.Add(Role);
            tagObjectMap.Add(Role.tag, tRoleTable);
            return true;
        }
        foreach (var obj in tagObjectMap[Role.tag])
        {
            if (obj == Role)
            {
                Debug.LogError("GameObject already exist.");
                return false;
            }
        }
        tagObjectMap[Role.tag].Add(Role);
        return true;
    }

    public void RoleCancel(GameObject Role = null)
    {
        if (Role == null)
        {
            foreach (var objtag in tagObjectMap)
            {
                foreach (var obj in objtag.Value)
                {
                    if (obj != null)
                        Destroy(obj);
                }
                objtag.Value.Clear();
            }
            tagObjectMap.Clear();
            return;
        }
        if (!tagObjectMap.ContainsKey(Role.tag))
            return;
        if (tagObjectMap[Role.tag].Count > 0)
        {

            tagObjectMap[Role.tag].Remove(Role);
            Destroy(Role);
            if (tagObjectMap[Role.tag].Count <= 0)
                tagObjectMap.Remove(Role.tag);
        }
    }

    public void RoleCancel(string Tag)
    {
        if (tagObjectMap[Tag].Count > 0)
        {
            foreach(var obj in tagObjectMap[Tag])
            {
                if (obj)
                    Destroy(obj);
            }
            tagObjectMap[Tag].Clear();
            tagObjectMap.Remove(Tag);
        }
    }


    public void Attack(GameObject Target, float damage)                                     //attack gameobject registered in battlemanager
    {
        if (tagObjectMap.ContainsKey(Target.tag))
        {
            foreach( var target in tagObjectMap[Target.tag])
            {
                if (target == Target)
                {
                    mGetTopParentObject(target).GetComponent<AttributeController>().Damage(damage);
                    return;
                }
            } 
        }
    }

    public void AttackObject(RaycastHit hit,bool isWrong=false)
    {
        GameObject newButter = new GameObject();

        if (hit.collider.gameObject.CompareTag("Bp"))
        {
            if (isWrong == false) {
                bpnums++;
                score++;
            }

            if (gameMode == Mode.Dendrite || gameMode == Mode.Axon)
            {
                SwcPrac.hitBp(hit.collider.gameObject,isWrong);
                //ic.AddText("Shoot the right Branching Points to score");
            }
            else if (gameMode == Mode.battle001)
            {
                SwcGen.desBpSwc(hit.collider.gameObject);
                WeaponCtrl.damage += WeaponCtrl.addDamage;
            }
            

        }
        else if (hit.collider.gameObject && hit.collider.gameObject.tag == "Enemy" && hit.collider.transform.parent && hit.collider.transform.parent.gameObject && hit.collider.transform.parent.gameObject.name == "Enemy_02")
        {
            if(firstEnemy == true && gameMode == Mode.battle001 && score == 0)
            {
                ic.AddText("It seems that your attack power is not enough. Go to shoot at the branching points to obtain your attack power!");
                firstEnemy = false;
            }
            if (firstEnemy == true && gameMode == Mode.battle001)
            {
                ic.AddText("Destroy the Enemies!");
                firstEnemy = false;
            }
            Attack(hit.collider.transform.parent.gameObject, WeaponCtrl.damage);
            hit.collider.transform.parent.gameObject.GetComponent<EnemyController>().enemyAni.TriggerOnDamage();
            

        }
        else if (hit.collider.gameObject.tag == "swcNode")
        {
            string[] words = hit.collider.transform.parent.gameObject.name.Split("-");
            int index = Int32.Parse(words[1]); 
            newButter = Instantiate(WeaponCtrl.butter, hit.collider.gameObject.transform.position, WeaponCtrl.bulletStartPoint.rotation);
            newButter.name += "-" + index;
        }
        else if (hit.collider.gameObject.CompareTag("soma") && gameMode == Mode.Dendrite)
        {
            SwcPrac.explodeSWC(hit.collider.gameObject);
            PostAnnotation();
            if (coinList)
            {
                coinList.SetActive(true);
            }

        }
        else if (hit.collider.gameObject.CompareTag("Butter"))
        {
            if (hit.collider.gameObject.transform.localScale.x < 1.0f)
                hit.collider.gameObject.transform.localScale += WeaponCtrl.addScale;
            if (hit.collider.gameObject.transform.localScale.x > 0.7f)
            {
                //newBranchingPoint = Instantiate(newBP,hit.collider.gameObject.transform.position,newBP.transform.rotation);
                if (gameMode == Mode.Dendrite || gameMode == Mode.Axon)
                {
                    SwcPrac.hitNormalSWCNode(hit.collider.gameObject);
                    bpnums++;
                }
            }
        }
        Destroy(newButter, 5f);
    }
    
    public float GetHealthPoint(GameObject Target)      
    {
        
        if (tagObjectMap.ContainsKey(Target.tag))
        {
            foreach (var target in tagObjectMap[Target.tag])
            {
                if (target == Target)
                {
                    return mGetTopParentObject(target).GetComponent<AttributeController>().GetHP();   
                }
            }
        }
        Debug.LogError("Cannot find the Target object.");
        return -1.0f;
    }

    public float GetCurrentWeaponDamage()
    {
        return WeaponCtrl.damage;
    }
    public void PostAnnotation()
    {
        PlayerDataPractice pdp = (new GameObject("PlayerDataPractice")).AddComponent<PlayerDataPractice>();
        string data = SwcPrac.GetBpAnnotation();
        pdp.Post(data);
    }

    public int GetBpNums()
    {
        return bpnums;
    }
    public void toggleRawImage()
    {
        if (SwcPrac != null)
        {
            SwcPrac.toggleRAWImage();
        }

        if (SwcGen != null)
        {
            SwcGen.toggleRAWImage();
        }
    }

    public void toggleRawSwc()
    {
        if (SwcPrac != null)
        {
            SwcPrac.toggleRAWSwc();
        }

        if (SwcGen != null)
        {
            SwcGen.toggleRAWSwc();
        }
    }
}
