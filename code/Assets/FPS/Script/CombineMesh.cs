using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineMesh : MonoBehaviour
{
    public GameObject newParent;
    // Start is called before the first frame update
    void Start()
    {
        MergeMesh(newParent, false); ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void MergeMesh(GameObject parent, bool mergeSubMeshes = false)
    {
        MeshRenderer[] meshRenderers = parent.GetComponentsInChildren<MeshRenderer>();
        Material[] materials = new Material[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            materials[i] = meshRenderers[i].sharedMaterial;
        }
        MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();   
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length]; 
        for (int i = 0; i < meshFilters.Length; i++)                                  
        {
            combineInstances[i].mesh = meshFilters[i].sharedMesh; 
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix * meshFilters[i].transform.worldToLocalMatrix; 
            GameObject.DestroyImmediate(meshFilters[i].gameObject);
        }
        Mesh newMesh = new Mesh();                                  
        newMesh.CombineMeshes(combineInstances, mergeSubMeshes, true);   
        parent.AddComponent<MeshFilter>().sharedMesh = newMesh; 
        parent.AddComponent<MeshRenderer>().sharedMaterials = materials;
        parent.AddComponent<MeshCollider>();
    }
}
