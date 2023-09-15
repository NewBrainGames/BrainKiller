using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public ItemsController slotItem;
    public ItemsListController playerInventory;
    public Image slotImage;
    public Text slotNum;
    public GameObject itemPicture;

    // Start is called before the first frame update
    void Start()
    {
        itemPicture = GameObject.Find("ItemPicture");
    }

    public void ItemOnClicked()
    {
        BagManage.updateItemInfo(slotItem.itemInfo);
        itemPicture.GetComponent<Image>().sprite = slotItem.itemImage;
        slotItem.itemHeld--;
        slotNum.text = slotItem.itemHeld.ToString();
        if( slotItem.itemHeld<=0)
        {
            playerInventory.ItemsList.Remove(slotItem);
            playerInventory.ItemsList.Add(null);
            BagManage.RefreshItem();
            //for (int i = 0; i < playerInventory.ItemsList.Count; i++)
            //{
            //    if (playerInventory.ItemsList[i] == slotItem)
            //    {
            //        playerInventory.ItemsList[i] = thisItem;
            //        //InventoryManage.CreateNewItem(thisItem);
            //        break;
            //    }
            //}
        }

        //sr.transform.GetComponent<MeshFilter>().sharedMesh = slotItem.mesh;
        //sr.transform.GetComponent<Renderer>().materials[0] = slotItem.material;

    }
}
