using Unity.VisualScripting;
using UnityEngine;

class SwcGenPractice : CommonSwcGenerator
{

    public GameObject newBp;
    
    protected override void Start()
    {
        base.Start();
        newBp = GameObject.Find("newBP");
        base.drawSWC(base.GetCurrentImageIndex());
        base.drawRawImage(base.GetCurrentImageIndex());
    }

    protected override void Update()
    {
        base.Update();
    }


    protected override void rotateSWC()
    {
        //put nothing to not rotate 
    }
    
    void desNewBp()
    {
        base._swcGameObjects[0].newBranchingPointsByPlayer.Clear();
    }
    
    public void addNewBp(GameObject bpswc)
    {
        foreach (var SwcGameObject in _swcGameObjects)
        {
            var BranchingPointsObjects = SwcGameObject.BranchingPointsObjects;
            if (BranchingPointsObjects.Contains(bpswc))
            {
                // change bpType to valid
                Vector3 position = bpswc.transform.position;
                markSWCAsType(position,BpType.validBp);
                // Debug.Log("mark as valid");
                BranchingPointsObjects.Remove(bpswc);
                // create new bp
                GameObject newBp = Instantiate(this.newBp, position, this.newBp.transform.rotation);
                SwcGameObject.newBranchingPointsByPlayer.Add(newBp);
            }
            Destroy(bpswc, 0.2f);
        }
    }
    
    private void markSWCAsType(Vector3 position,BpType type)
    {
        // for (int i = 0; i < _swcs.Count; i++)
        // {
        //     Vector3 branchOffset = GameObject.Find("Branch_0").transform.position;
        //     Vector3 rawPosition = position;
        //     Vector3 swcPosition = _swcs[i].pos + branchOffset;
        //     if ( nearVector3(swcPosition, rawPosition))
        //     {
        //         // Debug.Log("marked one bp success");
        //         _swcs[i].type = type;
        //     }
        // }
        //
    }

    public override void explodeSWC(GameObject hitTarget)
    {
        desNewBp();
        base.explodeSWC(hitTarget);
    }
}