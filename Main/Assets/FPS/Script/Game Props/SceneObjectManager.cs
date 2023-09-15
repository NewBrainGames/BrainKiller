using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectManager : MonoBehaviour
{

    public GameObject coin;
    public List<GameObject> coinList;
    // Start is called before the first frame update
    void Start()
    {

        //Debug.Log("GameManager");
    }

    // Update is called once per frame 
    void Update()
    {
        
    }

    public void CreateCoin(Vector3 position)
    {
        GameObject newCoin = new GameObject();
        newCoin = Instantiate(coin,position,coin.transform.rotation);
        coinList.Add(newCoin);
    }

    public void DesCoin()
    {
        foreach (var nc in coinList)
        {
            DestroyImmediate(nc);
        }

    }
}
