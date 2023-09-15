using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    public float rotateSpeed = 180;
    [Range(1,2)]
    public float rotateRatio = 1;
    public Transform playerTrans;
    public Transform eyeTrans;
    private float x_RotateOffset;
    public float x_limit = 60;

    public CharacterController _playerCC;
    public float moveSpeed = 5;
    private float addSpeed = 3f;
    private bool accelerated = false;
    private float _gravity = -19.8f;
    public float v_Velocity = 0;
    public bool isGround = false;
    public Transform groundCheckPoint;
    public float checkSphereRadius = 1.2f;
    public LayerMask groundLayer;
    public float maxHeight = 5;
    public bool firstTime = true;

    //JetPack
    public AudioSource jetAudio;
    public bool haveJetPack = false;
    public bool useJetPack = false;
    public bool stayFloat = false;

    //low gravity mode
    public bool lowGravity = false;

    //Animator
    public HoverbotAnimatorController aniController;

    public GameObject enemy;
    public GameObject IntrouductionUI;
    public GameObject CoinUI;

    public float damage = 10;
    public float HP;
    public Slider HP_Slider;
    public BattleManager bm;
    public AttributeController ac;
    private bool attributeChanged = false;
    private bool showMap = false;

    void Start()
    {
        //Debug.Log("PlayerController");
        Physics.autoSyncTransforms = true;//ljs add
        _playerCC = this.GetComponent<CharacterController>();
        aniController = this.GetComponent<HoverbotAnimatorController>();
        bm.RoleRegister(this.gameObject);
        ac = this.GetComponent<AttributeController>();
        HP = ac.GetHP();
        if(bm.gameMode == Mode.battle001)
            HP_Slider = GameObject.Find("PlayerHP_Slider").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerControlObject();//control the image
        LowGravityControl();
        if (_playerCC.transform.position.y < -20.0f)
        {
            ac.SetHP(0);
        }
        if(ac.GetAngryValue()>0f)
        {
            addSpeed = 2f;
        }
        if(attributeChanged)
        {
            ac.SaveAttribute();
            attributeChanged = false;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            showMap = !showMap; 
            if (showMap)
            {
                useMainCamera(false);
            }
            else
            {
                useMainCamera(true);
            }
        }

    }

    private void useMainCamera(bool use)
    {
        Camera mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        Camera mapCamera = GameObject.Find("minimapcur").GetComponent<Camera>();
        mapCamera.enabled = !use;
        mainCamera.enabled = use;
    }

    private void FixedUpdate()
    {
        PlayerRotateControl();
        PlayerMovement();
    }

    private void LowGravityControl()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (lowGravity == false)
            {
                this._gravity = -1;
                lowGravity = true;
            }
            else
            {
                this._gravity = -19.8f;
                lowGravity = false;
            }
        }
    }
    //
    private void PlayerRotateControl()
    {
        if(playerTrans == null||eyeTrans == null)
        {
            return;
        }
        float offset_x = Input.GetAxis("Mouse X");
        float offset_y = Input.GetAxis("Mouse Y");
        //Debug.Log(offset_y);
        playerTrans.Rotate(Vector3.up*rotateSpeed*rotateRatio*Time.fixedDeltaTime*offset_x);
        x_RotateOffset -= offset_y * rotateSpeed * rotateRatio * Time.fixedDeltaTime;
        x_RotateOffset = Mathf.Clamp(x_RotateOffset,-x_limit, x_limit);
        Quaternion currentLocalRotation = Quaternion.Euler(new Vector3(x_RotateOffset, eyeTrans.localEulerAngles.y, eyeTrans.localEulerAngles.z));

        eyeTrans.localRotation = currentLocalRotation;
    }

    private void PlayerControlObject()
    {
        if(playerTrans == null||eyeTrans == null)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.K) && (bm.gameMode == Mode.Dendrite || bm.gameMode == Mode.Axon))
        {
            bm.toggleRawImage();
        }
        if (Input.GetKeyUp(KeyCode.F) && (bm.gameMode == Mode.Dendrite || bm.gameMode == Mode.Axon))
        {
            bm.toggleRawSwc();
        }

    }

    private void PlayerMovement()
    {
        if (_playerCC == null)
            return;
        Vector3 motionValue = Vector3.zero;
        float h_InputValue = Input.GetAxis("Horizontal");//左右
        float v_InputValue = Input.GetAxis("Vertical");//前后
        motionValue += this.transform.forward * moveSpeed * v_InputValue * Time.fixedDeltaTime;
        motionValue += this.transform.right * moveSpeed * h_InputValue * Time.fixedDeltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift)&&accelerated == false)
        {
            moveSpeed += addSpeed;
            accelerated = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)&&accelerated == true)
        {
            moveSpeed -= addSpeed;
            accelerated = false;
        }

        //地面检测
        if (groundCheckPoint != null)
        {
            isGround = Physics.CheckSphere(groundCheckPoint.position, checkSphereRadius, groundLayer);
            if (isGround && v_Velocity < 0)
            {
                isGround = true;
                v_Velocity = 0;
            }
        }
        if (isGround)
        {
            if (Input.GetButtonDown("Jump"))
            {
                v_Velocity = -_gravity * Mathf.Sqrt((2 * maxHeight) / -_gravity);
            }

        }

        if (haveJetPack)
        {
           if (Input.GetKeyDown(KeyCode.G))
           {
               if (useJetPack == false)
                   useJetPack = true;
                else if (useJetPack == true)
                   useJetPack = false;
            }
            if (Input.GetButtonDown("Jump") && useJetPack)
            {
                v_Velocity += 1f;
                stayFloat = false;
                jetAudio.Play();
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (stayFloat == false)
                    stayFloat = true;
                else
                    stayFloat = false;

            }

        }

        if (!useJetPack)
        {
            v_Velocity += _gravity * Time.fixedDeltaTime;
            motionValue += Vector3.up * v_Velocity * Time.fixedDeltaTime;
        }
        else
        {
            if (stayFloat == true)
            {
                v_Velocity = 0;

            }
            v_Velocity += 0.5f * _gravity * Time.fixedDeltaTime;
            motionValue += Vector3.up * v_Velocity * Time.fixedDeltaTime;
        }

        _playerCC.Move(motionValue);

        //给动画参数赋值
        if (aniController)
        {
            aniController.moveSpeed = moveSpeed * v_InputValue;
            aniController.alerted = v_InputValue == 0 ? false : true;

        }
    }

    public void OnDamage()
    {
        aniController.TriggerOnDamage();
    }

    private void ChangeAttribute()
    {

    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "PickUpHealth")
        {
            this.GetComponent<AttributeController>().ChangeHP(50);
            ac.ChangeAngryValue(1);
            attributeChanged = true;
        }
        if (col.gameObject.tag == "PickUpJetPack")
        {
            haveJetPack = true;
            jetAudio = this.gameObject.AddComponent<AudioSource>();
            jetAudio.clip = (AudioClip)Resources.Load("Assets//FPS//Audio//PlayerAudio//JetPackPush",typeof(AudioClip));
            
        }
        //if (col.gameObject.tag == "present")
        //{
        //    int ranNum = Random.Range(0, 2);
        //    if (ranNum == 0)
        //    {
        //        this.moveSpeed += addSpeed;
        //    }
        //    if (ranNum == 1)
        //    {
        //        this._gravity -= 0.5f;
        //    }
        //}
    }
}
