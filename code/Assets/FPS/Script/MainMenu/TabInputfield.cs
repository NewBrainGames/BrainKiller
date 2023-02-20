using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TabInputfield : MonoBehaviour
{
    // Start is called before the first frame update
    EventSystem system;

    private List<GameObject> inputList;
    void Start ()
    {

        system = EventSystem.current;
        inputList = new List<GameObject>();
        InputField[] array = transform.GetComponentsInChildren<InputField>();
        Debug.Log("array" + array);
        for (int i = 0; i < array.Length; i++)
        {
            Debug.Log("enter");
            inputList.Add(array[i].gameObject);
            Debug.Log("inputList"+inputList[i].name);
        }
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("press Tab");
            if (inputList.Contains(system.currentSelectedGameObject))
            {
                Debug.Log("press Tab successfully");
                GameObject next = NextInput(system.currentSelectedGameObject);
                system.SetSelectedGameObject(next);
            }
        }
    }

    private GameObject LastInput(GameObject input)
    {
        int indexNow = IndexNow(input);
        if (indexNow - 1 >= 0)
        {
            return inputList[indexNow - 1].gameObject;
        }
        else
        {
            return inputList[inputList.Count - 1].gameObject;
        }
    }
    
    private int IndexNow(GameObject input)
    {
        int indexNow = 0;
        for(int i =0;i<inputList.Count;i++)
        {
            if(input == inputList[i])
            {
                indexNow = i;
                break;
            }
        }
        return indexNow;
        
    }

    private GameObject NextInput(GameObject input)
    {
        int indexNow = IndexNow(input);
        if (indexNow + 1 < inputList.Count)
        {
            return inputList[indexNow + 1].gameObject;
        }
        else
        {
            return inputList[0].gameObject;
        }
    }

  

}
