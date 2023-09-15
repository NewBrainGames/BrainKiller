using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepScript : MonoBehaviour
{

    public GameObject footStep;

    public GameObject sprintStep;

    public GameObject jumpStep;

    private PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        if (footStep == null)
        {
           footStep = GameObject.Find("footStep"); 
        }
        if (sprintStep == null)
        {
           sprintStep = GameObject.Find("sprintStep"); 
        }
        if (jumpStep == null)
        {
           jumpStep = GameObject.Find("jumpStep"); 
        }
        footStep.SetActive(false);
        sprintStep.SetActive(false);
        jumpStep.SetActive(false);
        player = this.GetComponent<PlayerController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isGround&&(Input.GetKeyDown("w") || Input.GetKeyDown("s") || Input.GetKeyDown("a") || Input.GetKeyDown("d")))
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                SprintSteps();
            }
            else
            {
                FootSteps();
            }

        }else if (player.isGround && Input.GetKeyDown(KeyCode.Space))
        {
            JumpSteps();
        }else if (player.isGround && Input.GetKeyUp("w") || Input.GetKeyUp("s") || Input.GetKeyUp("a") || Input.GetKeyUp("d") || Input.GetKeyUp(KeyCode.Space)|| Input.GetKeyUp(KeyCode.LeftShift))
        {
            stopFootSteps();
        }
        
    }

    void FootSteps()
    {
        footStep.SetActive(true);
        sprintStep.SetActive(false);
        jumpStep.SetActive(false);
    }

    void SprintSteps()
    {
        footStep.SetActive(false);
        sprintStep.SetActive(true);
        jumpStep.SetActive(false);
    }
    
    void JumpSteps()
    {
        footStep.SetActive(false);
        sprintStep.SetActive(false);
        jumpStep.SetActive(true);
    }
    void stopFootSteps()
    {
        footStep.SetActive(false);
        sprintStep.SetActive(false);
        jumpStep.SetActive(false);
        
    }
}
