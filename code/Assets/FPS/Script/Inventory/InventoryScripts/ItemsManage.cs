using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManage : MonoBehaviour
{
    public ItemsController thisItem;
    public ItemsListController playerInventory;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AddItems();
            Destroy(gameObject);
        }
    }
    public void AddItems()
    {

        if (!playerInventory.ItemsList.Contains(thisItem))
        {
            // playerInventory.itemlist.Add(thisItem);
            for (int i = 0; i < playerInventory.ItemsList.Count; i++)
            {
                if (playerInventory.ItemsList[i] == null)
                {
                    playerInventory.ItemsList[i] = thisItem;
                    //InventoryManage.CreateNewItem(thisItem);
                    break;
                }
            }
            thisItem.itemHeld = 1;

            //InventoryManage.CreateNewItem(thisItem);
        }
        else
        {
            thisItem.itemHeld += 1;

        }
        BagManage.RefreshItem();
    }
}
