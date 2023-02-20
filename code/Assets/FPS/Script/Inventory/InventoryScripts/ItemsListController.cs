using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ItemsList", menuName = "Inventory/ItemsList")]
public class ItemsListController : ScriptableObject
{
    public List<ItemsController> ItemsList = new List<ItemsController>();
}
