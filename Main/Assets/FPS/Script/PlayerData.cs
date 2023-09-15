using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityVolumeRendering;


public struct Record
{
    public string GameMode;
    public string ImageId;
    public int Score;
    public int TotalCheckingPoints;
    public int TotalSwcLength;
    public int TotalGeneratedCheckingPoints;
}

internal class DailyGameRecord
{
    public string UserId;
    public int[] indexes;
    public int[] axonIndexes;
    public List<Record> Records;
    public List<Record> axonRecords;
}

public class PlayerData : MonoBehaviour
{
    // 存放全局配置信息
    public static PlayerData Instance { get; private set; }
    
    private string localArchivePath;
    // daily record save path
    private string dailyRecordPath;
    private string basicRecordPath;

    // practise 模式相关信息
    private const string Key_PracticeLevelId = "practise_level_id";

    // Start is called before the first frame update
    void Start()
    {
        localArchivePath = Application.streamingAssetsPath;
        basicRecordPath = localArchivePath + "/DailyRecord";
        dailyRecordPath = basicRecordPath + "_" + PlayerPrefs.GetString("username", "Guest") + ".json";
    }

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

    public void initDailyGameRecord(string userId, ref int[] i1, ref int[] i2)
    {
        
        //Debug.Log(dailyRecordPath);
        if (!File.Exists(dailyRecordPath))
        {
            DailyGameRecord dailyGameRecord = new DailyGameRecord
            {
                UserId = userId,
                indexes = i1,
                axonIndexes = i2,
                Records = new List<Record>(),
                axonRecords = new List<Record>()
            }; 
            
            string recordString = JsonConvert.SerializeObject(dailyGameRecord);
            Debug.Log("init record" + recordString);

            try
            {
                File.WriteAllText(dailyRecordPath, recordString);
                Debug.Log("record saved succeed");
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
                Debug.Log("record saved error");
                throw;
            }
        }
        else
        {
            var jsonString = File.ReadAllText(dailyRecordPath);
            DailyGameRecord dailyGameRecord = JsonConvert.DeserializeObject<DailyGameRecord>(jsonString);
            i1 = dailyGameRecord.indexes;
            i2 = dailyGameRecord.axonIndexes;
        }
    }

    // practice game record
    public void AddDailyGameRecord(string userId, Record record, Mode gameMode = Mode.Dendrite, int[] indexes = null)
    {
        // Debug.Log("daily_record_path:" + dailyRecordPath);
        if (!File.Exists(dailyRecordPath))
        {
            // new daily record
            DailyGameRecord dailyGameRecord = new DailyGameRecord
            {
                UserId = userId,
                Records = new List<Record> { record },
                axonRecords = new List<Record> {record}
            };
            
            if (gameMode == Mode.Dendrite)
            {
                print("set den index");
                dailyGameRecord.indexes = indexes;
            }else if (gameMode == Mode.Axon)
            {
                print("set axon index");
                dailyGameRecord.axonIndexes = indexes;
            }
            // save file
            string recordString = JsonConvert.SerializeObject(dailyGameRecord);
            Debug.Log("serialize record" + recordString);

            try
            {
                File.WriteAllText(dailyRecordPath, recordString);
                Debug.Log("record saved succeed");
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
                Debug.Log("record saved error");
                throw;
            }
        }
        else
        {
            // read file and to object
            var jsonString = File.ReadAllText(dailyRecordPath);
            DailyGameRecord dailyGameRecord = JsonConvert.DeserializeObject<DailyGameRecord>(jsonString);
            if (dailyGameRecord != null && dailyGameRecord.UserId == userId)
            {
                if (gameMode == Mode.Dendrite)
                {
                    print("set den index");
                    dailyGameRecord.indexes = indexes;
                    dailyGameRecord.Records.Add(record);
                }else if (gameMode == Mode.Axon)
                {
                    print("set axon index");
                    dailyGameRecord.axonIndexes = indexes;
                    dailyGameRecord.axonRecords.Add(record);
                }
                
                // save file
                string recordString = JsonConvert.SerializeObject(dailyGameRecord);
                Debug.Log("serialize record" + recordString);

                try
                {
                    File.WriteAllText(dailyRecordPath, recordString);
                    Debug.Log("record saved succeed");
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e);
                    Debug.Log("record saved error");
                    throw;
                }
                
            }
        }
    }

    public void setIndexes(Mode gameMode, int[] indexes)
    {
        
    }
    public int[] GetRandomImageIndexes()
    {
        // Debug.Log("daily_record_path:" + dailyRecordPath);
        if (!File.Exists(dailyRecordPath))
        {
            Debug.Log("Daily Game Record not found");
            return null;
        }

        var jsonString = File.ReadAllText(dailyRecordPath);
        DailyGameRecord dailyGameRecord = JsonConvert.DeserializeObject<DailyGameRecord>(jsonString);
        return dailyGameRecord != null ? dailyGameRecord.indexes : null; 
    }
    
    public int[] GetAxonRandomImageIndexes()
    {
        // Debug.Log("daily_record_path:" + dailyRecordPath);
        if (!File.Exists(dailyRecordPath))
        {
            Debug.Log("Daily Game Record not found");
            return null;
        }

        var jsonString = File.ReadAllText(dailyRecordPath);
        DailyGameRecord dailyGameRecord = JsonConvert.DeserializeObject<DailyGameRecord>(jsonString);
        return dailyGameRecord != null ? dailyGameRecord.axonIndexes : null; 
    }

    public List<Record> GetDailyGameRecord()
    {
        // Debug.Log("daily_record_path:" + dailyRecordPath);
        if (!File.Exists(dailyRecordPath))
        {
            Debug.Log("Daily Game Record not found");
            return null;
        }

        var jsonString = File.ReadAllText(dailyRecordPath);
        DailyGameRecord dailyGameRecord = JsonConvert.DeserializeObject<DailyGameRecord>(jsonString);
        return dailyGameRecord != null ? dailyGameRecord.Records : null;
    }


    public void ClearDailyGameRecord()
    {
        Debug.Log("Clear Daily Game Record");
        if (File.Exists(dailyRecordPath))
        {
            File.Delete(dailyRecordPath);
        }
    }
    

    public int GetDenLevelId()
    {
        if (!File.Exists(dailyRecordPath))
        {
            Debug.Log("Daily Game Record not found");
            return -1;
        }

        var jsonString = File.ReadAllText(dailyRecordPath);
        DailyGameRecord dailyGameRecord = JsonConvert.DeserializeObject<DailyGameRecord>(jsonString);
        return dailyGameRecord != null ? dailyGameRecord.Records.Count : -1;
    }
    
    public int GetAxonLevelId()
    {
        if (!File.Exists(dailyRecordPath))
        {
            Debug.Log("Daily Game Record not found");
            return -1;
        }

        var jsonString = File.ReadAllText(dailyRecordPath);
        DailyGameRecord dailyGameRecord = JsonConvert.DeserializeObject<DailyGameRecord>(jsonString);
        return dailyGameRecord != null ? dailyGameRecord.axonRecords.Count : -1;
    } 
    
    

    public void RemovePracticeLevelId()
    {
        if (PlayerPrefs.HasKey(Key_PracticeLevelId))
        {
            PlayerPrefs.DeleteKey(Key_PracticeLevelId);
            // PlayerPrefs.DeleteAll();
        }
    }

    // 删除所有的key
    public void ClearAllData()
    {
        PlayerPrefs.DeleteAll();
    }

    public int GetExperience()
    {
        // Debug.Log("daily_record_path:" + dailyRecordPath);
        if (!File.Exists(dailyRecordPath))
        {
            return 0;
        }

        var jsonString = File.ReadAllText(dailyRecordPath);

        DailyGameRecord dailyGameRecord = JsonConvert.DeserializeObject<DailyGameRecord>(jsonString);
        int count = 0;
        int dendritemax = 0;
        int axonmax = 0;
        foreach(Record var in dailyGameRecord.Records)
        {
            if (var.Score > dendritemax)
                dendritemax = var.Score;
        }
        foreach (Record var in dailyGameRecord.axonRecords)
        {
            if (var.Score > axonmax)
                axonmax = var.Score;
        }
        count = dendritemax + axonmax;
        return count*10;
    }
}