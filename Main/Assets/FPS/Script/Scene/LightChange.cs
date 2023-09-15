using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class LightChange : MonoBehaviour
{

    private Light lightq;
    // Start is called before the first frame update
    public static int num = 0;

    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (SceneFadeInOut.changeLight == true)
        {
            lightq = this.GetComponent<Light>();
            //Debug.Log(num);
            if (num == 0)
            {
                lightq.color = new Color(255 / 255f, 255 / 255f, 255 / 255f);//早上
                num++;
            }
            else if (num == 1)
            {
                lightq.color = new Color(126 / 255, 66 / 255, 120 / 255);//晚上
                num++;

            }
            else if (num == 2)
            {
                lightq.color = new Color(108 / 255, 177 / 255, 199 / 255);//黄昏
                num++;
            }
            else if (num >= 3)
            {
                lightq.color = new Color(0 / 255, 0 / 255, 0 / 255);//晚上
                num = 0;
            }
            SceneFadeInOut.changeLight = false;

        }
    }
}
