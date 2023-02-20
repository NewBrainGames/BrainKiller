using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial_Practice : MonoBehaviour
{
    // Start is called before the first frame update

    public Material[] mat;
    public GameObject roof;
    public GameObject ground;
    private Renderer rend1;
    private Renderer rend2;
    private int index;
    public static bool changeSceneflag = false;

    void Start()
    {
        
        index = 0;
        ground = GameObject.FindGameObjectWithTag("CircleGround");
        rend1 = roof.GetComponent<Renderer>();
        rend1.enabled = true;
        rend1.sharedMaterial = mat[index];
        // rend2 = ground.GetComponent<Renderer>();
        // rend2.enabled = true;
        // rend2.sharedMaterial = mat[index];
    }

    // public class NormalMaterialInfo
    // {
    //     public string objectName;
    //     public Material[] objectMaterials;
    // }
    
    // Update is called once per frame
    void Update()
    {
        rend1.sharedMaterial = mat[index];
        // rend2.sharedMaterial = mat[index];


    }

    public void NextMaterial()
    {
        if (changeSceneflag == true)
        {
            index++;
        }

        changeSceneflag = false;

    }
}
