// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Networking;
//
// public class HttpTool : MonoBehaviour
// {
//     pulic delegate void HttpHelperPostGetCallbacks(long code,HttpHelperRequests request,HttpHelperResponses response);
//
//     [Serializable]
//     public struct HttpHelperRequests
//     {
//         public string url;
//         public JsonData jsonDATA;
//         
//     }
//
//     [Serializable]
//     public struct HttpHelperResponses
//     {
//         public long code;
//         public string message;
//         public string text;
//         public byte[] bytes;
//         
//     }
//     
//     private static HttpTool _instance = null;
//     public string url = "";
//
//
//
//     void Awake()
//     {
//         DonDestoryOnLoad(gameobject);
//
//     }
//
//
//         public void Post(string url,JsonData jsonData,HttpHelperPostGetCallbacks callback)
//     {
//         StartCoroutine(PostRequest(url,jsonData,callback));
//
//     }
//
//     public IEnumerator PostRequest(string url, string jsonString,HttpHelperPostGetCallbacks callback)
//     {
//         using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
//         {
//             byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);
//             request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
//             request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
//             request.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
//             yield return request.SendWebRequest();
//             
//            
//
//             Debug.Log("Status Code:" + request.responseCode);
//             if (request.isNetworkError || request.isHttpError)
//             {
//                 Debug.LogError(request.error+'\n'+request.downloadHandler.text);
//                 if (callback != null)
//                 {
//                     callback(null);
//                 }
//             }
//             else
//             {
//                 if (callback != null)
//                 {
//                     
//                     HttpHelperResponses response = new HttpHelperResponses();
//                     response.cede = request.repsonseCode;
//                     response.message = request.error;
//                     response.text = request.downloadHandler.text;
//                     response.bytes= request.downloadHandler.data;
//                     callback(request.downloadHandler.text);
//                 }
//             }
//
//             callback?.Invoke(request.responseCode, request, response);
//             request.Abort();
//             request.disposeDownloadhandlerOnDisponse = true;
//             request.Dispose();
//         }
//    
//     }
//     
//     
// }
