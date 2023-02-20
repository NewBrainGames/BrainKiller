using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwcMiniCamera : MonoBehaviour
{
    private Camera minimap;
    private GameObject swccurrent;
    // Start is called before the first frame update
    void Start()
    {
        minimap = this.GetComponent<Camera>();
        swccurrent = GameObject.Find("swccurrent");
    }

    // Update is called once per frame
    //void Update()
    //{
    //    minimap.transform.position = swccurrent.transform.position + 10 * Vector3.back;
    //}
}
