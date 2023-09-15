using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms;
using UnityVolumeRendering;
using Random = UnityEngine.Random;
using System.Collections;
using FPS.Script.UI;
using Unity.Mathematics;
using RenderMode = UnityVolumeRendering.RenderMode;


public class SwcGenerator_Practice : MonoBehaviour
{
    // image size parameter todo get from load v3draw
    private int xsize=256;
    private int ysize=256;
    private int zsize=256;
    
    public int imageScale = 10;
    
    public int currentImageIndex;
    private int currentDendriteImageIndex;
    private int trueDenIndex;
    private int currentAxonImageIndex;
    private int trueAxonIndex;
    
    public float shakeBranchIntensity = 2f;
    private List<Color> somaLinearColors = new List<Color>();
    // bp rotation speed
    public float _speed = 1f;
    
    public GameObject imageAndSWC; // todo generate from code
    public GameObject somaMonster; 
    public Vector3 imagePosition = new Vector3(70,8,50);
    
    // swc data structures:
    private int numberOfSWCStructure = 1;
    private List<SWCStructure> _swcStructures;
    private List<SWCGameObjects> _swcGameObjects;
    private List<SWC> _swcs; // todo may be override when two swc is loaded
    SWCGameObjects lastHitSWCObject = new SWCGameObjects();
    public SWCState state;
    
    // gameobject rendered in scene
    public GameObject BpSwc;  
    public GameObject swcNodeObject;
    public GameObject swcLinkObject;
    public GameObject somaSphereObject;
    public GameObject newBp;
    [SerializeField] private GameObject WrongBp;
    public GameObject hightlight;
    [SerializeField] private GameObject MapHighLight;

    // holding ref to raw image 
    private VolumeRenderedObject v3drawImage;
    private VolumeDataset dataset;
    private bool showRawImage;
    private bool showRawSwc;

    public BattleManager bm;
    public SceneObjectManager gm;

    private string[] v3drawImageFiles;
    private string[] swcFiles;
    private string streamingAssetFilePath = Application.streamingAssetsPath + "/ImageResources/";
    private string modifiedSWCPath;

    private int[] playerImageRandomIndexes;
    private int[] axonPlayerImageRandomIndexes;

    private Dictionary<GameObject, int> swcIndex;
    private List<int> wrongBpSet;
    private GameObject swccurrent;
    private GameObject swclast;
    private GameObject swcnext;
    private GameObject highlight;

    private Vector3 currentCoord;

    private ImageDownloader imageDownloader;
    private ImageNameHelper nameHelper;
    private TimeCounter tc;

    public Mode gameMode;
    public gameHints gh;

    void Start()
    {
        _swcStructures = new List<SWCStructure>(numberOfSWCStructure);
        _swcGameObjects = new List<SWCGameObjects>(numberOfSWCStructure);
        _swcs = new List<SWC>();
        swcIndex = new Dictionary<GameObject, int>();
        wrongBpSet = new List<int>();
        modifiedSWCPath = streamingAssetFilePath + "modifiedSWC/";
        
        somaSphereObject = GameObject.Find("somaSphere");
        WrongBp = GameObject.Find("WrongBp");
        swccurrent = GameObject.Find("swccurrent");
        swclast = GameObject.Find("swclast");
        swcnext = GameObject.Find("swcnext");
        nameHelper = new ImageNameHelper();
        tc = GameObject.Find("Canvas").GetComponent<TimeCounter>();
        imageDownloader = this.GetComponent<ImageDownloader>();
        gh = new gameHints();
        // set initial indexes
        playerImageRandomIndexes = shuffle(nameHelper.getDendriteSWCFileNumber(),leaveOut:0); // the first three image will be the same

        axonPlayerImageRandomIndexes = shuffle(nameHelper.getAxonImageNumber(),leaveOut:0); // the first three image will be the same
        String userID = PlayerPrefs.GetString("username");
        PlayerData.Instance.initDailyGameRecord(userID,ref playerImageRandomIndexes,ref axonPlayerImageRandomIndexes);

        // 获取玩家保存的index
        Mode gameM = GameConfig.Instance.GetGameMode();
        if (gameM == Mode.Dendrite)
        {
            int savedIndex = PlayerData.Instance.GetDenLevelId();
            if (savedIndex == -1)
            {
                currentDendriteImageIndex = 0;
            }
            else
            {
                currentDendriteImageIndex = savedIndex;
            }
        }else if (gameM == Mode.Axon)
        {
            int savedIndex = PlayerData.Instance.GetAxonLevelId();
            if (savedIndex == -1)
            {
                currentAxonImageIndex = 0;
            }
            else
            {
                print("axon read index " + currentAxonImageIndex);
                currentAxonImageIndex = savedIndex;
            } 
        }
        
        roundImageIndex();
        showRawImage = true;
        showRawSwc = true;
        initColors();
        state = SWCState.normal;
        
        if (GameConfig.Instance)
        {
            gameMode = GameConfig.Instance.GetGameMode();
        }
        else
        {
            Debug.LogError("no game config exists, game mode undecided");
        }
        drawAll(); 
        showHints();
        
        if (gameMode == Mode.Dendrite)
        {
            GameObject.Find("Canvas").GetComponent<UILevel>().UpdateGameLevel(currentDendriteImageIndex);
        }else if (gameMode == Mode.Axon)
        {
            GameObject.Find("Canvas").GetComponent<UILevel>().UpdateGameLevel(currentAxonImageIndex);
        }
    }

    private void roundImageIndex()
    {
        switch (gameMode)
        {
            case Mode.Axon:
                currentAxonImageIndex %= nameHelper.getAxonImageNumber();
                break;
            case Mode.Dendrite:
                currentDendriteImageIndex %= nameHelper.getDendriteSWCFileNumber();
                break;
        }
    }
    
    private void drawAll()
    {
        switch (gameMode)
        {
            case Mode.Dendrite:
                showDendriteSWCAndImage(currentDendriteImageIndex);
                break;
            case Mode.Axon:
                // GameObject.Find("Canvas").GetComponent<UILevel>().setLabel("");
                showAxonSWCAndRawImage(currentAxonImageIndex);
                string mapPath = nameHelper.getMapPath();
                Vector3 offset = new Vector3(3.0f, 15.0f, 0.0f); 
                drawResampleMapSWCandRawImage(mapPath,swccurrent.transform.position - offset);
                break;
            default:
                Debug.LogError("unknown gamemode in swc generator");
                break;
        } 
    }
    

    private void showDendriteSWCAndImage(int index)
    {
        // generate random index
        int[] randomIndex = PlayerData.Instance.GetRandomImageIndexes();
        if (randomIndex != null)
        {
            index = randomIndex[index];
        }
        else
        {
            playerImageRandomIndexes = shuffle(nameHelper.getDendriteSWCFileNumber(),leaveOut:0); // the first three image will be the same
            index = playerImageRandomIndexes[index];
        }
        print("current true dendrite image index is " + index);
        trueDenIndex = index;
        // currentDendriteImageIndex = index;
        // draw image
        string swcPath = nameHelper.getDendriteSWCFilePath(index);
        string currentSWCName = System.IO.Path.GetFileNameWithoutExtension(swcPath);
        // GameObject.Find("Canvas").GetComponent<UILevel>().setLabel(currentSWCName);
        string imagePath = nameHelper.getDendriteImagesPath(index);
        drawSWCandRawImage(swcPath,imagePath);
    }

    private void showAxonSWCAndRawImage(int index)
    {
        int[] randomIndex = PlayerData.Instance.GetAxonRandomImageIndexes();
        if (randomIndex != null)
        {
            index = randomIndex[index];
        }
        else
        {
            axonPlayerImageRandomIndexes = shuffle(nameHelper.getAxonImageNumber(),leaveOut:0); // the first three image will be the same
            index = axonPlayerImageRandomIndexes[index];
        } 
        print("current true axon image index is " + index);
        trueAxonIndex = index;
        string imagePath = nameHelper.getAxonPbdImageName(index);
        string imageName = System.IO.Path.GetFileNameWithoutExtension(imagePath);
        //Debug.Log(imageName);
        //string[] xyz = imageName.Split("_");
        //Vector3 center = new Vector3(float.Parse(xyz[0]), float.Parse(xyz[1]), float.Parse(xyz[2]));
        //Vector3 block = new Vector3(256f, 256f, 256f);
        //MapHightLight(center, block);
        // GameObject.Find("Canvas").GetComponent<UILevel>().setLabel(imageName);
        PbdDecompresser pbdDecompresser = new PbdDecompresser();
        byte[] imageData = File.ReadAllBytes(imagePath);
        string v3drawDecompressedPath = "";
        pbdInfo? info = pbdDecompresser.readHeader(imageData);
        if (info == null)
        {
            print("read header of pbd image failed, no v3draw image saved");
        }
        else
        {
            pbdInfo trueInfo = info.Value;
            byte[] v3drawData = pbdDecompresser.decompressPbdBytes(trueInfo, imageData);
            v3drawDecompressedPath = Application.streamingAssetsPath + "/ImageResources/" + "ServerDownloadImages/" + imageName + ".v3draw";
            File.WriteAllBytes(v3drawDecompressedPath, v3drawData);
            // print("new image " + v3drawFileName + " saved");
        }
        string swcName = imageName + ".swc";
        string swcPath = nameHelper.getAxonSWCPath(swcName);
        drawSWCandRawImage(swcPath,v3drawDecompressedPath);
        

        // load from cloud
        // imageDownloader = this.GetComponent<ImageDownloader>();
        // string remoteImagePath = imageDownloader.makeURL(v3drawImageName);
        // print(remoteImagePath);
        // imageDownloader.DownloadImage(remoteImagePath,v3drawImageName,successHandler,failHandler,true);
    }

    // private void successHandler(string savePath)
    // {
    //     // GameObject.Find("Canvas").GetComponent<UILevel>().setLabel("");
    //      string swcPath = nameHelper.getAxonSWCName(currentImageIndex);
    //      print(System.IO.Path.GetFileNameWithoutExtension(swcPath));
    //      drawSWCandRawImage(swcPath,savePath);
    //      // print("bpcount " + getBpCount());
    //      tc.setTime(getBpCount() * 20);
    //      string currentSWCName = savePath; //same as v3dpbd image name
    //      int[] coords = getCoordFromName(currentSWCName);
    //      MapHightLight(new Vector3(coords[0], coords[1], coords[2]), new Vector3(256f, 256f, 256f),xsize,ysize,zsize); 
    //      imageDownloader.cleanLocalRawImageCache();
    // }

    private void failHandler(string error)
    { 
        GameObject.Find("Canvas").GetComponent<UILevel>().setLabel("Image Download Failed");
       print(error); 
    }

    private int[] getCoordFromName(string swcFileName)
    {
        string rawName = Path.GetFileNameWithoutExtension(swcFileName);
        string[] coords = rawName.Split("_");
        int[] coordArray = new int[3];
        if (coords.Length != 3)
        {
            Debug.Log("error when read coords from swc filename");
            return null;
        }
        for (int i = 0; i < coords.Length; i++)
        {
            coordArray[i] = int.Parse(coords[i]);
        }

        return coordArray;
    }
    void drawSWCandRawImage(string swcpath,string v3drawPath)
    {
        
        List<Vector3> positions = new List<Vector3>();
        positions.Add(imagePosition); 
        // check for given position number;
        if (positions.Count != numberOfSWCStructure)
        {
            Debug.LogError("try to draw " + numberOfSWCStructure + " swc but only found " + positions.Count + " position given");
            return;
        }
        for (int i = 0; i < numberOfSWCStructure; i++)
        {
            // string imagePath = v3drawImageFiles[index];
            generateV3drawImage(filePath: v3drawPath, position: positions[i]);
        }
       
        string swcPath = swcpath;
        string swcmsg = File.ReadAllText(swcPath, Encoding.UTF8);
        SWCStructure newSwcStructure = ParseSwcFile(swcpath,swcmsg,xsize,ysize,zsize);
        _swcStructures.Add(newSwcStructure);
        // draw all swc and image
        for (int i = 0; i < numberOfSWCStructure; i++)
        {
            SWCGameObjects newSwcGameObjects = GenerateSwcGameObjects(_swcStructures[i]);
            _swcGameObjects.Add(newSwcGameObjects);
            imageAndSWC.transform.position = imagePosition;
        }


        //GenerateFakeBranch();
    }


    // void drawMapSWCandRawImage(int index,Vector3 position,bool drawswc=true,bool drawimg=false)
    // {
    //     int[] randomIndex = PlayerData.Instance.GetRandomImageIndexes();
    //     if (randomIndex != null)
    //     {
    //         index = randomIndex[index];
    //     }
    //     else
    //     {
    //         playerImageRandomIndexes = shuffle(swcFiles.Length);
    //         index = playerImageRandomIndexes[index];
    //     }
    //     print("current true image index is " + index);
    //     // check for index
    //     if (index < 0 || index > v3drawImageFiles.Length - 1)
    //     {
    //         return;
    //     }
    //
    //     // read all swc files
    //     List<Vector3> mappositions = new List<Vector3>();
    //     mappositions.Add(position);
    //     // check for given position number;
    //     if (mappositions.Count != numberOfSWCStructure)
    //     {
    //         Debug.LogError("try to draw " + numberOfSWCStructure + " swc but only found " + mappositions.Count + " position given");
    //         return;
    //     }
    //
    //     // read all swc files
    //     //for (int i = 0; i < numberOfSWCStructure; i++)
    //     //{
    //         int currentIndex = index ;
    //         currentIndex = currentIndex % (swcFiles.Length);
    //         string swcPath = swcFiles[currentIndex];
    //          //Debug.Log("swc file path" + swcPath);
    //         string swcmsg = File.ReadAllText(swcPath, Encoding.UTF8);
    //         SWCStructure newSwcStructure = ParseMapSwcFile(swcmsg);
    //         _swcStructures.Add(newSwcStructure);
    //     //}
    //     // draw all swc and image
    //     //for (int i = 0; i < numberOfSWCStructure; i++)
    //     //{
    //         GameObject imgandswc = new GameObject();
    //         imgandswc.name = "ImageAndSwc_" + index.ToString();
    //         if (drawswc)
    //         {
    //
    //             SWCGameObjects newSwcGameObjects = GenerateMapSwcGameObjects(newSwcStructure, imgandswc);
    //             _swcGameObjects.Add(newSwcGameObjects);
    //         }
    //
    //         imgandswc.transform.position = position;
    //         if (drawimg)
    //         {
    //             string imagePath = v3drawImageFiles[index];
    //             generateMapV3drawImage(imagePath, position);
    //         }
    //     //}
    // }
    void drawResampleMapSWCandRawImage(string path,Vector3 position)
    {
        string swcPath = path;
        NeuronTree nt = readSWC_file(swcPath);
        NeuronTree ntres = resample(nt, 256f);
        string swcmsg = export_list2string(ntres.listNeuron);
        // Debug.Log(swcmsg);
        SWCStructure newSwcStructure = ParseMapSwcFile(swcmsg,256,256,256);
        //_swcStructures.Add(newSwcStructure);
        GameObject mapSWC = new GameObject();
        mapSWC.name = "MapSWC";
        SWCGameObjects newSwcGameObjects = GenerateMapSwcGameObjects(newSwcStructure, mapSWC);
        //_swcGameObjects.Add(newSwcGameObjects);
        mapSWC.transform.position = position;
        mapSWC.transform.localScale = Vector3.one / 100 * 3;
        //string imagePath = nameHelper.getAxonPbdImageName(currentAxonImageIndex);
        //string imageName = System.IO.Path.GetFileNameWithoutExtension(imagePath);
        //Debug.Log(imageName);
        //string[] xyz = imageName.Split("_");
        //Vector3 center = new Vector3(float.Parse(xyz[0]), float.Parse(xyz[1]), float.Parse(xyz[2]));
        //Vector3 block = new Vector3(256f, 256f, 256f);
        //MapHightLight(center, block);
    }

    private string getCurrentSWCFileName()
    {
        string result = "";
        switch (gameMode)
        {
            case Mode.Dendrite:
                result = nameHelper.getDendriteSWCFilePath(trueDenIndex);
                result = Path.GetFileName(result);
                break;
            case Mode.Axon:
                result = nameHelper.getAxonPbdImageName(trueAxonIndex);
                result = Path.GetFileName(result);
                break;
        }
        return Path.GetFileNameWithoutExtension(result);
    }
    public void nextImage()
    {
        state = SWCState.normal;
        // save game record
        if (PlayerData.Instance == null)
        {
            print("null player data");
        }

        int level = 0;
        if (gameMode == Mode.Dendrite)
        {
            level = PlayerData.Instance.GetDenLevelId()+1;
        }else if (gameMode == Mode.Axon)
        {
            level = PlayerData.Instance.GetAxonLevelId() + 1;
        }
        Record gameRecord = new Record
        {
            GameMode = "Level-" + level.ToString(),
            Score = bm.getScore(),
            ImageId = getCurrentSWCFileName(),
            TotalCheckingPoints = bm.GetBpNums(),
            TotalSwcLength = 50,
            TotalGeneratedCheckingPoints = GetMissedBpCount()
        };
        String userID = PlayerPrefs.GetString("username");
        // Debug.Log("userid"+userID);
        if (userID != null)
        {
            Mode gameMode = GameConfig.Instance.GetGameMode();
            switch (gameMode)
            {
                case Mode.Axon:

                    PlayerData.Instance.AddDailyGameRecord(userID, gameRecord,gameMode,axonPlayerImageRandomIndexes);
                    break;
                case Mode.Dendrite:
                    PlayerData.Instance.AddDailyGameRecord(userID, gameRecord,gameMode,playerImageRandomIndexes);
                    break;
            }
           
        }
        else
        {
            PlayerData.Instance.AddDailyGameRecord("test", gameRecord,GameConfig.Instance.GetGameMode(),playerImageRandomIndexes);
        }
        
         
        imageDownloader.cleanLocalRawImageCache();
        GameObject imageBlock = GameObject.Find("VolumeRenderedObject_" + dataset.datasetName);
        Destroy(imageBlock);
        desNewBp();
        _swcStructures.Clear();
        _swcGameObjects.Clear();
        _swcs.Clear();
        swcIndex.Clear();
        wrongBpSet.Clear();
        destroyLastHitSWC();
        Destroy(highlight);
        lastHitSWCObject = null;
        gm.DesCoin();

        if (gameMode == Mode.Dendrite)
        {
            currentDendriteImageIndex += 1;
        }else if (gameMode == Mode.Axon)
        {
            currentAxonImageIndex += 1;
        }
        // currentImageIndex += 1;
        roundImageIndex();
        drawAll();
        showHints();
        
        if (gameMode == Mode.Dendrite)
        {
            GameObject.Find("Canvas").GetComponent<UILevel>().UpdateGameLevel(currentDendriteImageIndex);
        }else if (gameMode == Mode.Axon)
        {
            GameObject.Find("Canvas").GetComponent<UILevel>().UpdateGameLevel(currentAxonImageIndex);
        }
    }

    void showHints()
    {
        string hint = gh.getHint();
        string swcName = getCurrentSWCFileName();
        string show = hint + "\nimage name: " + swcName;
        GameObject.Find("Canvas").GetComponent<UILevel>().setHintsLabel(show);
    }

    // Update is called once per frame
    void Update()
    {
        if (_swcGameObjects.Count > 0)
        {
            rotateAllBranchingPoint();
        }

        if (gameMode == Mode.Dendrite)
        {
            switch (state)
            {
                case SWCState.normal:
                    break;
                case SWCState.toBeExploded:
                    if (_swcGameObjects.Count > 0)
                    {
                        shakeBranch(_swcGameObjects[0]);
                    }

                    if (_swcGameObjects[0].somaSphere)
                    {
                        _swcGameObjects[0].somaSphere.transform.LookAt(bm.player.transform.position, Vector3.up);
                    }

                    break;
                case SWCState.exploded:
                    break;
                default:
                    Debug.LogError("invaild swc state");
                    break;
            }
        }
    }
    
    void rotateAllBranchingPoint()
    {
        foreach (var swcGameObject in _swcGameObjects)        
        {
            foreach (var point in swcGameObject.BranchingPointsObjects)
            {
                point.transform.Rotate(0, _speed, 0, Space.Self);
            }
        }
    }

    void desNewBp()
    {
        // print("new bp here in " + _swcGameObjects.Count);
        if (_swcGameObjects.Count == 0)
        {
            return;
        }
        else
        {
            foreach (GameObject o in _swcGameObjects[0].newBranchingPointsByPlayer)
            {
                DestroyImmediate(o);
            }
            _swcGameObjects[0].newBranchingPointsByPlayer.Clear(); 
        }
    }

    // swc 文件解析新函数
    SWCStructure ParseSwcFile(string swcpath, string swcmsg,int xsize=96,int ysize=96,int zsize=48)
    {
        int nmax=0;
        SWCStructure newSwcStructure = new SWCStructure();
        Dictionary<int, Branch> _branchSet = new Dictionary<int, Branch>();
        List<SWC> BranchingPoint = new List<SWC>();
        string[] coordinate = new string[] {};
        coordinate = swcmsg.Split('\n');
        
        int branchId = 0;
        Branch tempBranch = new Branch();
        tempBranch.NodeList = new List<SWC>();
        tempBranch.LinkList = new List<SWCLink>();
        
        // 处理branching points
        var indexChildrenMap = new Dictionary<int, int>();
        int count = 0;
        foreach (string var in coordinate)
        {
            if (var.Length <= 0 || var[0] == '#')
                continue;
            string[] line = var.Split(" ");
            float index = float.Parse(line[0]);
            int nodeType = int.Parse(line[1]);
            float x = float.Parse(line[2]);
            float y = float.Parse(line[3]);
            float z = float.Parse(line[4]);
            if (x <= -10f || x >= 266f || y <= -10f || y >= 266f || z <= -10f || z >= 266f)         //csz added ,try to crop the useless swc segment
                nodeType = 0;
            indexChildrenMap.Add((int)index, 0);

            Vector3 point = new Vector3(imageScale * x / xsize, imageScale * y / ysize, imageScale * z / zsize);
            // node
            SWC tswc = new SWC();
            tswc.index = int.Parse(line[0]);
            tswc.parent = int.Parse(line[6]);
            // print(tswc.parent);
            tswc.nodetype = nodeType;
            tswc.pos = point;
            if (index <= nmax)
                tswc.type = BpType.defaultNode;
            else
                tswc.type = BpType.untouchedBp;
            _swcs.Add(tswc);
            count++;
        }

        foreach (var tswc in _swcs)
        {
            if (tswc.nodetype == 0)
            {
               continue; // skip type=0 point
            }

            // find branching point
            if (tswc.parent != -1)
            {
                indexChildrenMap[tswc.parent]++;
                if (indexChildrenMap[tswc.parent] == 2)
                {
                    //Debug.Log(nswcs.Count);
                    foreach (SWC swc in _swcs) 
                    {
                        if (swc.index == tswc.parent)
                        {
                           BranchingPoint.Add(swc);
                        }
                    }
                }
            }
            
            // branch test
            int nodeCnt = tempBranch.NodeList.Count;
            if (nodeCnt == 0)
            {
                // add node
                tempBranch.NodeList.Add(tswc);
            }
            else
            {
                var lastNode = tempBranch.NodeList.ElementAt(nodeCnt - 1);
                if (tswc.parent == lastNode.index)
                {
                    // add node
                    tempBranch.NodeList.Add(tswc);
                    // add connection
                    var swcLink = new SWCLink();
                    swcLink.NodePair = new float[] { tswc.index, tswc.parent };

                    // direction 
                    swcLink.Pos = (tswc.pos + lastNode.pos) / 2;
                    swcLink.Direction = tswc.pos - lastNode.pos;
                    swcLink.n = tswc.index;
                    swcLink.p = lastNode.index;
                    tempBranch.LinkList.Add(swcLink);
                }
                else
                {
                    // 找到父亲节点添加connection
                    var nodeIndex = tswc.parent - 1;
                    if (nodeIndex < _swcs.Count-1 && nodeIndex >= 0)
                    {
                        var parentNode = _swcs.ElementAt(tswc.parent - 1);
                        var swcLink = new SWCLink();
                        swcLink.NodePair = new float[] { tswc.index, tswc.parent };
                        // link
                        swcLink.Pos = (tswc.pos + parentNode.pos) / 2;
                        swcLink.Direction = tswc.pos - parentNode.pos;
                        swcLink.n = tswc.index;
                        swcLink.p = parentNode.index;
                        // 计入segment id比较小的segment中
                        tempBranch.LinkList.Add(swcLink);
                    }
                    _branchSet.Add(branchId, tempBranch);

                    // 参数重置
                    branchId++;
                    tempBranch = new Branch();
                    tempBranch.NodeList = new List<SWC>();
                    tempBranch.LinkList = new List<SWCLink>();

                    tempBranch.NodeList.Add(tswc);

                }
                
            }
        }
        // 处理最后一个branch
        _branchSet.Add(branchId, tempBranch);
        
        // 设置branching point类型
        for (int i = 0; i < BranchingPoint.Count; i++) // change type value for each node
        {
            var swc = BranchingPoint[i];
            swc.type = BpType.untouchedBp;
            
            // change type in _swcs list
            int index = _swcs.IndexOf(BranchingPoint[i]);
            if (index != -1)
            {
                _swcs[index] = swc; 
            }
            else
            {
                Debug.LogError("can't find swc in _swcs list");
            }
        }

        int id = -1;
        int bmax = 65536;
        foreach (var branch in _branchSet)
        {
            if (branch.Value.NodeList.Count < bmax)
            {
                id = branch.Key;
                bmax = branch.Value.NodeList.Count;
            }
        }
        foreach (SWC swc in _branchSet[id].NodeList)
        {
            if (BranchingPoint.Contains(swc))
            {
                BranchingPoint.Remove(swc);
            }
        }
        _branchSet.Remove(id);

        newSwcStructure.branches = _branchSet;
        newSwcStructure.BranchingPoint = BranchingPoint;
        return newSwcStructure;
    }

    string modifyswc(string swcmsg,ref int nmax,float wrongpercent=0.5f,int count=10)
    {
        string[] swc = swcmsg.Split('\n');
        Dictionary<int, int> swcnode = new Dictionary<int, int>();
        int linenum = 0;
        foreach (var s in swc)
        {
            
            if (s.Length<=0||s[0] == '#')
            {
                continue;
            }
            int n = int.Parse(s.Split(' ')[0]);
            int p = int.Parse(s.Split(' ')[6]);
            swcnode.Add(n, p);
            linenum++;
        }
        nmax = linenum;
        int len = swcnode.Count+1;
        int[] mark = new int[len];
        for(int i = 0; i < len; i++)
        {
            mark[i] = 0;
        }
        List<int> bplist = new List<int>();
        foreach(var node in swcnode)
        {
            if (node.Value != -1)
            {
                mark[node.Value]++;
                if (mark[node.Value] == 2)
                {
                    bplist.Add(node.Value);
                }
            }
        }
        for (int i = 0; i < len; i++)
        {
            mark[i] = 0;
        }
        List<List<int>> branches = new List<List<int>>();
        //Debug.Log(swcnode.Count);
        //Debug.Log(len);
        for (int i = swcnode.Count-1; i >= 0; i--)
        {

            if (mark[swcnode.ElementAt(i).Key] == 1)
                continue;
            List<int> branch = new List<int>();
            int m = i;
            int val = swcnode.ElementAt(m).Value;
            while (m >= 0 && val != -1 && mark[val] == 0)
            {
                int n = swcnode.ElementAt(m).Key;

                mark[n] = 1;
                if (bplist.Contains(n))
                {

                    break;
                }
                branch.Add(n);
                m--;
                val = swcnode.ElementAt(m).Value;
            }
            
            branches.Add(branch);
        }
        List<int> wrongbp = new List<int>();
        foreach(var branch in branches)
        {
            float p = Random.Range(0f, 1f);
            if (p < wrongpercent)
            {
                int mid = branch.Count / 2;
                if(branch.Count>0)
                    wrongbp.Add(branch[mid]);
            }
        }

        foreach(int i in wrongbp)
        {
            foreach (var s in swc)
            {
                if (s.Length <= 0 || s[0] == '#')
                {
                    continue;
                }
                int n = int.Parse(s.Split(' ')[0]);
                if (n != i)
                    continue;
                List<Vector3> bias = generatebias(count);
                float x = float.Parse(s.Split(" ")[2]);
                float y = float.Parse(s.Split(" ")[3]);
                float z = float.Parse(s.Split(" ")[4]);
                int parent=int.Parse(s.Split(" ")[0]);
                foreach(var b in bias)
                {
                    linenum++;
                    int index = linenum;
                    float nx = x + b.x;
                    float ny = y + b.y;
                    float nz = z + b.z;
                    string tres = index.ToString() + " " + s.Split(" ")[1] + " " + nx.ToString() + " " + ny.ToString() + " " + nz.ToString() + " " + s.Split(" ")[5] + " " + parent.ToString() + "\n";
                    parent = index;
                    swcmsg += tres;
                }
            }
        }

        
        //Debug.Log(branches.Count);
        return swcmsg;
    }

    List<Vector3> generatebias(int count,int mindistanceoneaxis=3,int maxdistanceoneaxis=5)
    {
        List<Vector3> result = new List<Vector3>();
        if (mindistanceoneaxis > maxdistanceoneaxis)
            return result;
        int dirx = Random.Range(0f, 1f) > 0.5f ? 1 : -1;
        int diry = Random.Range(0f, 1f) > 0.5f ? 1 : -1;
        int dirz = Random.Range(0f, 1f) > 0.5f ? 1 : -1;
        Vector3 last = Vector3.zero;
        while (count > 0)
        {
            int x = Random.Range(mindistanceoneaxis, maxdistanceoneaxis) + (int)last.x;
            int y = Random.Range(mindistanceoneaxis, maxdistanceoneaxis) + (int)last.y;
            int z = Random.Range(mindistanceoneaxis, maxdistanceoneaxis) + (int)last.z;
            last = new Vector3(x, y, z);
            Vector3 posbias = new Vector3(last.x * dirx, last.y * diry, last.z * dirz);
            result.Add(posbias);
            count--;
            //Debug.Log(posbias);
        }

        return result;
    }
    SWCStructure ParseMapSwcFile(string swcmsg,int xsize=96,int ysize=96,int zsize=48)
    {
        SWCStructure newSwcStructure = new SWCStructure();
        Dictionary<int, Branch> _branchSet = new Dictionary<int, Branch>();
        List<SWC> BranchingPoint = new List<SWC>();
        string[] coordinate = new string[] {};
        coordinate = swcmsg.Split('\n');
        int branchId = 0;
        Branch tempBranch = new Branch();
        tempBranch.NodeList = new List<SWC>();
        tempBranch.LinkList = new List<SWCLink>();
        
        // 处理branching points
        var indexChildrenMap = new Dictionary<int, int>();
        List<SWC> nswcs = new List<SWC>();
        foreach (string var in coordinate)
        {
            if (var.Length <= 0 || var[0] == '#')
                continue;
            string[] line = var.Split(" ");
            float index = float.Parse(line[0]);
            int nodetype = int.Parse(line[1]);
            float x = float.Parse(line[2]);
            float y = float.Parse(line[3]);
            float z = float.Parse(line[4]);
            indexChildrenMap.Add((int)index, 0);

            Vector3 point = new Vector3(imageScale * x / xsize, imageScale * y / ysize, imageScale * z / zsize);
            // node
            SWC tswc = new SWC();
            tswc.index = int.Parse(line[0]);
            tswc.parent = int.Parse(line[6]);
            tswc.pos = point;
            tswc.type = BpType.defaultNode;
            tswc.nodetype = nodetype;
            nswcs.Add(tswc);
        }
        foreach (string var in coordinate)
        {
            if (var.Length <= 0 || var[0] == '#')
                continue;
            
            string[] line = var.Split(" ");
            float index = float.Parse(line[0]);
            int nodetype = int.Parse(line[1]);
            float x = float.Parse(line[2]);
            float y = float.Parse(line[3]);
            float z = float.Parse(line[4]);
            
            Vector3 point = new Vector3(imageScale * x / xsize, imageScale * y / ysize, imageScale * z / zsize);
            
            // node
            SWC tswc = new SWC();
            tswc.index = int.Parse(line[0]);
            tswc.parent = int.Parse(line[6]);
            tswc.pos = point;
            tswc.type = BpType.defaultNode;
            //_swcs.Add(tswc);

            // find branching point
            //indexChildrenMap.Add(tswc.index, 0);
            if (tswc.parent != -1)
            {
                // children count ++
                
                indexChildrenMap[tswc.parent]++;
                if (indexChildrenMap[tswc.parent] == 2)
                {
                    //Debug.Log(nswcs.Count);
                    foreach (SWC swc in nswcs) 
                    {
                        if (swc.index == tswc.parent)
                        {
                           BranchingPoint.Add(swc);
                           //Debug.Log(swc.index);
                        }
                    }
                    // BranchingPoint.Add(tswc);
                }
            }
            
            // branch test
            int nodeCnt = tempBranch.NodeList.Count;
            if (nodeCnt == 0)
            {
                // add node
                tempBranch.NodeList.Add(tswc);
            }
            else
            {
                var lastNode = tempBranch.NodeList.ElementAt(nodeCnt - 1);
                //Debug.Log("--------------------------");
                //Debug.Log(index);
                //Debug.Log(tswc.parent);
                //Debug.Log(lastNode.index);
                if (tswc.parent == lastNode.index)
                {
                    // add node
                    tempBranch.NodeList.Add(tswc);
                    // add connection
                    var swcLink = new SWCLink();
                    swcLink.NodePair = new float[] { index, tswc.parent };

                    // direction 
                    swcLink.Pos = (tswc.pos + lastNode.pos) / 2;
                    swcLink.Direction = tswc.pos - lastNode.pos;
                    swcLink.n = tswc.index;
                    swcLink.p = lastNode.index;
                    tempBranch.LinkList.Add(swcLink);
                }
                else
                {
                    // 找到父亲节点添加connection
                    var parentNode = nswcs.ElementAt(tswc.parent - 1);
                    //Debug.Log("-----------------------------");
                    //Debug.Log(tswc.index);
                    //Debug.Log(tswc.parent);
                    var swcLink = new SWCLink();
                    swcLink.NodePair = new float[] { index, tswc.parent };
                    // link
                    swcLink.Pos = (tswc.pos + parentNode.pos) / 2;
                    swcLink.Direction = tswc.pos - parentNode.pos;
                    swcLink.n = tswc.index;
                    swcLink.p = parentNode.index;
                    // 计入segment id比较小的segment中
                    tempBranch.LinkList.Add(swcLink);
                    
                    
                    _branchSet.Add(branchId, tempBranch);
                    
                    // 参数重置
                    branchId++;
                    tempBranch = new Branch();
                    tempBranch.NodeList = new List<SWC>();
                    tempBranch.LinkList = new List<SWCLink>();
                    
                    tempBranch.NodeList.Add(tswc);
                }
                
            }
        }
        // 处理最后一个branch
        _branchSet.Add(branchId, tempBranch);
        
        // 设置branching point类型
        for (int i = 0; i < BranchingPoint.Count; i++) // change type value for each node
        {
            var swc = BranchingPoint[i];
            swc.type = BpType.untouchedBp;
            
            // change type in _swcs list
            int index = nswcs.IndexOf(BranchingPoint[i]);
            if (index != -1)
            {
                nswcs[index] = swc; 
            }
            else
            {
                Debug.LogError("can't find swc in _swcs list");
            }
        }

        newSwcStructure.branches = _branchSet;
        newSwcStructure.BranchingPoint = BranchingPoint;

        return newSwcStructure;
    }
    
    
    SWCGameObjects GenerateSwcGameObjects(SWCStructure swcStructure)
    {
        List<GameObject> BranchingPointsObjects = new List<GameObject>(); 
        List<GameObject> branchGameObjects = new List<GameObject>(); 
        GameObject somaSphere = null;
        imageAndSWC.transform.position = new Vector3(imageScale / 2.0f, imageScale / 2.0f, imageScale / 2.0f);
        foreach (var item in swcStructure.branches)
        {
            int branchId = item.Key;
            var branch = item.Value;
            GameObject branchGameObject = new GameObject("Branch_" + branchId.ToString());
            branchGameObjects.Add(branchGameObject);
            foreach (var swcNode in branch.NodeList)
            {
                if (swcNode.parent == -1 && gameMode == Mode.Dendrite)
                {
                    print("soma gen");
                    somaSphere = Instantiate(somaSphereObject, swcNode.pos, Quaternion.identity);
                    // somaSphere.transform.GetChild(0).tag = "soma";
                    // somaSphere.transform.localScale = Vector3.one * 4;
                    //imageAndSWC.transform.position = somaSphere.transform.position;
                    somaSphere.transform.SetParent(imageAndSWC.transform);
                    //Debug.Log(somaSphere.transform.position - imageAndSWC.transform.position);
                }
                GameObject node = Instantiate(swcNodeObject, swcNode.pos, Quaternion.identity);
                swcIndex.Add(node, swcNode.index);
                node.name = "node-" + swcNode.index.ToString();
                node.transform.SetParent(branchGameObject.transform);

            }

            foreach (var swcLink in branch.LinkList)
            {
                Vector3 position = swcLink.Pos;
                if (swcLink.Direction != Vector3.zero)
                {
                    Quaternion rotation = Quaternion.LookRotation(swcLink.Direction);
                    GameObject linkObject = Instantiate(swcLinkObject, position, rotation);

                    // scale
                    Vector3 scale = Vector3.one;
                    var dirMagnitude = swcLink.Direction.magnitude;
                    scale.z = dirMagnitude;
                    linkObject.transform.localScale = scale;

                    linkObject.transform.SetParent(branchGameObject.transform);
                }
            
        }
            
            branchGameObject.transform.SetParent(imageAndSWC.transform);
        }

        foreach (SWC bp in swcStructure.BranchingPoint)
        {
            // Debug.Log(bp.n)
            GameObject newBranchingPointObject = (GameObject)Instantiate(BpSwc, bp.pos, Quaternion.identity);
            swcIndex.Add(newBranchingPointObject, bp.index);
            newBranchingPointObject.name = "virus-" + bp.index;
            BranchingPointsObjects.Add(newBranchingPointObject);
            newBranchingPointObject.transform.SetParent(imageAndSWC.transform);
        }

        SWCGameObjects newSWCObjects = new SWCGameObjects();
        newSWCObjects.somaSphere = somaSphere;
        newSWCObjects.branchGameObjects = branchGameObjects;
        newSWCObjects.BranchingPointsObjects = BranchingPointsObjects;
        return newSWCObjects;
    }

    List<GameObject> GenerateFakeBranch(int count = 3)
    {
        List<GameObject> FakeBranch = new List<GameObject>();
        List<GameObject> BranchGameObjects = _swcGameObjects[0].branchGameObjects;
        foreach(var branch in BranchGameObjects)
        {
            float p = Random.Range(0f, 1f);
            if (count>0&&p < 0.5f)
            {
                count--;
                Transform[] nodes = branch.GetComponentsInChildren<Transform>();
                GameObject fakebp = new GameObject();
                Vector3 pos = new Vector3();
                foreach(Transform var in nodes)
                {
                    if (var.name.StartsWith("node"))
                    {
                        fakebp = var.gameObject;
                        break;
                    }
                }
                foreach (Transform var in nodes)
                {
                    float pp = Random.Range(0f, 1f);
                    if (pp<0.3f&&var.name.StartsWith("node")&&var.gameObject!=fakebp)
                    {
                        pos = branch.transform.position - fakebp.transform.position + var.position;
                        break;
                    }
                }
                GameObject fakebranch = Instantiate(branch,pos,Quaternion.identity);
                FakeBranch.Add(fakebranch);
            }
        }
        return FakeBranch;
    }
    public int getBpCount()
    {
        switch (gameMode)
        {
            case Mode.Dendrite:
                case Mode.Axon:
                if (_swcGameObjects != null && _swcGameObjects.Count > 0)
                {
                    return _swcGameObjects[0].BranchingPointsObjects.Count + 1;
                }
                else
                {
                    // print("bp count use default 15");
                    return 15; 
                }
            default:
                // print("bp count use default 15");
                return 15;
        }
        if (_swcStructures.Count == 2)
        {
           return _swcGameObjects[0].BranchingPointsObjects.Count; 
        }
        else
        {
            return 15;
        }
        
    }

    SWCGameObjects GenerateMapSwcGameObjects(SWCStructure swcStructure,GameObject imgandswc=null)
    {
        if (imgandswc == null)
        {
            imgandswc = imageAndSWC;
        }
        List<GameObject> BranchingPointsObjects = new List<GameObject>();
        List<GameObject> branchGameObjects = new List<GameObject>();
        // generate soma object
        GameObject somaSphere = new GameObject();
        foreach (var item in swcStructure.branches)
        {
            int branchId = item.Key;
            var branch = item.Value;
            GameObject branchGameObject = new GameObject("Branch_" + branchId.ToString());
            branchGameObjects.Add(branchGameObject);
            foreach (var swcNode in branch.NodeList)
            {
                if (swcNode.parent == -1)
                {
                    somaSphere = Instantiate(somaSphereObject, swcNode.pos, Quaternion.identity);
                    // somaSphere.transform.GetChild(0).tag = "soma";
                    // somaSphere.transform.localScale = Vector3.one * 4;
                    imgandswc.transform.position = somaSphere.transform.position;
                    somaSphere.transform.SetParent(imgandswc.transform);
                }
                GameObject node = Instantiate(swcNodeObject, swcNode.pos, Quaternion.identity);
                swcIndex.Add(node, swcNode.index);
                node.name = "node-" + swcNode.index.ToString();
                node.transform.SetParent(branchGameObject.transform);
                Vector3 oldScale = node.transform.localScale;
                Vector3 newScale = oldScale * 4.0f;
                node.transform.localScale = newScale;
            }

            foreach (var swcLink in branch.LinkList)
            {
                Vector3 position = swcLink.Pos;
                Quaternion rotation = Quaternion.LookRotation(swcLink.Direction);

                GameObject linkObject = Instantiate(swcLinkObject, position, rotation);
                // scale
                Vector3 scale = Vector3.one;
                var dirMagnitude = swcLink.Direction.magnitude;
                scale.z = dirMagnitude;
                linkObject.transform.localScale = scale;

                linkObject.transform.SetParent(branchGameObject.transform);
                linkObject.name = "link-" + swcLink.n.ToString() + "_" + swcLink.p.ToString();
                
                Vector3 oldScale = linkObject.transform.localScale;
                Vector3 newScale = new Vector3(oldScale.x * 4.0f, oldScale.y * 4.0f, oldScale.z);
                linkObject.transform.localScale = newScale;
            }

            branchGameObject.transform.SetParent(imgandswc.transform);
        }
        MapHighLight = new GameObject("MapHighLight");
        MapHighLight.transform.SetParent(imgandswc.transform);
        //MapHighLight.transform.position = imgandswc.transform.position;
        // foreach (SWC bp in swcStructure.BranchingPoint)
        // {
        //     // Debug.Log(bp.n)
        //     GameObject newBranchingPointObject = (GameObject)Instantiate(BpSwc, bp.pos, Quaternion.identity);
        //     swcIndex.Add(newBranchingPointObject, bp.index);
        //     newBranchingPointObject.name = "virus-" + bp.index;
        //     BranchingPointsObjects.Add(newBranchingPointObject);
        //     newBranchingPointObject.transform.SetParent(imgandswc.transform);
        // }

        SWCGameObjects newSWCObjects = new SWCGameObjects();
        newSWCObjects.somaSphere = somaSphere;
        newSWCObjects.branchGameObjects = branchGameObjects;
        newSWCObjects.BranchingPointsObjects = BranchingPointsObjects;
        return newSWCObjects;
    }
    void generateV3drawImage(string filePath, Vector3 position){
        string fileToImport = filePath;
        int dimX = 0;
        int dimY = 0;
        int dimZ = 0;
        int bytesToSkip = 43;
        DataContentFormat dataFormat = DataContentFormat.Uint8;
        Endianness endianness = Endianness.LittleEndian;
        if (Path.GetExtension(fileToImport) == ".ini")
            fileToImport = fileToImport.Replace(".ini", ".raw");

        // Try parse ini file (if available)
        DatasetIniData initData = DatasetIniReader.ParseIniFile(fileToImport + ".ini");
        if (initData != null)
        {
            dimX = initData.dimX;
            dimY = initData.dimY;
            dimZ = initData.dimZ;
            bytesToSkip = initData.bytesToSkip;
            dataFormat = initData.format;
            endianness = initData.endianness;
        }

        string formatKey = "raw_image_stack_by_hpeng";
        byte[] byteArray = File.ReadAllBytes(fileToImport);
        byte[] stringByteArray = byteArray.Take(24).ToArray();
        string readFormatKey = System.Text.Encoding.UTF8.GetString(stringByteArray);
        // Debug.Log("reading header...");
        if (readFormatKey != formatKey)
        {
            // Debug.Log(readFormatKey);
            Debug.LogError("This format is not supported");
        }
        string end = Convert.ToChar(byteArray[24]).ToString();
        if (end == "L"){
            endianness = Endianness.LittleEndian;
        }else if(end == "B"){
            endianness = Endianness.BigEndian;
        }else{
            Debug.Log(end);
            Debug.LogError("This endianness is not supported"); 
        }
        // byte dataType = [25];
        switch (byteArray[25])
        {
            case 1:
                dataFormat = DataContentFormat.Uint8;
                break;
            case 2:
                dataFormat = DataContentFormat.Uint16;
                break;
            default:
                Debug.Log("unsupported data type");
                break;
        }
        Int32[] size = new Int32[4];
        for (int i = 0; i < 4; i++)
        {
            int offset = 27+i*4;
            int length = 4;
            byte[] subArray = byteArray.SubArray(offset,length);
            // if (BitConverter.IsLittleEndian)
                // Array.Reverse(subArray);
            Int32 number = BitConverter.ToInt32(subArray,0);
            size[i] = number;
        }
        dimX = size[0];
        dimY = size[1];
        dimZ = size[2];

        xsize = dimX;
        ysize = dimY;
        zsize = dimZ;

        // import image
        V3DRawDatasetImporter importer = new V3DRawDatasetImporter(fileToImport,dimX,dimY,dimZ,dataFormat,endianness,bytesToSkip);
        VolumeDataset newDataset = importer.Import();
        dataset = newDataset;
        string filepath = Application.streamingAssetsPath + "/ill_darkred.tf2d";

        drawImageWithColorPath(dataset,filepath);
    }

    void drawImageWithColorPath(VolumeDataset dataset,string filePath)
    {
        if (dataset != null)
        {
            v3drawImage = VolumeObjectFactory.CreateObject(dataset);
            VolumeRenderedObject obj = v3drawImage.GetComponent<VolumeRenderedObject>();
            obj.SetRenderMode(RenderMode.MaximumIntensityProjectipon);
            // v3drawImage.SetTransferFunctionMode(TFRenderMode.TF2D);
            // if(filePath != "")
            // {
            //     TransferFunction2D newTF = TransferFunctionDatabase.LoadTransferFunction2D(filePath); 
            //     if(newTF != null)
            //     {
            //         v3drawImage.transferFunction2D = newTF;
            //     }
            // }
           
            v3drawImage.transform.localScale = Vector3.one * imageScale;
            v3drawImage.transform.position = imagePosition;  
        }
    }
    void generateMapV3drawImage(string filePath, Vector3 position=default)
    {
        string fileToImport = filePath;
        int dimX = 0;
        int dimY = 0;
        int dimZ = 0;
        int bytesToSkip = 43;
        DataContentFormat dataFormat = DataContentFormat.Uint8;
        Endianness endianness = Endianness.LittleEndian;
        if (Path.GetExtension(fileToImport) == ".ini")
            fileToImport = fileToImport.Replace(".ini", ".raw");

        // Try parse ini file (if available)
        DatasetIniData initData = DatasetIniReader.ParseIniFile(fileToImport + ".ini");
        if (initData != null)
        {
            dimX = initData.dimX;
            dimY = initData.dimY;
            dimZ = initData.dimZ;
            bytesToSkip = initData.bytesToSkip;
            dataFormat = initData.format;
            endianness = initData.endianness;
        }

        string formatKey = "raw_image_stack_by_hpeng";
        byte[] byteArray = File.ReadAllBytes(fileToImport);
        byte[] stringByteArray = byteArray.Take(24).ToArray();
        string readFormatKey = System.Text.Encoding.UTF8.GetString(stringByteArray);
        // Debug.Log("reading header...");
        if (readFormatKey != formatKey)
        {
            // Debug.Log(readFormatKey);
            Debug.LogError("This format is not supported");
        }
        string end = Convert.ToChar(byteArray[24]).ToString();
        if (end == "L")
        {
            endianness = Endianness.LittleEndian;
        }
        else if (end == "B")
        {
            endianness = Endianness.BigEndian;
        }
        else
        {
            Debug.Log(end);
            Debug.LogError("This endianness is not supported");
        }
        // byte dataType = [25];
        switch (byteArray[25])
        {
            case 1:
                dataFormat = DataContentFormat.Uint8;
                break;
            case 2:
                dataFormat = DataContentFormat.Uint16;
                break;
            default:
                Debug.Log("unsupported data type");
                break;
        }
        Int32[] size = new Int32[4];
        for (int i = 0; i < 4; i++)
        {
            int offset = 27 + i * 4;
            int length = 4;
            byte[] subArray = byteArray.SubArray(offset, length);
            // if (BitConverter.IsLittleEndian)
            // Array.Reverse(subArray);
            Int32 number = BitConverter.ToInt32(subArray, 0);
            size[i] = number;
        }
        dimX = size[0];
        dimY = size[1];
        dimZ = size[2];

        xsize = dimX;
        ysize = dimY;
        zsize = dimZ;

        // import image
        V3DRawDatasetImporter importer = new V3DRawDatasetImporter(fileToImport, dimX, dimY, dimZ, dataFormat, endianness, bytesToSkip);
        VolumeDataset newDataset = importer.Import();
        dataset = newDataset;
        string filepath = Application.streamingAssetsPath + "/ill_darkred.tf2d";

        drawMapImageWithColorPath(dataset, filepath, position);
    }

    void drawMapImageWithColorPath(VolumeDataset dataset, string filePath,Vector3 position=default)
    {
        if (dataset != null)
        {
            v3drawImage = VolumeObjectFactory.CreateObject(dataset);
            VolumeRenderedObject obj = v3drawImage.GetComponent<VolumeRenderedObject>();
            obj.SetRenderMode(RenderMode.MaximumIntensityProjectipon);
            // v3drawImage.SetTransferFunctionMode(TFRenderMode.TF2D);
            // if(filePath != "")
            // {
            //     TransferFunction2D newTF = TransferFunctionDatabase.LoadTransferFunction2D(filePath); 
            //     if(newTF != null)
            //     {
            //         v3drawImage.transferFunction2D = newTF;
            //     }
            // }

            v3drawImage.transform.localScale = Vector3.one * imageScale;
            if(position==default)
                v3drawImage.transform.position = imagePosition;
            else
            {
                v3drawImage.transform.position = position;
            }
        }
    }

    void changeRawImageColor()
    {
        string filepath = Application.streamingAssetsPath + "/transparentBlue.tf2d";
        TransferFunction2D tf = TransferFunctionDatabase.LoadTransferFunction2D(filepath);
        if (tf != null)
        {
            Debug.Log("change transfer function");
            v3drawImage.transferFunction2D = tf;
        }
    }
    
    public void hitBp(GameObject bpswc,bool isWrong=false)
    {
        foreach (var SwcGameObject in _swcGameObjects)
        {
            var BranchingPointsObjects = SwcGameObject.BranchingPointsObjects;
            if (BranchingPointsObjects.Contains(bpswc))
            {
                // change bpType to valid
                if (isWrong == false)
                {
                    Vector3 position = bpswc.transform.position;
                    markSWCAsType(bpswc.name, BpType.validBp);
                    Debug.Log(bpswc.name + " mark as valid");
                    BranchingPointsObjects.Remove(bpswc);// create new bp
                    GameObject newBp = Instantiate(this.newBp, position, this.newBp.transform.rotation);
                    SwcGameObject.newBranchingPointsByPlayer.Add(newBp);
                }
                else
                {
                    Vector3 position = bpswc.transform.position;
                    GameObject newBp = Instantiate(this.WrongBp, position, Quaternion.identity);
                    markSWCAsType(bpswc.name, BpType.invalidBp);
                    Debug.Log(bpswc.name + " mark as invalid");
                    // wrongBpSet.Add(swcIndex[bpswc]);
                    BranchingPointsObjects.Remove(bpswc);
                    SwcGameObject.newBranchingPointsByPlayer.Add(newBp);
                }
                
                float totalBp = _swcStructures[0].BranchingPoint.Count;
                float remainingPercetageBp = (BranchingPointsObjects.Count / totalBp);
                // Debug.Log("percentage: " + remainingPercetageBp);
                if (remainingPercetageBp <= 0.25f)
                {
                     if(GameConfig.Instance.GetGameMode() != Mode.Axon)
                        state = SWCState.toBeExploded;
                     GameObject soma = SwcGameObject.somaSphere;
                     if (soma != null)
                     {
                         GameObject newSoma = Instantiate(somaMonster, soma.transform.position, quaternion.identity);
                         newSoma.transform.Rotate(0, 180, 0); // face to front
                         newSoma.transform.localScale = Vector3.one * 2;
                         SwcGameObject.somaSphere = newSoma;
                         Destroy(soma);
                     }
                     storeBranchPosition(); 
                }
                else if (remainingPercetageBp <= 0.5f)
                {
                }
                else if (remainingPercetageBp <= 0.75f)
                {
                     StartCoroutine(changeSomaColorTo(somaLinearColors[3]));
                }
                 

            }
            Destroy(bpswc, 0.2f);
        }
    }
    public List<int> GetWrongBps()
    {
        return wrongBpSet;
    }
    public void hitNormalSWCNode(GameObject node)
    {
        // mark node as missing bp
        Vector3 position = node.transform.position;
        markSWCAsType(node.name,BpType.missingBp);
        // Debug.Log("mark as missing");

        // make new bp
        GameObject newBp = Instantiate(this.newBp, position, this.newBp.transform.rotation);
        _swcGameObjects[0].newBranchingPointsByPlayer.Add(newBp);
    }
    
    public string GetBpAnnotation()
    {
        string result = getCurrentSWCFileName();
        string validBpCount = "v:";
        string invalidBpCount = "i:";
        string missBpCount = "m:";
        string untouched = "u: ";
        // print(_swcs.Count);
        
        foreach (SWC swc in _swcs)
        {
            if (swc.type == BpType.invalidBp)
            {
                invalidBpCount += swc.index.ToString() + ",";
            }else if (swc.type == BpType.validBp)
            {
                validBpCount += swc.index.ToString() + ",";
            }else if (swc.type == BpType.missingBp)
            {
                missBpCount += swc.index.ToString() + ",";
            }else if (swc.type == BpType.untouchedBp)
            {
                untouched += swc.index.ToString() + ", "; // no need for untouched type to be uploaded
            }
        }

        return result + " \n" + validBpCount + " \n" + invalidBpCount + " \n" + missBpCount;
    }

    public int GetMissedBpCount()
    {
        int missBpCount = 0;
        
        foreach (SWC swc in _swcs)
        {
            if (swc.type == BpType.missingBp)
            {
                missBpCount += swc.index;
            }
        }
        return missBpCount;
    }

    private void markSWCAsType(string BpName,BpType type)
    {
        string[] words = BpName.Split("-");
        int index = Int32.Parse(words[1]);
        // print(_swcs.Count);
        for (int i = 0; i < _swcs.Count; i++)
        {
            if (_swcs[i].index == index)
            {
                _swcs[i].type = type;
                // print("mark success");
            }
        }
        
    }

    private bool nearVector3(Vector3 v1, Vector3 v2)
    {
        return Math.Abs(v1.x - v2.x) < 0.005 && Math.Abs(v1.y - v2.y) < 0.005 && Math.Abs(v1.z - v2.z) < 0.005;
    }

    public void explodeSWC(GameObject hitTarget)
    {
        if (state != SWCState.toBeExploded)
        {
            return;
        }

        state = SWCState.exploded;
        if (isSwcShow() == false)
        {
            toggleRAWSwc();
        }
        foreach (SWCGameObjects swcGameObject in _swcGameObjects)
        {
            if (swcGameObject.somaSphere. GetInstanceID() == hitTarget.transform.parent.gameObject.GetInstanceID())
            {
                lastHitSWCObject = swcGameObject; 
            }
            else
            {
                Debug.Log("no game object hit");
            }
        }
        
        Destroy(lastHitSWCObject.somaSphere);
        foreach (GameObject bp in lastHitSWCObject.BranchingPointsObjects)
        {
            bp.AddComponent<Rigidbody>();
        }
        
        foreach (var branchObject in lastHitSWCObject.branchGameObjects)
        {
            Rigidbody rb = branchObject.AddComponent<Rigidbody>();
        }
        
        // change raw image color
        GameObject imageBlock = GameObject.Find("VolumeRenderedObject_" + dataset.datasetName);
        Destroy(imageBlock);
        string filepath = Application.streamingAssetsPath + "/transparentBlue.tf2d";
        drawImageWithColorPath(dataset,filepath);
        
        //create coin to show the direction
        foreach(var cp in bm.coinList.GetComponentsInChildren<Transform>(true))
        {
            gm.CreateCoin(cp.transform.position);
        }
        
        // open door 
        GameObject.Find("DoorAni").SendMessage("openDoor");
    }

    public void destroyLastHitSWC()
    {
        // clear objects in scene
        foreach (Transform child in imageAndSWC.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void toggleRAWImage()
    {
        GameObject imageBlock = GameObject.Find("VolumeContainer(Clone)");
        MeshRenderer mr = imageBlock.GetComponent<MeshRenderer>();
        // Text keyf = GameObject.Find("KeyF").GetComponent<Text>();
        if (showRawImage)
        {
            showRawImage = false;
            mr.enabled = false;
            // keyf.color = Color.red;
        }
        else
        {
            showRawImage = true;
            mr.enabled = true;
            // keyf.color = Color.green;
        }
    }
    public bool isSwcShow()
    {
        return showRawSwc;
    }
    public void toggleRAWSwc()
    {
        GameObject swcBlock = GameObject.Find("ImageAndSWC");
        //MeshRenderer mr = swcBlock.GetComponent<MeshRenderer>();
        // Text keyk = GameObject.Find("KeyK").GetComponent<Text>();
        if (showRawSwc)
        {
            showRawSwc = false;
            //mr.enabled = false;
            swcBlock.transform.localScale = Vector3.zero;
            // keyk.color = Color.red;
        }
        else
        {
            showRawSwc = true;
            //mr.enabled = true;
            swcBlock.transform.localScale = Vector3.one;
            if (GameConfig.Instance.GetGameMode() != Mode.Axon)
            {
                if (state != SWCState.normal)
                {
                    for (int i = 0; i < _swcGameObjects[0].branchGameObjects.Count; i++)
                    {
                        _swcGameObjects[0].branchGameObjects[i].transform.position = _swcGameObjects[0].branchOriginalPositions[i];
                    }
                }

            }

            // keyk.color = Color.green;
        }
    }

    // public int GetCurrentImageIndex()
    // {
    //     return currentImageIndex;
    // }
    
    
    private void shakeBranch(SWCGameObjects swc)
    {
        for (int i = 0; i < swc.branchGameObjects.Count; i++)
        {
            float step = shakeBranchIntensity * Time.deltaTime;
            Vector3 oldPosition = swc.branchGameObjects[i].transform.position;
            Vector3 newPosition = swc.branchOriginalPositions[i] + Random.insideUnitSphere * shakeBranchIntensity;
            swc.branchGameObjects[i].transform.position = Vector3.MoveTowards(oldPosition, newPosition, step);
        }
    }

    private void initColors()
    {
        Color color25 = new Color(0.63f, 0.45f, 0.04f, 1.0f);
        Color color50 = new Color(0.71f, 0.36f, 0.03f, 1.0f);
        Color color75 = new Color(0.83f, 0.20f, 0.02f, 1.0f);
        Color color100 = new Color(0.99f, 0.01f, 0.01f, 1.0f);
        somaLinearColors = new List<Color>();
        somaLinearColors.Add(color25);
        somaLinearColors.Add(color50);
        somaLinearColors.Add(color75);
        somaLinearColors.Add(color100);
    }

    private IEnumerator changeSomaColorTo(Color newColor)
    {
        // Debug.Log("change color to" + newColor);
        float step = 0f;
        if (_swcGameObjects[0].somaSphere != null)
        {
            GameObject soma = _swcGameObjects[0].somaSphere.transform.GetChild(0).gameObject;
            Renderer ren = soma.GetComponent<Renderer>();
            while (ren != null && ren.material.color != newColor)
            {
                step += Time.deltaTime *0.05f;
                Color oldColor = ren.material.color;
                ren.material.color = Color.Lerp(oldColor, newColor, step);
                yield return new WaitForSeconds(.2f);
            }
        }
        
    }
    
    private void storeBranchPosition()
    {
        _swcGameObjects[0].branchOriginalPositions.Clear();
        foreach (GameObject branch in _swcGameObjects[0].branchGameObjects)
        {
            _swcGameObjects[0].branchOriginalPositions.Add(branch.transform.position);
        }
    }
    
    private int[] shuffle(int length,int leaveOut = 0) // leave the first leaveOut number index unchanged
    {
        int[] array = new int[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = i;
        }
        for (int i = length-1; i > leaveOut-1; i--)
        {
            int rnd = Random.Range(leaveOut, i);
            (array[i], array[rnd]) = (array[rnd],array[i]);
        }

        return array;
    }

    private NeuronTree readSWC_file(string filename)
    {
        NeuronTree nt = new NeuronTree();
        List<NeuronSWC> listNeuron = new List<NeuronSWC>();
        Dictionary<long, long> hashNeuron = new Dictionary<long, long>();
        string swcmsg = File.ReadAllText(filename, Encoding.UTF8);
        string[] lines = new string[] { };
        lines = swcmsg.Split('\n');
        foreach(string line in lines)
        {
            if (line.Length <= 0 || line[0] == '#')
                continue;
            string[] qsl = line.Split(" ");
            if (qsl.Length == 0)
                continue;
            NeuronSWC S = new NeuronSWC();
            S.fea_val = new List<float>();
            //Debug.Log(qsl.Length);
            //Debug.Log(qsl);
            for (int i = 0; i < qsl.Length; i++)
            {
                //Debug.Log(i);
                //Debug.Log(qsl[i]);
                switch (i)
                {
                    case 0:
                        S.n = int.Parse(qsl[i]);
                        break;
                    case 1:
                        S.type = int.Parse(qsl[i]);
                        break;
                    case 2:
                        S.x = float.Parse(qsl[i]);
                        break;
                    case 3:
                        S.y = float.Parse(qsl[i]);
                        break;
                    case 4:
                        S.z = float.Parse(qsl[i]);
                        break;
                    case 5:
                        S.r = float.Parse(qsl[i]);
                        break;
                    case 6:
                        S.pn = int.Parse(qsl[i]);
                        break;
                    //case 7:
                    //    S.seg_id = long.Parse(qsl[i]);
                    //    break;
                    //case 8:
                    //    S.level = long.Parse(qsl[i]);
                    //    break;
                    //case 9:
                    //    S.createmode = long.Parse(qsl[i]);
                    //    break;
                    //case 10:
                    //    S.timestamp = double.Parse(qsl[i]);
                    //    break;
                    //case 11:
                    //    S.tfresindex = double.Parse(qsl[i]);
                    //    break;
                    //default:
                    //    S.fea_val.Add(float.Parse(qsl[i]));
                    //    break;
                }
            }
            listNeuron.Add(S);
            hashNeuron.Add(S.n, listNeuron.Count - 1);
        }
        if (listNeuron.Count < 1)
            return nt;
        nt.n = 1;
        nt.listNeuron = listNeuron;
        nt.hashNeuron = hashNeuron;
        nt.color = new Color(0, 0, 0, 0);
        nt.on = true;
        nt.name = filename;
        return nt;
    }

    NeuronTree resample(NeuronTree input,double step)
    {
        NeuronTree result = new NeuronTree();
        result.listNeuron = new List<NeuronSWC>();
        result.hashNeuron = new Dictionary<long, long>();
        List<NeuronSWC> tree = new List<NeuronSWC>();
        long siz= input.listNeuron.Count;
        for(long i = 0; i < siz; i++)
        {
            NeuronSWC s = input.listNeuron[(int)i];
            NeuronSWC pt = new NeuronSWC();
            pt.x = s.x;
            pt.y = s.y;
            pt.z = s.z;
            pt.r = s.r;
            pt.type = s.type;
            pt.seg_id = s.seg_id;
            pt.level = s.level;
            pt.fea_val = s.fea_val;
            pt.p = null;
            pt.childNum = 0;
            tree.Add(pt);
        }
        for(long i = 0; i < siz; i++)
        {
            if (input.listNeuron[(int)i].pn < 0)
                continue;
            long pid = input.hashNeuron[input.listNeuron[(int)i].pn];
            tree[(int)i].p = tree[(int)pid];
            tree[(int)pid].childNum++;
        }
        List<NeuronTree> seg_list = new List<NeuronTree>();
        for(long i = 0; i < siz; i++)
        {
            if (tree[(int)i].childNum != 1)
            {
                NeuronTree seg = new NeuronTree();
                seg.listNeuron = new List<NeuronSWC>();
                NeuronSWC cur = tree[(int)i];
                do
                {
                    seg.listNeuron.Add(cur);
                    cur = cur.p;
                } while (cur != null && cur.childNum == 1);
                seg_list.Add(seg);
            }
        }
        for(long i = 0; i < seg_list.Count; i++)
        {
            seg_list[(int)i] = resample_path(seg_list[(int)i], step);
        }
        tree.Clear();
        Dictionary<NeuronSWC, long> index_map = new Dictionary<NeuronSWC, long>();
        for(long i = 0; i < seg_list.Count; i++)
        {
            for(long j = 0; j < seg_list[(int)i].listNeuron.Count; j++)
            {
                tree.Add(seg_list[(int)i].listNeuron[(int)j]);
                index_map.Add(seg_list[(int)i].listNeuron[(int)j], tree.Count - 1);
            }
        }
        for(long i = 0; i < tree.Count; i++)
        {
            NeuronSWC S = new NeuronSWC();
            NeuronSWC p = tree[(int)i];
            S.n = i + 1;
            if (p.p == null)
                S.pn = -1;
            else
                S.pn = index_map[p.p] + 1;
            S.x = p.x;
            S.y = p.y;
            S.z = p.z;
            S.r = p.r;
            S.type = p.type;
            S.seg_id = p.seg_id;
            S.level = p.level;
            S.fea_val = p.fea_val;
            result.listNeuron.Add(S);
        }
        for (long i = 0; i < result.listNeuron.Count; i++)
            result.hashNeuron.Add(result.listNeuron[(int)i].n, i);
        return result;
    }

    NeuronTree resample_path(NeuronTree seg,double step)
    {
        NeuronTree seg_r = new NeuronTree();
        seg_r.listNeuron = new List<NeuronSWC>();
        double path_length = 0;
        NeuronSWC start = seg.listNeuron[0];
        NeuronSWC seg_par = seg.listNeuron[seg.listNeuron.Count - 1].p;
        long iter_old = 0;
        seg_r.listNeuron.Add(start);
        while (iter_old < seg.listNeuron.Count && start!=null && start.p!=null)
        {
            path_length += DISTP(start, start.p);
            if (path_length <= seg_r.listNeuron.Count * step)
            {
                start = start.p;
                iter_old++;
            }
            else
            {
                path_length -= DISTP(start, start.p);
                NeuronSWC pt = new NeuronSWC();
                double rate = (seg_r.listNeuron.Count * step - path_length) / (DISTP(start, start.p));
                pt.x = start.x + (float)rate * (start.p.x - start.x);
                pt.y = start.y + (float)rate * (start.p.y - start.y);
                pt.z = start.z + (float)rate * (start.p.z - start.z);
                pt.r = start.r * (1 - (float)rate) + start.p.r * (float)rate;
                pt.p = start.p;
                pt.type = start.type;
                if (rate < 0.5)
                {
                    pt.seg_id = start.seg_id;
                    pt.level = start.level;
                    pt.fea_val = start.fea_val;
                }
                else
                {
                    pt.seg_id = start.p.seg_id;
                    pt.level = start.p.level;
                    pt.fea_val = start.p.fea_val;
                }
                seg_r.listNeuron[seg_r.listNeuron.Count - 1].p = pt;
                seg_r.listNeuron.Add(pt);
                path_length += DISTP(start, pt);
                start = pt;
            }
        }
        seg_r.listNeuron[seg_r.listNeuron.Count - 1].p = seg_par;
        seg = seg_r;
        return seg;
    }

    double DISTP(NeuronSWC a,NeuronSWC b)
    {
        Vector3 pa, pb;
        pa = new Vector3(a.x, a.y, a.z);
        pb = new Vector3(b.x, b.y, b.z);
        return (pa - pb).magnitude;
    }

    string export_list2string(List<NeuronSWC> ln)
    {
        string result = "";
        result += "# id,type,x,y,z,r,pid\n";
        for (int i = 0; i < ln.Count; i++)
        {
            string temp = "";
            temp += ln[i].n.ToString() + " " + ln[i].type.ToString() + " " + ln[i].x.ToString() + " " + ln[i].y.ToString() + " " + ln[i].z.ToString() + " " + ln[i].r.ToString() + " " + ln[i].pn.ToString() + "\n";
            result += temp;
        }
        //Debug.Log(result);
        //sort_swc(result);
        return result;
    }

    void MapHightLight(Vector3 center_position,Vector3 block_size, int xsize = 256, int ysize = 256, int zsize = 256)
    {
        float x = center_position.x * imageScale / xsize;
        float y = center_position.y * imageScale / ysize;
        float z = center_position.z * imageScale / zsize;
        float xscale = block_size.x * imageScale / xsize;
        float yscale = block_size.y * imageScale / ysize;
        float zscale = block_size.z * imageScale / zsize;
        Vector3 pos = new Vector3(x, y, z);
        Debug.Log(pos);
        if (highlight)
            DestroyImmediate(highlight);
        highlight = Instantiate(hightlight,pos,Quaternion.identity);

        highlight.transform.SetParent(MapHighLight.transform);
        highlight.transform.localPosition = pos;
        highlight.transform.localScale = new Vector3(xscale, yscale, zscale);
        // print("update highlight cube");
    }

}
