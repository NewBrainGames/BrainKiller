using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSkyBox : MonoBehaviour
{
    public Material[] mat;
    private Skybox skyBox;
    public static bool flags = false;
    private int divider = 2;
    // Start is called before the first frame update
    void Start()
    {
        skyBox = GetComponent<Skybox>();
    }

    public void setSkyBox()
    {
        if (GameConfig.Instance)
        {
            switch (GameConfig.Instance.GetGameMode())
            {
                case Mode.Dendrite:
                    skyBox.material = mat[Random.Range(0, divider)];
                    break;
                case Mode.Axon:
                    skyBox.material = mat[Random.Range(divider, mat.Length)];
                    break;
            } 
        }
        else
        {
            skyBox.material = mat[Random.Range(0, mat.Length)];
        }
        
    }
}
