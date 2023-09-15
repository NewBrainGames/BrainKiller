using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerInfo
{ 

}

public class AttributeController : MonoBehaviour
{
    //HP
    private float HP = 100F;
    private float maxHP = 100F;
    private Slider HP_Slider;

    //Intelligence
    private int infoAccessPower = 1;//ability to access infomation
    private int analysisPower = 1;//ability to analyze enemy weakness, meybe we can make the enemy weakness visualized if the value reach a certain standard 


    //Memory
    private int mapMemory = 1;//ability to memorize the map and the path

    //Perception
    private int visualPower = 1;//ability to discover sth
    private int auditoryPower = 1;//ability to hear sth
    private int spatialPower = 1;//ability to understand the space
    

    //Athletic Ability
    private int explosivePower = 1;//the ability to explode in a flash, associated with jumping and so on 
    private int stayingPower = 1;//sth determine the decreasing speed of energy value


    //Emotion
    private int energyValue = 10;//sth like HP, we can let the energyValue determine whether some actions like sprint can be performed
    private int angryValue = 0;//sth can add the enemtValue
    private int fearValue = 0;//sth can reduce the enemtValue and bring some other debuffs, but it can play an important role in certain situations Such as avoiding enemy attacks


    private bool isChanged = false;
    public GameObject botExplosion;
    // Start is called before the first frame update
    void Start()
    {
        GetAttribute();
        SaveAttribute();
        if(this.gameObject.tag == "Enemy")
        {
            HP = this.gameObject.GetComponent<EnemyController>().enemyHP;
            HP_Slider = GameObject.Find("EnemyHP_Slider").GetComponent<Slider>();
            maxHP = HP;
            if(HP_Slider)
            {
                HP_Slider.maxValue = HP;
                HP_Slider.value = HP/maxHP*HP_Slider.maxValue;
            }
            
            
        }
        else if(this.gameObject.tag == "Player")
        {
            HP = maxHP;
            if (GameConfig.Instance && GameConfig.Instance.GetGameMode() == Mode.battle001)
                HP_Slider = GameObject.Find("PlayerHP_Slider").GetComponent<Slider>();
            if (HP_Slider)
            {
                HP_Slider.maxValue = maxHP;
                HP_Slider.value = HP / maxHP * HP_Slider.maxValue;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isChanged)
        {
            SaveAttribute();
            isChanged = false;
        }
    }

    
    public void SaveAttribute()//save the attributes in the PlayerPrefs
    {
        PlayerPrefs.SetFloat("maxHP",this.maxHP);
        PlayerPrefs.SetInt("infoAccessPower", this.infoAccessPower);
        PlayerPrefs.SetInt("analysisPower", this.analysisPower);
        PlayerPrefs.SetInt("mapMemory", this.mapMemory); 
        PlayerPrefs.SetInt("visualPower", this.visualPower);
        PlayerPrefs.SetInt("auditoryPower", this.auditoryPower); 
        PlayerPrefs.SetInt("spatialPower", this.spatialPower);
        PlayerPrefs.SetInt("explosivePower", this.explosivePower);
        PlayerPrefs.SetInt("stayingPower", this.stayingPower);
        PlayerPrefs.SetInt("eneryValue", this.energyValue);
        PlayerPrefs.SetInt("angryValue", this.angryValue);
        PlayerPrefs.SetInt("fearValue", this.fearValue);
        PlayerPrefs.Save();
  

    }

    public void GetAttribute()
    {
        PlayerPrefs.GetInt("test", 1);
        this.maxHP = PlayerPrefs.GetFloat("maxHP",(float)100.0F);
        this.infoAccessPower = PlayerPrefs.GetInt("infoAccessPower",1);
        this.analysisPower  = PlayerPrefs.GetInt("analysisPower",1);
        this.mapMemory = PlayerPrefs.GetInt("mapMemory",1);
        this.visualPower = PlayerPrefs.GetInt("visualPower",1);
        this.auditoryPower = PlayerPrefs.GetInt("auditoryPower",1);
        this.spatialPower = PlayerPrefs.GetInt("spatialPower",1);
        this.explosivePower = PlayerPrefs.GetInt("explosivePower",1);
        this.stayingPower = PlayerPrefs.GetInt("stayingPower",1);
        this.energyValue = PlayerPrefs.GetInt("eneryValue",10);
        this.angryValue = PlayerPrefs.GetInt("angryValue",0);
        this.fearValue = PlayerPrefs.GetInt("fearValue",0);
    }

    public void DeleteAllAttribute()
    {
        // todo delete corresponding data
        PlayerPrefs.DeleteAll();
    }

    //HP 
    public void Damage(float damage)
    {
        if(HP>0)
        {
            HP -= damage;
            if (HP_Slider)
            {
                HP_Slider.value = HP / maxHP * HP_Slider.maxValue;
            }
        }
        if (HP <= 0)
        {
            //death;
            BotExplosion();
            if(this.gameObject.tag == "Enemy")
            {
                Vector3 enemyPosition = GameObject.Find("Enemy_02").transform.position;
                Destroy(this.gameObject);
                this.GetComponent<EnemyController>().ClearSlaveList();
                // winUI.SetActive(true);
            }
            else if (this.gameObject.tag == "Player")
            {
                
                Time.timeScale = 0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                GameObject.Find("Weapon_Root").GetComponent<Weapon_Controller>().currentCount = 0;
          
                
            }
        }

    }

    private void BotExplosion()
    {
        if(botExplosion)
        {
            GameObject newExplosion = Instantiate(botExplosion, this.transform.position,botExplosion.transform.rotation);
            Destroy(newExplosion, 1);
        }
    }

    public float GetHP()
    {
        return this.HP;
    }

    public void ChangeHP(float value)
    {
        HP += value;
        if (HP_Slider)
        {
            HP_Slider.value = HP / maxHP * HP_Slider.maxValue;
        }
    }

    public void SetHP(float value)
    {
        HP = value;
    }

    //Intelligence
    public int GetInfoAccessPower()//ability to access infomation
    {
        return infoAccessPower;
    }
    public void ChangeInfoAccessPower(int value)//change the value of infoAccessPower
    {
        infoAccessPower += value;
        isChanged = true;
    }

    public int GetAnalysisPower()//ability to analyze enemy weakness, meybe we can make the enemy weakness visualized if the value reach a certain standard 
    {
        return analysisPower;
    }

    public void ChangeAnalysisPower(int value)//change the value of analysisPower
    {
        analysisPower += value;
        isChanged = true;
    }

    //Memory
    public int GetmMapMemory()//ability to memorize the map and the path
    {
        return mapMemory;
    }
    public void ChangeMapMemory(int value)//ability to memorize the map and the path
    {
        mapMemory += value;
        isChanged = true;
    }

    //Perception
    public int GetVisualPower()//ability to discover sth
    {
        return visualPower;
    }

    public void ChangeVisualPower(int value)
    {
        visualPower += value;
        isChanged = true;
    }

    public int GetAuditoryPower()//ability to hear sth
    {
        return auditoryPower;
    }
    public void ChangeAuditoryPower(int value)
    {
        auditoryPower += value;
        isChanged = true;
    }
    public int GetSpatialPower()//ability to understand the space
    {
        return spatialPower;
    }
    public void ChangeSpatialPower(int value)
    {
        spatialPower += value;
        isChanged = true;
    }

    //Athletic Ability
    public int GetExplosivePower()//the ability to explode in a flash, associated with jumping and so on
    {
        return explosivePower;
    }

    public void ChangeExplosivePower(int value)
    {
        explosivePower += value;
        isChanged = true;
    }

    public int GetStayingPower()
    {
        return stayingPower;
    }
    public void ChangeStayingPower(int value)//sth determine the decreasing speed of energy value
    {
        stayingPower += value;
        isChanged = true;
    }


    //Emotion
    public int GetEnergyValue()//sth like HP, we can let the energyValue determine whether some actions like sprint can be performed
    {
        return energyValue;
    }
    public void ChangeEnergyValue(int value)
    {
        energyValue += value;
        isChanged = true;
    }
    public int GetAngryValue()//sth can add the enemtValue
    {
        return angryValue;
    }
    public void ChangeAngryValue(int value)
    {
        angryValue += value;
        isChanged = true;
    }
    public int GetFearValue()//sth can reduce the enemtValue and bring some other debuffs, but it can play an important role in certain situations Such as avoiding enemy attacks
    {
        return fearValue;
    }
    public void ChangeFearValue(int value)
    {
        fearValue += value;
        isChanged = true;
    }
}
