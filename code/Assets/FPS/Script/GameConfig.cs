using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mode
{
    Dendrite,   
    battle001,
    Axon,
};

public class GameConfig : MonoBehaviour
{
    // 存放全局配置信息
    public static GameConfig Instance { get; private set; }

    public bool BGMOn { get; set; } = true;
    public float BGMVolume { get; set; } = 0.5f;

    private Mode gameMode;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectDendriteMode()
    {
        this.gameMode = Mode.Dendrite;
    }
    
    public void SelectBattleMode()
    {
        this.gameMode = Mode.battle001;
    }

    public void SelectAxonMode()
    {
        this.gameMode = Mode.Axon;
    }
    
    public Mode GetGameMode()
    {
        return gameMode;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(BGMOn.ToString());
        // Debug.Log( BGMVolume.ToString());
    }
}
