using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    //NavMesh
    private NavMeshAgent enemyAgent;
    private NavMeshPath enemyNavMeshPath;
    private Vector3 enemySourcePos;
    private Vector3 enemyTargetPos;
    private Vector3[] m_WayPoint = new Vector3[30];
    private int enemyAreaMask;
    private NavMeshHit enemyNavMeshHit;

    public GameObject player;
    public PlayerController _playerController;
    public GameObject enemy;
    public float minDistance = 10;

    public GameObject bullet;
    public Transform enemyShootPoint;//敌人开火点
    public float enemyShootSpeed = 10;
    public float minAngle = 15; 
    public float damage = 10;
    public float enemyHP = 10000;
    public Slider HP_Slider;
    public bool isFire = false;
    public float fireInterval = 1f;
    public float fireDistance = 100f;
    public bool coolDown;
    private float coolTime;

    //Animator
    public HoverbotAnimatorController enemyAni;
    public BattleManager bm;
    private Transform m_enemy_position;
    private GameObject Slave;
    private bool isSummonStart = false;
    private List<GameObject> slaveList=new List<GameObject>();

    //AudioSource
    private AudioSource enemyAudio;
    public AudioClip clip;
    private string enemyAudioPath = "Audio/EnemyAudio/";

    // Start is called before the first frame update
    void Start()
    {
        bm.RoleRegister(this.gameObject);
        enemy = GameObject.Find("Enemy_02");
        enemyShootPoint = GameObject.FindGameObjectWithTag("EnemyShootPoint").transform;
        enemyAudio = enemy.GetComponent<AudioSource>();
        HP_Slider = GameObject.Find("EnemyHP_Slider").GetComponent<Slider>();
        bullet = GameObject.Find("EnemyBullet");
        fireInterval = 3f;
        damage = 0.1f;
        player = GameObject.FindGameObjectWithTag("Player");
        _playerController = player.GetComponent<PlayerController>();//获取player中的PlayerController脚本组件
        enemyAni = this.GetComponent<HoverbotAnimatorController>();//获取动画脚本
        enemy.gameObject.SetActive(false);
        m_enemy_position = this.GetComponent<Transform>();
        Slave = GameObject.Find("Slave");

        //NavMesh
        enemySourcePos = transform.position;
        enemyTargetPos = player.transform.position;

        int area = NavMesh.GetAreaFromName("Walkable");
        enemyAreaMask = 1 << area;
        enemyNavMeshPath = new NavMeshPath();
        // Set NavMeshAgent
        enemyAgent = GetComponent<NavMeshAgent>();
        enemyAgent.autoRepath = true;
        enemyAgent.autoBraking = true;
        enemyAgent.radius = 0.6f;
        enemyAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        enemyAgent.acceleration = 10000;
        enemyAgent.angularSpeed = 2000;
        enemyAgent.speed = 3.5f;
        enemyAgent.updateRotation = true;
        enemyAgent.stoppingDistance = 5f;


    }

    // Update is called once per frame
    void Update()
    {
        FindPath();
        enemyAni.moveSpeed = enemyAgent.speed;
        enemyAni.alerted = true;
        if (bm.enemyAppear == true)
        {
            clip = Resources.Load<AudioClip>(enemyAudioPath + "Warning");
            enemyAudio.clip = clip;
            enemyAudio.playOnAwake = false;
        }
        
        if (isSummonStart == false)
        {
            
            if(this.name == "Enemy_02"&&Slave != null)
            {
                isSummonStart = true;
                StartCoroutine(SummonSlaves(Slave));
            }
                
        }
        if(Vector3.Distance(enemyAgent.transform.position,_playerController.transform.position)<minDistance)
        {

            FireControl();
                
        }
    }

    public void FindPath()
    {
        enemySourcePos = transform.position;
        enemyTargetPos = player.transform.position;
        enemyAgent.destination = enemyTargetPos;
        transform.LookAt(enemyTargetPos);
        if (NavMesh.Raycast(enemySourcePos, enemyTargetPos, out enemyNavMeshHit, enemyAreaMask))
        {
            if (NavMesh.CalculatePath(enemySourcePos, enemyTargetPos, enemyAreaMask, enemyNavMeshPath))
            {
                if (enemyNavMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    int wayPointLength = enemyNavMeshPath.GetCornersNonAlloc(m_WayPoint);
                    if (wayPointLength > m_WayPoint.Length)
                    {
                        m_WayPoint = new Vector3[wayPointLength];
                        enemyNavMeshPath.GetCornersNonAlloc(m_WayPoint);
                    }
                    enemyNavMeshPath.ClearCorners();

            
                }
            }
        }

        

    }


    private void FireControl()
    {
        float distance = (_playerController.transform.position - enemy.transform.position).magnitude;
        if (distance <= fireDistance)
        {
            PlayEnemyAudio();
        }

        if (coolTime >= fireInterval)
        {
            coolDown = true;
            coolTime = 0;
        }
        else
        {
            coolTime += Time.deltaTime;
        }
        //在敌人射击角度内才攻击玩家
        var direction = (_playerController.transform.position - enemyShootPoint.transform.position).normalized;
        if(Vector3.Angle(direction,enemyShootPoint.forward)<minAngle&&distance<=fireDistance)
        {
            if (coolDown)
            {
                StartCoroutine("Fire");
                isFire = true;
            }
        }
        else
        {

                StopCoroutine("Fire");
                isFire = false;

        }
        
    }

    IEnumerator Fire()
    {
        while(isFire)
        {
            if(bullet!=null && enemyShootPoint != null)
            {
                RaycastHit hit;
                if (Physics.Raycast(enemyShootPoint.position, enemyShootPoint.forward, out hit, 80f))
                {
                    if (hit.collider.gameObject.tag == "Player")
                    {
                        bm.Attack(hit.collider.gameObject, damage);
                        hit.collider.gameObject.GetComponent<PlayerController>().aniController.TriggerOnDamage();
                    }


                }

                GameObject newBullet = Instantiate(bullet, enemyShootPoint.position, enemyShootPoint.rotation);
                newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * enemyShootSpeed;
                Destroy(newBullet, 2);
            }
            enemyAni.TriggerAttack();//触发开火动画
            yield return new WaitForSeconds(fireInterval);
        }
    }

    IEnumerator SummonSlaves(GameObject Slave)
    {
        while (isSummonStart)
        {
            yield return new WaitForSeconds(10.0f);
            Vector3 center = m_enemy_position.position;
            GameObject slave1 = Instantiate(Slave, 5 * Vector3.forward + center, Quaternion.identity);
            slaveList.Add(slave1);
            GameObject slave2 = Instantiate(Slave, center - 5 * Vector3.forward, Quaternion.identity);
            slaveList.Add(slave2);
            GameObject slave3 = Instantiate(Slave, center - 5 * Vector3.right, Quaternion.identity);
            slaveList.Add(slave3);
            GameObject slave4 = Instantiate(Slave, center + 5 * Vector3.right, Quaternion.identity);
            slaveList.Add(slave4);

        }

    }

    public void ClearSlaveList()
    {
        isSummonStart = false;
        if (slaveList.Count > 0)
        {
            foreach(GameObject var in slaveList)
            {
                if (var != null)
                    Destroy(var);
            }
        }
        slaveList.Clear();
    }

    public void PlayEnemyAudio()
    { 

        enemyAudio.Play();
    }

}
