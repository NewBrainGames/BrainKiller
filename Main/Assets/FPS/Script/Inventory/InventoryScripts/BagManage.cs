using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagManage : MonoBehaviour
{
    static BagManage instance;
    public ItemsListController myBag;
    public Slot slotPrefab;
    public GameObject slotGrid;
    public Text itemInformation;
    void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }
    private void OnEnable()
    {
        RefreshItem();
        instance.itemInformation.text = "This is for items description";
        itemInformation.fontSize = 25;
        itemInformation.alignment = TextAnchor.MiddleCenter;
    }
    public static void updateItemInfo(string itemDescription)
    {
        instance.itemInformation.text = itemDescription;
        instance.itemInformation.fontSize = 20;
        instance.itemInformation.alignment = TextAnchor.MiddleCenter;

    }
    public static void CreateNewItem(ItemsController item)
    {
        Slot newItem = Instantiate(instance.slotPrefab, instance.slotGrid.transform.position, Quaternion.identity);
        newItem.gameObject.transform.SetParent(instance.slotGrid.transform);
        newItem.slotItem = item;
        newItem.slotImage.sprite = item.itemImage;
        newItem.transform.GetComponent<MeshFilter>().sharedMesh = item.mesh;
        newItem.transform.GetComponent<Renderer>().materials[0] = item.material;
        newItem.slotNum.text = item.itemHeld.ToString();
    }
    public static void RefreshItem()
    {
        for (int i = 0; i < instance.slotGrid.transform.childCount; i++)
        {
            //print(instance.slotGrid.transform.childCount);
            if (instance.slotGrid.transform.childCount == 0)
                break;
            Destroy(instance.slotGrid.transform.GetChild(i).gameObject);
            //instance.slots.Clear();
        }
        for (int i = 0; i < instance.myBag.ItemsList.Count; i++)
        {
            if (instance.myBag.ItemsList[i] != null)
            {
                CreateNewItem(instance.myBag.ItemsList[i]);
            }

        }
    }
}
