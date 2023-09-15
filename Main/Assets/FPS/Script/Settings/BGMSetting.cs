using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
// using UnityEngine.UIElements;
using UnityEngine.UI;

public class BGMSetting : MonoBehaviour
{
    public Toggle switchToggle;
    public Slider volumeSlider;
    
    // Start is called before the first frame update
    void Start()
    {
        // initial setting music switch and volume
        if (switchToggle != null && GameConfig.Instance)
        {
            switchToggle.isOn = GameConfig.Instance.BGMOn;
        }

        if (volumeSlider != null && GameConfig.Instance)
        {
            volumeSlider.value = GameConfig.Instance.BGMVolume;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // switch value change
        if (switchToggle.isOn != GameConfig.Instance.BGMOn)
        {

            GameConfig.Instance.BGMOn = switchToggle.isOn;
        }

        if (Math.Abs(volumeSlider.value - GameConfig.Instance.BGMVolume) > 0.01f)
        {
            GameConfig.Instance.BGMVolume = volumeSlider.value;

        }
    }
}
