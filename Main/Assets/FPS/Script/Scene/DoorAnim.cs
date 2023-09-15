using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnim : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject cube;
    private Animation ani;
    public static bool doorAni = false;
    void Start()
    {
        cube = GameObject.Find("DoorAni");
        ani = GetComponent<Animation>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (SceneFadeInOut.doorAni == true && Input.GetKeyDown(KeyCode.K))
        //{
        //    //cube.GetComponent<Animation>.Play()
        //    ani.Play("door");
        //    Debug.Log("play ani");
        //    SceneFadeInOut.doorAni = false;
        //}
        if(SceneFadeInOut.CoinAndNextImage == true)
        {
            ResetAni(ani, "Door1");
            SceneFadeInOut.CoinAndNextImage = false;
        }
    }

    public void ResetAni(Animation ani, string name)
    {
        AnimationState state = ani[name];
        ani.Play(name);
        state.time = 0;
        ani.Sample();
        state.enabled = false;
        doorAni = false;

    }

    public void openDoor()
    {
        ani.Play("Door1");
        doorAni = true;
    }


}
