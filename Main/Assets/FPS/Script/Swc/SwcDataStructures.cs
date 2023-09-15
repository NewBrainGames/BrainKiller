using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System;
using UnityEngine;
using UnityVolumeRendering;
using System;

// data structures 
public static class Extensions
{
    public static T[] SubArray<T>(this T[] array, int offset, int length)
    {
        return new ArraySegment<T>(array, offset, length)
                    .ToArray();
    }
}

enum BpType
{
    defaultNode,
    untouchedBp,
    validBp,
    invalidBp,
    missingBp
}

class SWCStructure
{
    public Dictionary<int, Branch> branches = new Dictionary<int, Branch>();
    public List<SWC> BranchingPoint = new List<SWC>();
}

public class SWCGameObjects
{
    public GameObject somaSphere;
    public List<GameObject> BranchingPointsObjects = new List<GameObject>();
    public List<GameObject> newBranchingPointsByPlayer = new List<GameObject>();
    public List<GameObject> branchGameObjects = new List<GameObject>();
    public List<Vector3> branchOriginalPositions = new List<Vector3>();
}

class SWC
{
    public int index;
    public int parent;
    public Vector3 pos;
    public BpType type;
    public int nodetype;
};

struct SWCLink
{
    public float[] NodePair;
    public Vector3 Pos;
    public Vector3 Direction;
    public int n;
    public int p;
};

struct Branch
{
    public List<SWC> NodeList;
    public List<SWCLink> LinkList;
}

public enum SWCState
{
   normal,
   toBeExploded,
   exploded
}

public class NeuronSWC
{
    public long n;
    public int type;
    public float x, y, z, r;
    public long pn;
    public long seg_id;
    public long level;
    public long createmode;
    public double timestamp;
    public double tfresindex;
    public List<float> fea_val;
    //Point
    public NeuronSWC p;
    public long childNum;

    
}

public struct NeuronTree
{
    public long n;
    public List<NeuronSWC> listNeuron;
    public Dictionary<long, long> hashNeuron;
    public Color color;
    public bool on;
    public string name;
    //public string comment;
}

