using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Weapon_Controller : MonoBehaviour
{
    //script
    public BattleManager bm;
    public SceneObjectManager som;
    public InstructController ic;
    public GameObject iu;

    //Instruct
    private bool firstTime = true;
    //fire
    public Transform bulletStartPoint;
    public Transform grenadeStartPoint;
    //public Transform fireEffectPoint;
    public GameObject bullet;
    public GameObject butter;
    private GameObject player;
    public Transform ShootPoint;

    public float bulletStartSpeed = 100;
    public float fireInterval = 0.5f;
    public bool isFire = false;
    public float damage = 0;
    public float addDamage = 0;
    public bool coolDown;
    private float coolTime;
    public float fireRange = 100f;

    public float maxBulletCount = 50;
    public float currentCount = 50;
    public float reloadTime = 2;
    public float waitTime = 0.5f;
    public Slider bullet_Slider;

    public bool allowFire = true;
    
    //recoil
    public Transform defaultPoint;
    public Transform backPoint;
    public float lerpRatio = 0.2f;

    //visual effect
    //public ParticleSystem fireEffect;

    //sound effect
    public AudioSource shootAudio;
    public AudioSource reloadBulletAudio;

    //view control
    public Camera mainCamera;
    public Camera weaponCamera;
    public Vector3 weaponCameraDefaultPoint;
    public Vector3 weaponCameraCenterPoint;
    public float defaultView = 60;
    public float centerView = 30;
    public float viewLerpRatio = 0.2f;
    public Vector3 aimDirection;



    //shoot object

    public Vector3 addScale = new Vector3(0.1f, 0.1f, 0.1f);
    private List<RaycastHit> HitList = new List<RaycastHit>();
    public bool isPrac = false;
    public bool throwGrenade = false;
    public bool isDown = false;
    public GameObject GrenadeWeapon;
    public float GrenadeStartSpeed = 1000f;
    public GameObject PlayerThrowPoint;
    public GameObject newGrenade = null;
    public List<GameObject> GrenadeExplodeList;
    private string findPath = "Player/Eye_View/Main Camera/Weapon_Root/";
    public Material[] materials;
    private int currentgun, lastgun;
    public enum WeaponType
    {
        GUN,//0
        Grenade//1
    }

    public enum Weapon
    {
        HANDGUN,//0
        LASERGUN,//1
        sciFIGun,//2
        Grenade01//3
    };
    public int gunIndex = 0;
    public int gunNum = 4;

    public WeaponType weaponType;
    public Weapon myCurWeapon;
    public Grenade gc;
    [SerializeField]private Transform curWeapon;

    private bool isWrong;

    // Start is called before the first frame update
    void Start()
    {
        bm = GameObject.FindGameObjectWithTag("BM").GetComponent<BattleManager>();
        som = GameObject.FindGameObjectWithTag("SOM").GetComponent<SceneObjectManager>();
        iu = GameObject.FindGameObjectWithTag("InstructUI");
        player = GameObject.FindGameObjectWithTag("Player");
        ic = GameObject.FindGameObjectWithTag("InstructText").GetComponent<InstructController>();
        //Cursor.lockState = CursorLockMode.Locked;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        weaponCamera = GameObject.FindGameObjectWithTag("Weapon_Camera").GetComponent<Camera>();
        currentCount = maxBulletCount;
        butter = GameObject.Find("Butter");
        bullet_Slider = GameObject.Find("BulletCount_Slider").GetComponent<Slider>();
        if (bullet_Slider)
        {
            bullet_Slider.maxValue = maxBulletCount;
            bullet_Slider.value = currentCount / maxBulletCount * bullet_Slider.maxValue;
        } 
        setChildrenActive(false);
        findChildren("Weapon_01", Weapon.LASERGUN);
        gunIndex = 1;
        GrenadeStartSpeed = 20;
        currentgun = 1;
        lastgun = 4;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeWeapon();
        if(weaponType == WeaponType.GUN)
        {
            OpenFire();
        }   
        if (weaponType == WeaponType.Grenade)
        {
            gc = curWeapon.GetComponent<Grenade>();
            ThrowGrenade();
            if (isDown)
            {
                // Debug.Log("player:" + player);
                // Debug.Log("player controller:" + player.GetComponent<PlayerController>());
                // Debug.Log("eye trans:" + player.GetComponent<PlayerController>().eyeTrans);
                // Debug.Log(aimDirection);
                aimDirection = player.GetComponent<PlayerController>().eyeTrans.forward;
                gc.drawTrajectory(PlayerThrowPoint.transform.position, aimDirection);
                //Debug.Log(aimDirection);
            }
        }
        checkGrenadeStatus();
        ViewChange();

        if (bullet_Slider)
        {
            bullet_Slider.maxValue = maxBulletCount;
            bullet_Slider.value = currentCount / maxBulletCount * maxBulletCount;
        }

    }

    private void ChangeWeapon()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            curWeapon.gameObject.SetActive(false);
            weaponType = WeaponType.GUN;
            findChildren("hand_gun", Weapon.HANDGUN);
            if (currentgun != 0)
            {
                lastgun = currentgun;
                currentgun = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            curWeapon.gameObject.SetActive(false);
            weaponType = WeaponType.GUN;
            isWrong = false;
            findChildren("Weapon_01", Weapon.LASERGUN);
            GameObject.Find("Primary_Weapon").GetComponent<MeshRenderer>().material = materials[0];
            if (currentgun != 1)
            {
                lastgun = currentgun;
                currentgun = 1;
            }

        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            curWeapon.gameObject.SetActive(false);
            weaponType = WeaponType.GUN;
            findChildren("sciFIGun", Weapon.sciFIGun);
            if (currentgun != 2)
            {
                lastgun = currentgun;
                currentgun = 2;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            curWeapon.gameObject.SetActive(false);
            weaponType = WeaponType.Grenade;
            findChildren("Grenade01", Weapon.Grenade01);
            if (currentgun != 3)
            {
                lastgun = currentgun;
                currentgun = 3;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            curWeapon.gameObject.SetActive(false);
            weaponType = WeaponType.GUN;
            isWrong = true;
            findChildren("Weapon_01", Weapon.LASERGUN);
            GameObject.Find("Primary_Weapon").GetComponent<MeshRenderer>().material = materials[1];
            if (currentgun != 4)
            {
                lastgun = currentgun;
                currentgun = 4;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            //Debug.Log(lastgun);
            //Debug.Log(currentgun);
            ChangeWeaponByIndex(lastgun);
        }
    }
    private void ChangeWeaponByIndex(int index)
    {
        switch (index)
        {
            case 0:
                curWeapon.gameObject.SetActive(false);
                weaponType = WeaponType.GUN;
                findChildren("hand_gun", Weapon.HANDGUN);
                if (currentgun != 0)
                {
                    lastgun = currentgun;
                    currentgun = 0;
                }

                break;
            case 1:
                curWeapon.gameObject.SetActive(false);
                weaponType = WeaponType.GUN;
                isWrong = false;
                findChildren("Weapon_01", Weapon.LASERGUN);
                GameObject.Find("Primary_Weapon").GetComponent<MeshRenderer>().material = materials[0];
                if (currentgun != 1)
                {
                    lastgun = currentgun;
                    currentgun = 1;
                }
                break;
            case 2:
                curWeapon.gameObject.SetActive(false);
                weaponType = WeaponType.GUN;
                findChildren("sciFIGun", Weapon.sciFIGun);
                if (currentgun != 2)
                {
                    lastgun = currentgun;
                    currentgun = 2;
                }
                break;
            case 3:
                curWeapon.gameObject.SetActive(false);
                weaponType = WeaponType.Grenade;
                findChildren("Grenade01", Weapon.Grenade01);
                if (currentgun != 3)
                {
                    lastgun = currentgun;
                    currentgun = 3;
                }
                break;
            case 4:
                curWeapon.gameObject.SetActive(false);
                weaponType = WeaponType.GUN;
                isWrong = true;
                findChildren("Weapon_01", Weapon.LASERGUN);
                GameObject.Find("Primary_Weapon").GetComponent<MeshRenderer>().material = materials[1];
                if (currentgun != 4)
                {
                    lastgun = currentgun;
                    currentgun = 4;
                }
                break;
        }
    }
    void findChildren(string name,Weapon weapon)
    {
        if(weaponType == WeaponType.GUN)
        {
            myCurWeapon = weapon;
            curWeapon = GameObject.Find(findPath + name).GetComponent<Transform>();
            defaultPoint = GameObject.Find(findPath + name + "/DefaultPoint").GetComponent<Transform>();
            backPoint = GameObject.Find(findPath + name + "/BackPoint").GetComponent<Transform>();
            bulletStartPoint = GameObject.Find(findPath + name + "/BulletStartPoint").GetComponent<Transform>();
            //fireEffectPoint = GameObject.Find(findPath + name + "/FireEffectPoint").GetComponent<Transform>();
            weaponCameraDefaultPoint = GameObject.Find(findPath + name + "/WCDefault").GetComponent<Transform>().localPosition;
            weaponCameraCenterPoint = GameObject.Find(findPath + name + "/WCCenter").GetComponent<Transform>().localPosition;
            if (this.myCurWeapon == Weapon.LASERGUN)
                bullet = GameObject.Find(findPath + name + "/BlueTailBullet");
            else
                bullet = GameObject.Find(findPath + name + "/Bullet");
            shootAudio = GameObject.Find(findPath + name).GetComponent<AudioSource>();
            //reloadBulletAudio = bullet.GetComponent<AudioSource>();
            //fireEffect = GameObject.Find(findPath + name + "/Effect");
            curWeapon.gameObject.SetActive(true);
            switch (myCurWeapon)
            {
                case Weapon.HANDGUN:
                    fireInterval = 0.5f;
                    addDamage = 5;
                    fireRange = 50f;
                    break;
                case Weapon.sciFIGun:
                    fireInterval = 1.5f;
                    addDamage = 20;
                    fireRange = 100f;
                    break;
                case Weapon.LASERGUN:
                    fireInterval = 0.5f;
                    addDamage = 10;
                    fireRange = 50f;
                    break;

            }
          
        }
        if (weaponType == WeaponType.Grenade)
        {
            myCurWeapon = weapon;
            curWeapon = GameObject.Find(findPath + name).GetComponent<Transform>();
            defaultPoint = GameObject.Find(findPath + name + "/DefaultPoint").GetComponent<Transform>();
            backPoint = GameObject.Find(findPath + name + "/ThrowPoint").GetComponent<Transform>();
            //grenadeStartPoint = GameObject.Find("GrenadeStartPoint").GetComponent<Transform>();
            grenadeStartPoint = curWeapon.GetChild(0).GetChild(1);
            curWeapon.gameObject.SetActive(true);
            GrenadeWeapon = GameObject.Find("Grenade");
            // curWeapon.gameObject.GetComponent<Rigidbody>().useGravity = false;   
         }

    }

    void setChildrenActive(bool isactive)
    {
        Transform GunList = GameObject.Find("Player/Eye_View/Main Camera/Weapon_Root").GetComponent<Transform>();
        foreach(Transform childgun in GunList.transform)
        {
            childgun.gameObject.SetActive(isactive);
        }
    }
    private void ThrowGrenade()
    {
        if (weaponType == WeaponType.Grenade )
        {
            
            if (Input.GetMouseButtonDown(0))
            {
                StopCoroutine("GrenadeToDefault");
                StartCoroutine("GrenadeToBack");
                isDown = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                StopCoroutine("GrenadeToBack");
                StartCoroutine("GrenadeToDefault");
                gc.deleteTrajectory();
                isDown = false;
                if (!newGrenade)
                {
                    newGrenade = Instantiate(GrenadeWeapon.gameObject, grenadeStartPoint.position, grenadeStartPoint.rotation);
                    newGrenade.GetComponent<GrenadeController>().isExploded = false;
                    newGrenade.AddComponent<Rigidbody>();       
                    newGrenade.GetComponent<Rigidbody>().useGravity = true;
                    newGrenade.GetComponent<Rigidbody>().velocity = GrenadeStartSpeed * aimDirection.normalized;        
                    
                    //curWeapon.gameObject.SetActive(false);
                    //weaponType = WeaponType.GUN;
                    //findChildren("Weapon_01", Weapon.LASERGUN);
                    ChangeWeaponByIndex(1);
                }

            }

        }
        
    }

    public void checkGrenadeStatus()
    {
        if (newGrenade && newGrenade.GetComponent<GrenadeController>().isExploded == true)
        {
            GrenadeExplodeList = newGrenade.GetComponent<GrenadeController>().GetExplodeObjects();
            Destroy(newGrenade);
        }
    }
    
    private void OpenFire()
    {
       
        if(weaponType == WeaponType.GUN)
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
            if (allowFire && Input.GetMouseButtonDown(0))
            {
                isFire = true;
                if (coolDown)
                {
                    StartCoroutine("Fire");
                    coolDown = false;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                isFire = false;
                StopCoroutine("Fire");
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                StopCoroutine("ReloadBullet");
                StartCoroutine("ReloadBullet");
            }
        }
        

    }
    
    IEnumerator Fire()
    {
        while (isFire && weaponType == WeaponType.GUN)
        {
            if ((bulletStartPoint != null || bullet != null)&&currentCount>0)
            {
                RaycastHit hit;
                if (Physics.Raycast(ShootPoint.position, ShootPoint.forward, out hit, fireRange))
                {
                    bm.AttackObject(hit, isWrong);                           
                }
                GameObject newBullet = Instantiate(bullet, bulletStartPoint.position, bulletStartPoint.rotation);
                newBullet.GetComponent<Rigidbody>().velocity = -1 * newBullet.transform.forward * bulletStartSpeed;
                if (currentCount > 0)
                {
                    PlayShootAudio();
                    StopCoroutine("WeaponBackAnimation");
                    //Debug.Log("111111   "+ curWeapon.transform.localPosition);
                    StartCoroutine("WeaponBackAnimation");
                    //Debug.Log("222222   " + curWeapon.transform.localPosition);
                    currentCount -= 1;
                    if (bullet_Slider)
                    {
                        bullet_Slider.value = currentCount / maxBulletCount * bullet_Slider.maxValue;
                    }
                }
                Destroy(newBullet, 5);
            }
            yield return new WaitForSeconds(fireInterval);
        }
    }

    IEnumerator ReloadBullet()
    {
        //yield return new WaitForSeconds(waitTime);
        ReloadBulletAudio();
        while (!isFire&&currentCount<maxBulletCount)
        {
            var loadvalue = maxBulletCount / reloadTime * Time.deltaTime;
            currentCount += loadvalue;
            if (bullet_Slider)
            {
               
                bullet_Slider.value = (float)currentCount / maxBulletCount * bullet_Slider.maxValue;
            }
            yield return null;
        }
        
    }

    IEnumerator WeaponBackAnimation()
    {
        if(defaultPoint!=null&&backPoint!=null)
        {
            while (curWeapon.transform.localPosition!=backPoint.localPosition)
            {
                curWeapon.transform.localPosition = Vector3.Lerp(curWeapon.transform.localPosition, backPoint.localPosition, lerpRatio*4);
                // Debug.Log("1   " + curWeapon.transform.localPosition);
                yield return null;
            }
            while (curWeapon.transform.localPosition != defaultPoint.localPosition)
            {
                curWeapon.transform.localPosition = Vector3.Lerp(curWeapon.transform.localPosition, defaultPoint.localPosition,lerpRatio);
                // Debug.Log("2   " + curWeapon.transform.localPosition);
                yield return null;
            }

        }
    }

    IEnumerator GrenadeToBack()
    {
        if (defaultPoint != null && backPoint != null)
        {

            while (curWeapon.transform.localPosition != backPoint.localPosition)
            {
                curWeapon.transform.localPosition = Vector3.Lerp(curWeapon.transform.localPosition, backPoint.localPosition, lerpRatio * 4);
                yield return null;
            }           
        }
    }
    IEnumerator GrenadeToDefault()
    {
        if (defaultPoint != null && backPoint != null)
        {
            while (curWeapon.transform.localPosition != defaultPoint.localPosition)
            {
                curWeapon.transform.localPosition = Vector3.Lerp(curWeapon.transform.localPosition, defaultPoint.localPosition, lerpRatio);
                yield return null;
            }
        }
    }
    private void PlayShootAudio()
    {
        if(shootAudio)
        {
            shootAudio.Play();
        }
    }

    private void ReloadBulletAudio()
    {
        if (reloadBulletAudio)
        {
            reloadBulletAudio.Play();
        }
    }

    private void ViewChange()
    {
        if(Input.GetMouseButtonDown(1))
        {
            StopCoroutine("ViewToDefault");
            StartCoroutine("ViewToCenter");
        }
        if(Input.GetMouseButtonUp(1))
        {
            StopCoroutine("ViewToCenter");
            StartCoroutine("ViewToDefault");
        }
    }

    IEnumerator ViewToCenter()
    {
        while(weaponCamera.transform.localPosition!=weaponCameraCenterPoint)
        {
            weaponCamera.transform.localPosition = Vector3.Lerp(weaponCamera.transform.localPosition, weaponCameraCenterPoint, viewLerpRatio);
            weaponCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView,centerView,viewLerpRatio);
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, centerView, viewLerpRatio);
            yield return null;
        }
    }
    IEnumerator ViewToDefault()
    {
        while (weaponCamera.transform.localPosition != weaponCameraDefaultPoint)
        {
            weaponCamera.transform.localPosition = Vector3.Lerp(weaponCamera.transform.localPosition, weaponCameraDefaultPoint, viewLerpRatio);
            weaponCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, defaultView, viewLerpRatio);
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, defaultView, viewLerpRatio);
            yield return null;
        }
    }
    public List<RaycastHit> hitTarget()
    {
        return HitList;
    }

    public void clearHitList()
    {
        HitList.Clear();
    }

    public void AllowFire()
    {
        allowFire = !allowFire;
    }

}
