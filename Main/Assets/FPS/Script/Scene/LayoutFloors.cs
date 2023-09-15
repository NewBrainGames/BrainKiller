using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LayoutFloors : MonoBehaviour
{
    // change leaf color
    public Vector3 cellCount = Vector3.zero;
    public Vector3 floorSize = Vector3.zero;
    public List<Transform> floorTransforms = new List<Transform>();

    //change light color
    //private Light lightq;
    //public static int num = 0;

    //change skybox
    //public Material[] mat;
    //private Skybox skyBox;
    //public static bool flags = false;


    void Start()
    {
        //mat = new Material[5];
        
        //mat[0] = new Material(Shader.Find("Assets/FREE Skyboxes - Sci-Fi & Fantasy/SBS Sci-Fi & Fantasy 1/Large/Skybox_Sci-Fi & Fantasy 1 Large.mat"));
        //mat[1] = new Material(Shader.Find("Assets/FREE Skyboxes - Sci-Fi & Fantasy/SBS Sci-Fi & Fantasy 2/Large/Skybox_SBS Sci-Fi & Fantasy 2 Large.mat"));
        //mat[2] = new Material(Shader.Find("Assets/FREE Skyboxes - Sci-Fi & Fantasy/SBS Sci-Fi & Fantasy 3/Large/Skybox_SBS Sci-Fi & Fantasy 3 Large.mat"));
        //mat[3] = new Material(Shader.Find("Assets/FREE Skyboxes - Sci-Fi & Fantasy/SBS Sci-Fi & Fantasy 4/Large/Skybox_SBS Sci-Fi & Fantasy 4 Large.mat"));
        //mat[4] = new Material(Shader.Find("Assets/FREE Skyboxes - Sci-Fi & Fantasy/SBS Sci-Fi & Fantasy 5/Large/Skybox_SBS Sci-Fi & Fantasy 5 Large.mat"));
        //skyBox = GetComponent<Skybox>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (flags == true)
        //{
        //    if (LightChange.num == 0)
        //    {
        //        skyBox.material = mat[Random.RandomRange(0, mat.Length)];
        //    }
        //    else if (LightChange.num == 1)
        //    {
        //        skyBox.material = mat[Random.RandomRange(0, mat.Length)];
        //    }
        //    else if (LightChange.num == 2)
        //    {
        //        skyBox.material = mat[Random.RandomRange(0, mat.Length)];
        //    }
        //    else
        //    {
        //        skyBox.material = mat[Random.RandomRange(0, mat.Length)];
        //    }
        //    flags = false;
        //}

        if (SceneFadeInOut.changeLight == true)
        {
        //    lightq = this.GetComponent<Light>();
        //    //Debug.Log(num);
        //    if (num == 0)
        //    {
        //        lightq.color = new Color(255 / 255f, 255 / 255f, 255 / 255f);//早上
        //        num++;
        //    }
        //    else if (num == 1)
        //    {
        //        lightq.color = new Color(126 / 255, 66 / 255, 120 / 255);//晚上
        //        num++;

        //    }
        //    else if (num == 2)
        //    {
        //        lightq.color = new Color(108 / 255, 177 / 255, 199 / 255);//黄昏
        //        num++;
        //    }
        //    else if (num >= 3)
        //    {
        //        lightq.color = new Color(0 / 255, 0 / 255, 0 / 255);//晚上
        //        num = 0;
        //    }

            //add if to save time
            foreach (Transform child in this.transform)
            {
                if (child.tag == "Tree" && LightChange.num == 0)
                {
                    //Debug.Log("children: " + child.name);
                    // child.Mater
                    //Material rendArray = child.GetComponent<Material>();
                    MeshRenderer mrender = child.GetComponent<MeshRenderer>();
                    Material[] array = mrender.materials;
                    array[1].color = Color.red;


                }
                else if (child.tag == "Tree" && LightChange.num == 1)
                {
                    MeshRenderer mrender = child.GetComponent<MeshRenderer>();
                    Material[] array = mrender.materials;
                    array[1].color = Color.green;
                }
                else if (child.tag == "Tree" && LightChange.num == 2)
                {
                    MeshRenderer mrender = child.GetComponent<MeshRenderer>();
                    Material[] array = mrender.materials;
                    array[1].color = Color.white;
                }
                else if (child.tag == "Tree" && LightChange.num == 3)
                {
                    MeshRenderer mrender = child.GetComponent<MeshRenderer>();
                    Material[] array = mrender.materials;
                    array[1].color = new Color(255 / 255, 25 / 255, 0 / 255);
                }

            }

            SceneFadeInOut.changeLight = false;

        }



        [ContextMenu("Reset Floor Layout")]
        void ResetFloorLayout()
        {
            floorTransforms.Clear();
            var childrenTransforms = this.GetComponentsInChildren<Transform>();
            for (int i = 1; i < childrenTransforms.Length; i++)
            {
                if (childrenTransforms[i].gameObject.CompareTag("floor"))
                {
                    floorTransforms.Add(childrenTransforms[i]);
                }

            }
            int currentIndex = 0;
            for (int x = 0; x < cellCount.x; x++)
            {
                for (int z = 0; z < cellCount.z; z++)
                {
                    Vector3 localPos = new Vector3(x * floorSize.x, 0, z * floorSize.z);
                    floorTransforms[currentIndex].localPosition = localPos;
                    currentIndex++;
                }
            }
        }

    }
}