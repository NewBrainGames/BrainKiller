using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    // 单例模式instance
    // private static BGMPlayer _instance;
    // public static BGMPlayer Instance => _instance;

    private AudioSource bgMusic;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ChangeVolume(GameConfig.Instance.BGMVolume);
    }

    // 音乐播放状态控制
    public void Awake()
    {
        // _instance = this;

            bgMusic = this.GetComponent<AudioSource>();
            if (bgMusic == null)
            {
                Debug.Log("bgm audio source not found");
            }

            if (GameConfig.Instance != null)
            {
                PlayMusic(GameConfig.Instance.BGMOn);
                ChangeVolume(GameConfig.Instance.BGMVolume);
            }
            

    }

    public void PlayMusic(bool isOpen)
    {
        // Debug.Log("play music" + isOpen.ToString());
        if (isOpen)
        {
            bgMusic.Play();
        }
        else
        {
            bgMusic.Stop();
            // bgMusic.Pause();
        }
    }
    
    // 音量控制
    public void ChangeVolume(float volume)
    {
        bgMusic.volume = volume;
    }
}
