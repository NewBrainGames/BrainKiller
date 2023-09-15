using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlaveController : MonoBehaviour
{
    private NavMeshAgent SlaveAgent;
    private Transform player;
    private Vector3 SlaveSourcePos;
    private Vector3 SlaveTargetPos;
    private Vector3[] m_WayPoint = new Vector3[30];
    private int SlaveAreaMask;
    private NavMeshHit SlaveNavMeshHit;
    private NavMeshPath SlaveNavMeshPath;

    private float minDistance = 10;
    private GameObject bullet;
    private bool isFire = false;
    private float minAngle = 15;
    private float damage = 1;
    private float fireInterval = 1f;
    public bool coolDown;
    private float coolTime;
    private float enemyShootSpeed = 15;

    private Animator SlaveAni;
    private Transform enemyShootPoint;//敌人开火点
    //public HoverbotAnimatorController slaveAni;
    private BattleManager bm;
    // Start is called before the first frame update
    void Start()
    {
        SlaveAgent = this.GetComponent<NavMeshAgent>();
        //slaveAni =  this.GetComponent<HoverbotAnimatorController>();
        player = GameObject.Find("Player").GetComponent<Transform>();
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        SlaveAni = this.GetComponent<Animator>();
        bullet = GameObject.Find("EnemyBullet");
        //enemyShootPoint = GameObject.Find("Slave/EnemyShootPoint").GetComponent<Transform>();
        SlaveSourcePos = transform.position;
        SlaveTargetPos = player.transform.position;
        int area = NavMesh.GetAreaFromName("Walkable");
        SlaveAreaMask = 1 << area;
        SlaveNavMeshPath = new NavMeshPath();
        SlaveAgent = GetComponent<NavMeshAgent>();
        SlaveAgent.autoRepath = true;
        SlaveAgent.autoBraking = true;
        SlaveAgent.radius = 0.6f;
        SlaveAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        SlaveAgent.acceleration = 10000;
        SlaveAgent.angularSpeed = 2000;
        SlaveAgent.speed = 3.5f;
        SlaveAgent.updateRotation = true;
        SlaveAgent.stoppingDistance = 5f;
    }

    // Update is called once per frame
    void Update()
    {


        if (this.name != "Slave"&& this.name != "Slave(robot)")
        {

            //    if (enemyShootPoint == null)
            //    {
            //        Transform[] children = this.gameObject.GetComponentsInChildren<Transform>();
            //        foreach (Transform var in children)
            //        {
            //            if (var.gameObject.name == "EnemyShootPoint")
            //            {
            //                enemyShootPoint = var;
            //                break;
            //            }
            //        }
            //    }
            FindPath();
            //slaveAni.alerted = true;
            //    if (Vector3.Distance(SlaveAgent.transform.position, player.transform.position) < minDistance)
            //    {
            //        //攻击玩家
            //        FireControl();

            //    }
        }

    }
    private void FindPath()
    {
        SlaveSourcePos = transform.position;
        SlaveTargetPos = player.transform.position;
        SlaveAgent.destination = SlaveTargetPos;
        transform.LookAt(SlaveTargetPos);
        if (NavMesh.Raycast(SlaveSourcePos, SlaveTargetPos, out SlaveNavMeshHit, SlaveAreaMask))
        {
            if (NavMesh.CalculatePath(SlaveSourcePos, SlaveTargetPos, SlaveAreaMask, SlaveNavMeshPath))
            {
                if (SlaveNavMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    int wayPointLength = SlaveNavMeshPath.GetCornersNonAlloc(m_WayPoint);
                    if (wayPointLength > m_WayPoint.Length)
                    {
                        m_WayPoint = new Vector3[wayPointLength];
                        SlaveNavMeshPath.GetCornersNonAlloc(m_WayPoint);
                    }
                    SlaveNavMeshPath.ClearCorners();


                }
            }
        }



    }
    private void FireControl()
    {
       
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
        var direction = (player.transform.position - enemyShootPoint.transform.position).normalized;
        if (Vector3.Angle(direction, enemyShootPoint.forward) < minAngle)
        {

            if (coolDown)
            {
                StartCoroutine("Fire");
                isFire = true;
            }
        }
        if (Vector3.Angle(direction, enemyShootPoint.forward) < minAngle)
        {
            StartCoroutine("Fire");
            isFire = true;

        }
        else
        {

            StopCoroutine("Fire");
            isFire = false;

        }

    }

    IEnumerator Fire()
    {
        while (isFire)
        {
            if (bullet != null && enemyShootPoint != null)
            {
                RaycastHit hit;
                if (Physics.Raycast(enemyShootPoint.position, enemyShootPoint.forward, out hit, 100f))
                {
                    if (hit.collider.gameObject.tag == "Player")
                    {
                        bm.Attack(hit.collider.gameObject, damage);
                        hit.collider.gameObject.GetComponent<PlayerController>().aniController.TriggerOnDamage();
                    }


                }

                //GameObject newBullet = Instantiate(bullet, enemyShootPoint.position, enemyShootPoint.rotation);
                //newBullet.GetComponent<Rigidbody>().velocity = -1 * newBullet.transform.forward * enemyShootSpeed;
                //Destroy(newBullet, 2);
            }
            //slaveAni.TriggerAttack();
            yield return new WaitForSeconds(fireInterval);
        }
    }
}
