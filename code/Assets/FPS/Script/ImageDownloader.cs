using System;
using Directory = System.IO.Directory;
using File = System.IO.File;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows;

public class ImageDownloader : MonoBehaviour
{
    private string labIP = "http://139.155.28.154:26000";
    private string axonFileLocation = "/static/GameImages/18454/";
    private string dendriteFileLocation = "/static/GameImages/new";
    private string testfileName = "10058_14434_6006.v3dpbd";
    private string DownloadImageFolder = "ServerDownloadImages/";
    private string streamingAssetFilePath = Application.streamingAssetsPath + "/ImageResources/";

    private Mode curGameMode;

    private void Start()
    {
        
    }

    public string makeURL(string fileName)
    {
        return labIP + axonFileLocation + fileName;
    }
    
    public void DownloadImage(string imageURL, string fileName, Action <string> onSuccess, Action <string> onFailure, bool reDownload)
    {
        if (!reDownload)
        {
            byte[] imageData = GetImage(fileName);

            if (imageData != null)
            {
                Debug.Log("data received");
                onSuccess?.Invoke("s");
            }
        }

        StartCoroutine(IEDownloadImage(imageURL, fileName, bytes =>
        {
            string savePath = SaveImageAsV3dRaw(bytes,fileName);
            onSuccess?.Invoke(savePath);
        }, s =>
        {
            print(s);
            onFailure?.Invoke("Download Image Failed");
        }));
    }

    public IEnumerator IEDownloadImage(string imageURL, string fileName, Action <byte[]> onSuccess, Action <string> onFailure)
    {
        UnityWebRequest request = UnityWebRequest.Get(imageURL);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            DownloadHandler downloadHandler = request.downloadHandler;
            if (downloadHandler.data != null)
            {
                print("IE download success");
                onSuccess?.Invoke(downloadHandler.data); 
               yield break;
            }
            else
            {
                Debug.LogError("download image: " + fileName + " with empty data");
            }
        }
        else
        {
            onFailure?.Invoke("IEDownloadImage request error");
        }
    }

    private string SaveImageAsV3dRaw(byte[] imageData, string fileName)
    {
        string savePath = streamingAssetFilePath + DownloadImageFolder;
        try
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            PbdDecompresser pbdDecompresser = new PbdDecompresser();
            pbdInfo? info = pbdDecompresser.readHeader(imageData);
            if (info == null)
            {
                print("read header of pbd image failed, no v2draw image saved");
            }
            else
            {
                pbdInfo trueInfo = info.Value;
                byte[] v3drawData = pbdDecompresser.decompressPbdBytes(trueInfo, imageData);
                string v3drawFileName = pbdName2v3dName(fileName); 
                File.WriteAllBytes(savePath + v3drawFileName, v3drawData);
                print("new image " + v3drawFileName + " saved");
                return savePath + v3drawFileName;
            }
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return null;
    }

    private byte[] GetImage(string fileName)
    {
        string savePath = streamingAssetFilePath;

        try
        {
            if (File.Exists(savePath + fileName))
            {
                byte[] bytes = File.ReadAllBytes(savePath + fileName);
                return bytes;
            }

            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private string pbdName2v3dName(string originalName)
    {
        return System.IO.Path.ChangeExtension(originalName, ".v3draw");
    }

    public void cleanLocalRawImageCache()
    {
        string[] imageNames = Directory.GetFiles(streamingAssetFilePath + DownloadImageFolder);
        foreach (var imageName in imageNames)        
        {
            if (imageName.EndsWith(".v3draw"))
            {
                File.Delete(imageName);
            }
        } 
    }
}
