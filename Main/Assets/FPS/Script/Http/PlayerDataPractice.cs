using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;



public class PlayerDataPractice : MonoBehaviour
{
    
    public BattleManager bm;
    private static String getBpUrl = "http://139.155.28.154:26000/dynamic/game/swc/bppoint/insert";
    
    // Start is called before the first frame update
    void Start()
    {
        // bm = GameObject.FindGameObjectWithTag("BM").GetComponent<BattleManager>();

    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public class InsertBranchingPoint
    {
        public User user = new User();
        public SwcDetail sd = new SwcDetail();

    }

    public class User
    {
        public string name;
        public string passwd;
    }

    public class SwcDetail
    {
        public string swcid;
        public int points;
        public string missedbp;

        public string wrongbp;

        public string correctbp;
    }

    public String ProcessPlayerData(String annotationInfo)
    {
        bm = GameObject.FindGameObjectWithTag("BM").GetComponent<BattleManager>();
        if (annotationInfo != null)
        {
            Debug.Log("annotationInfo: " + annotationInfo);
            User user = new User();
            user.name = PlayerPrefs.GetString("username");
            user.passwd = PlayerPrefs.GetString("password");
            SwcDetail swcDetail = new SwcDetail();
            int v_index = annotationInfo.IndexOf("v:");
            swcDetail.swcid = annotationInfo.Substring(0, v_index).TrimEnd('\n').Trim(' ');
            String tempBp = annotationInfo.Substring(v_index);
            int i_index = tempBp.IndexOf("i");
            String correctbp = tempBp.Substring(2, i_index-2).TrimEnd((char[])"\n".ToCharArray());
            swcDetail.correctbp = (correctbp == null) ? "" : correctbp;
            
            
            String temp = tempBp.Substring(i_index+2);
            int m_index = temp.IndexOf("m");
            String wrongbp = temp.Substring(0, m_index).TrimEnd((char[])"\n".ToCharArray());
            swcDetail.wrongbp = (wrongbp == null) ? "" : wrongbp;
            
            String missedbp = temp.Substring(m_index + 2).TrimEnd((char[])"\n".ToCharArray());

            swcDetail.missedbp = (missedbp == null) ? "" : missedbp;
            int usedSeconds = 600 - TimeCounter.totalTime;
            if (usedSeconds > 0)
            {
                swcDetail.points = usedSeconds;
            }else
            {
                swcDetail.points = -1;
            }
            // swcDetail.points = bm.getScore();
            InsertBranchingPoint insertBranchingPoint = new InsertBranchingPoint();
            insertBranchingPoint.user = user;
            insertBranchingPoint.sd = swcDetail;
            String jsonData = JsonConvert.SerializeObject(insertBranchingPoint);
            return jsonData;
            
            
        }

        return "";
        
    }

    
    public void Post(String annotationInfo)
    {
        String jsonData = ProcessPlayerData(annotationInfo);
        // Debug.Log("jsonData" + jsonData);
        StartCoroutine(PostRequest(getBpUrl,jsonData));
    }

    public IEnumerator PostRequest(string url, string jsonData)
    {
        // Debug.Log("url" + url);
        Debug.Log("jsonData" + jsonData);
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            yield return request.SendWebRequest();

            if(request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Upload successfully!");
            }

        }

        yield return null;

    }


    
    
    

    
    
    
    
    
    
    
    
    
    
    
    
    
}
