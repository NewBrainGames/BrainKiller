using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
// using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms;
using UnityVolumeRendering;
using Random = UnityEngine.Random;
using System.Collections;

public class CommonSwcGenerator : MonoBehaviour
{
    // image size parameter todo get from load v3draw
    public int xsize=96;
    public int ysize=96;
    public int zsize=48;
    
    public int imageScale = 10;
    
    private int currentImageIndex;
    
    // bp rotation speed
    [FormerlySerializedAs("_speed")] public float bpRotationSpeed = 1f;
    public float swcRotationSpeed = 0.2f;
    public float swcRotationSpeedMax = 0.6f;
    public bool animateSWCFloat;
    public float shakeBranchIntensity = 3f;
    // private float shakeDecay = 0.003f;
    private Vector3 originalBranchPosition;

    private List<Color> somaLinearColors;
     
    public GameObject imageAndSWC; // todo generate from code
    public Vector3 imagePosition;
    protected SWCState state;
    
    // swc data structures:
    private int numberOfSWCStructure = 1;
    private List<SWCStructure> _swcStructures;
    protected List<SWCGameObjects> _swcGameObjects;
    SWCGameObjects lastHitSWCObject = null;
    
    // gameobject rendered in scene
    public GameObject BpSwc;  
    public GameObject swcNodeObject;
    public GameObject swcLinkObject;
    public GameObject somaSphereObject;
    
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
    
    protected virtual void Start()
    {
        bindObjects();
        initParameters(); 
    }

    void bindObjects()
    {
        imageAndSWC = GameObject.Find("ImageAndSWC");
        BpSwc = GameObject.Find("virus");
        swcNodeObject = GameObject.Find("SWCNode");
        swcLinkObject = GameObject.Find("SWCLink");
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        somaSphereObject = GameObject.Find("somaSphere");
        gm = GameObject.Find("SceneObjectManager").GetComponent<SceneObjectManager>();
    }

    void initParameters()
    {
        _swcStructures = new List<SWCStructure>(numberOfSWCStructure);
        _swcGameObjects = new List<SWCGameObjects>(numberOfSWCStructure);
        v3drawImageFiles = Directory.GetFiles(streamingAssetFilePath,"*.v3draw");
        swcFiles = Directory.GetFiles(streamingAssetFilePath,"*.swc");
        initColors();
        currentImageIndex = 0;
        // drawSWCandRawImage(currentImageIndex);
        state = SWCState.normal;
    }

    protected void drawSWC(int index)
    {
        // check for index
        if (index < 0 || index > v3drawImageFiles.Length-1 )
        {
           return; 
        }
        
        List<Vector3> positions = new List<Vector3>();
        positions.Add(imagePosition); 
        // check for given position number;
        if (positions.Count != numberOfSWCStructure)
        {
            Debug.LogError("try to draw " + numberOfSWCStructure + " swc but only found " + positions.Count + " position given");
            return;
        }
        
        // read all swc files
        for (int i = 0; i < numberOfSWCStructure; i++)
        {
            int currentIndex = index + i;
            currentIndex = currentIndex % (swcFiles.Length);
            string swcPath = swcFiles[currentIndex];
            string swcmsg = File.ReadAllText(swcPath, Encoding.UTF8);
            SWCStructure newSwcStructure = ParseSwcFile(swcmsg);
            _swcStructures.Add(newSwcStructure);
        }
        // draw all swc and image
        for (int i = 0; i < numberOfSWCStructure; i++)
        {
            SWCGameObjects newSwcGameObjects = GenerateSwcGameObjects(_swcStructures[i]);
            
            _swcGameObjects.Add(newSwcGameObjects);
            imageAndSWC.transform.position = imagePosition;
        }
    }

    protected void drawRawImage(int index)
    {
        // check for indexList
        if (index < 0 || index > v3drawImageFiles.Length-1 )
        {
           return; 
        }
        
        List<Vector3> positions = new List<Vector3>();
        positions.Add(imagePosition);
        
        for (int i = 0; i < numberOfSWCStructure; i++)
        {
            string imagePath = v3drawImageFiles[index];
            generateV3drawImage(filePath: imagePath, position: positions[i]);
        }
    }

    public void nextImage()
    {
        // todo: clear old image
        GameObject imageBlock = GameObject.Find("VolumeRenderedObject_" + dataset.datasetName);
        Destroy(imageBlock);
        _swcStructures.Clear();
        _swcGameObjects.Clear();
        
        destroyLastHitSWC();
        gm.DesCoin();

        // generate new images
        // currentImageIndex += numberOfSWCStructure;
        // currentImageIndex = currentImageIndex%(v3drawImageFiles.Length-1);
        // drawSWC(currentImageIndex);
        // drawRawImage(currentImageIndex);
        lastHitSWCObject = null;
        
        // update game-level
        GameObject.Find("Canvas").GetComponent<UILevel>().UpdateGameLevel(currentImageIndex);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (_swcGameObjects.Count > 0)
        {
            rotateAllBranchingPoint();
        }
        changeBehaviorWithState();
    }

    protected void changeBehaviorWithState()
    {
        switch (state)
        {
            case SWCState.normal:
                rotateSWC();
                break;
            case SWCState.toBeExploded:
                shakeBranch(_swcGameObjects[0]);
                break;
            case SWCState.exploded:
                break;
            default:
                Debug.LogError("invaild swc state");
                break;
        }
    }
    
    void rotateAllBranchingPoint()
    {
        foreach (var swcGameObject in _swcGameObjects)        
        {
            foreach (var point in swcGameObject.BranchingPointsObjects)
            {
                point.transform.Rotate(0, bpRotationSpeed, 0, Space.Self);
            }
        }
    }

    SWCStructure ParseSwcFile(string swcmsg)
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

        foreach (string var in coordinate)
        {
            if (var.Length <= 0 || var[0] == '#')
                continue;
            
            string[] line = var.Split(" ");
            float index = float.Parse(line[0]);
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

            // find branching point
            indexChildrenMap.Add(tswc.index, 0);
            if (tswc.parent != -1)
            {
                // children count ++
                indexChildrenMap[tswc.parent]++;
                if (indexChildrenMap[tswc.parent] == 2)
                {
                    BranchingPoint.Add(tswc);
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
                    swcLink.NodePair = new float[] { index, tswc.parent };

                    // direction 
                    swcLink.Pos = (tswc.pos + lastNode.pos) / 2;
                    swcLink.Direction = tswc.pos - lastNode.pos;
                    tempBranch.LinkList.Add(swcLink);
                }
                else
                {
                    // 找到父亲节点添加connection
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
            BranchingPoint[i] = swc; 
        }

        newSwcStructure.branches = _branchSet;
        newSwcStructure.BranchingPoint = BranchingPoint;

        return newSwcStructure;
    }
    
    SWCGameObjects GenerateSwcGameObjects(SWCStructure swcStructure)
    {
        List<GameObject> BranchingPointsObjects = new List<GameObject>(); 
        List<GameObject> branchGameObjects = new List<GameObject>(); 
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
                    somaSphere.transform.GetChild(0).tag = "soma";
                    somaSphere.transform.localScale = Vector3.one * 4;
                    imageAndSWC.transform.position = somaSphere.transform.position;
                    somaSphere.transform.SetParent(imageAndSWC.transform);
                }

                GameObject node = Instantiate(swcNodeObject, swcNode.pos, Quaternion.identity);
                foreach (Transform o in node.transform)
                {
                    o.gameObject.GetComponent<Animator>().enabled = false;
                }
                node.transform.SetParent(branchGameObject.transform);

            }

            // foreach (var swcLink in branch.LinkList)
            // {
            //     Vector3 position = swcLink.Pos;
            //     Quaternion rotation = Quaternion.LookRotation(swcLink.Direction);
            //
            //     GameObject linkObject = Instantiate(swcLinkObject, position, rotation);
            //
            //     // scale
            //     Vector3 scale = Vector3.one;
            //     var dirMagnitude = swcLink.Direction.magnitude;
            //     scale.z = dirMagnitude;
            //     linkObject.transform.localScale = scale;
            //
            //     linkObject.transform.SetParent(branchGameObject.transform);
            // }
            
            branchGameObject.transform.SetParent(imageAndSWC.transform);
        }

        foreach (SWC bp in swcStructure.BranchingPoint)
        {
            // Debug.Log(bp.n)
            GameObject newBranchingPointObject = (GameObject)Instantiate(BpSwc, bp.pos, Quaternion.identity);
            BranchingPointsObjects.Add(newBranchingPointObject);
            newBranchingPointObject.transform.SetParent(imageAndSWC.transform);
        }

        SWCGameObjects newSWCObjects = new SWCGameObjects();
        newSWCObjects.somaSphere = somaSphere;
        newSWCObjects.branchGameObjects = branchGameObjects;
        newSWCObjects.BranchingPointsObjects = BranchingPointsObjects;
        return newSWCObjects;
    }

    private void storeBranchPosition()
    {
        _swcGameObjects[0].branchOriginalPositions.Clear();
        foreach (GameObject branch in _swcGameObjects[0].branchGameObjects)
        {
            _swcGameObjects[0].branchOriginalPositions.Add(branch.transform.position);
        }
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
            v3drawImage.SetTransferFunctionMode(TFRenderMode.TF2D);
            if(filePath != "")
            {
                TransferFunction2D newTF = TransferFunctionDatabase.LoadTransferFunction2D(filePath); 
                if(newTF != null)
                {
                    v3drawImage.transferFunction2D = newTF;
                }
            }
           
            v3drawImage.transform.localScale = Vector3.one * imageScale;
            v3drawImage.transform.position = imagePosition;  
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
    
    public void desBpSwc(GameObject bpswc)
    {
        foreach (var SwcGameObject in _swcGameObjects)
        {
            var BranchingPointsObjects = SwcGameObject.BranchingPointsObjects;
            if (BranchingPointsObjects.Contains(bpswc))
            {
                BranchingPointsObjects.Remove(bpswc);
            }
            Destroy(bpswc, 0.2f);
            updateSWCRotationSpeedUnderMaximum(0.1f);

            float totalBp = _swcStructures[0].BranchingPoint.Count;
            float remainingPercetageBp = (BranchingPointsObjects.Count / totalBp) ;
            // Debug.Log("percentage: " + remainingPercetageBp);
            if (remainingPercetageBp <= 0.2f)
            {
                StartCoroutine(changeSomaColorTo(somaLinearColors[3]));
                
            }else if (remainingPercetageBp <= 0.25f)
            {
                StartCoroutine(changeSomaColorTo(somaLinearColors[2]));
                state = SWCState.toBeExploded;
                storeBranchPosition();
            }else if(remainingPercetageBp <= 0.5f)
            {
                StartCoroutine(changeSomaColorTo(somaLinearColors[1]));
            }else if (remainingPercetageBp <= 0.75f)
            {
                StartCoroutine(changeSomaColorTo(somaLinearColors[0]));
            }
        }
        
    }

    public virtual void explodeSWC(GameObject hitTarget)
    {
        // todo find out which swcGameObject
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
        if (showRawImage)
        {
            showRawImage = false;
            mr.enabled = false;
        }
        else
        {
            showRawImage = true;
            mr.enabled = true;
        }
    }

    public void toggleRAWSwc()
    {
        GameObject swcBlock = GameObject.Find("ImageAndSWC");
        //MeshRenderer mr = swcBlock.GetComponent<MeshRenderer>();
        if (showRawSwc)
        {
            showRawSwc = false;
            //mr.enabled = false;
            swcBlock.transform.localScale = Vector3.zero;
        }
        else
        {
            showRawSwc = true;
            //mr.enabled = true;
            swcBlock.transform.localScale = Vector3.one;
        }
    }

    public int GetCurrentImageIndex()
    {
        return currentImageIndex;
    }

    protected virtual void rotateSWC()
    {
        imageAndSWC.transform.Rotate(0, swcRotationSpeed, 0, Space.Self);
    }

    private void updateSWCRotationSpeedUnderMaximum(float increase)
    {
        if (swcRotationSpeed <= swcRotationSpeedMax)
        {
            swcRotationSpeed += increase;
        }
    }

    private void animateNodeFloat()
    {
        foreach (Transform child in imageAndSWC.transform)
        {
            string substring = child.gameObject.name.Substring(0, 6);
            if (string.Compare(substring,"Branch") == 0)
            {
                foreach (Transform childNode in child.transform)
                {
                    if (childNode.gameObject.name == "SWCNode(Clone)")
                    {
                        Animator animation = childNode.transform.GetChild(0).GetComponent<Animator>();
                        animation.Play("NodeUpDown");
                    }
                }
            }
        } 
    }

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
        Color color100 = new Color(0.99f, 0.08f, 0.01f, 1.0f);
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
        GameObject soma = _swcGameObjects[0].somaSphere.transform.GetChild(0).gameObject;
        Renderer ren = soma.GetComponent<Renderer>();
        while (ren.material.color != newColor)
        {
            step += Time.deltaTime *0.05f;
            Color oldColor = ren.material.color;
            ren.material.color = Color.Lerp(oldColor, newColor, step);
            yield return new WaitForSeconds(.2f);
        }
    }

    
}
