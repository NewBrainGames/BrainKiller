using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Prefab1;//获取预设体,然后在场景里自动生成
    public GameObject Prefab2;
    public GameObject Prefab3;
    public GameObject Prefab31;
    public GameObject Prefab21;

    // Start is called before the first frame update
    void Start()
    {
        //    Vector3[] arrays = new Vector3[5];

        //    for (int i = 0; i < 3; i++)
        //    {
        //        float nub = 43; //用来在3到5里面生成一个随机的整数，但是不包括5
        //        float nubs = 38;
        //        Vector3 a = new Vector3(nub, 0, nubs);
        //        Instantiate(Prefab1, a, Quaternion.identity);

        //        //transform.Rotate(Vector3.up * speed);
        //    }
        //}
       
        //Instantiate(Prefab1, new Vector3(8, 6.5f, 34.48f), Quaternion.identity);
        ////Prefab1.transform.Rotate(90,90,0);
        //Instantiate(Prefab2, new Vector3(8, 6.5f, 23.85f), Quaternion.identity);
        
        //Instantiate(Prefab3, new Vector3(8, 12.03f, 29.18f), Quaternion.identity);
        //Instantiate(Prefab31, new Vector3(8, 12.03f, 17.16f), Quaternion.identity);
        //Instantiate(Prefab21, new Vector3(8, 6.5f, 11.81f), Quaternion.identity);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
