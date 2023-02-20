using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImageNameHelper 
{
    private string imageResourcesPath = Application.streamingAssetsPath + "/ImageResources/";
    // dendrite mode
    private string dendriteSWCLocation;
    private string dendriteImageLocation;
    private string[] dendriteSWCFiles;
    private string[] dendriteImages;
    // axon mode
    private string axonSWCLocation;
    private string axonPbdImageLocation;
    private string[] axonSWCFiles;
    private string[] axonPbdImageFiles;
    private string mapPath;

    public ImageNameHelper()
    {
        dendriteSWCLocation = imageResourcesPath + "dendriteSWC";
        dendriteImageLocation = imageResourcesPath + "dendriteImageRaw";
        dendriteSWCFiles = Directory.GetFiles(dendriteSWCLocation,"*.swc");
        dendriteImages = Directory.GetFiles(dendriteImageLocation, "*.v3draw");
        axonSWCLocation = imageResourcesPath + "ServerSWCFiles";
        axonPbdImageLocation = imageResourcesPath + "AxonPbdImages";
        axonSWCFiles = Directory.GetFiles(axonSWCLocation,"*.swc");
        axonPbdImageFiles = Directory.GetFiles(axonPbdImageLocation, "*.v3dpbd");
        mapPath = imageResourcesPath + "ServerWholdBrainSWC/18454_00002.swc"; 
    }

    public int getDendriteSWCFileNumber()
    {
        return dendriteSWCFiles.Length;
    }

    public int getAxonImageNumber()
    {
        return axonPbdImageFiles.Length;
    }

    public string getDendriteSWCFilePath(int index)
    {
        return dendriteSWCFiles[index];
    }

    public string getDendriteImagesPath(int index)
    {
        return dendriteImages[index];
    }

    public string getAxonSWCName(int index)
    {
        return axonSWCFiles[index];
    }

    public string getAxonSWCPath(string swcName)
    {
        return axonSWCLocation + "/" + swcName;
    }

    public string getAxonPbdImageName(int index)
    {
        return axonPbdImageFiles[index];
    }

    public string getMapPath()
    {
        return mapPath;
    }
}
