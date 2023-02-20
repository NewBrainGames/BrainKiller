using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Items", menuName = "Inventory/Items")]
public class ItemsController : ScriptableObject
{
    public string itemName;
    public Sprite itemImage;
    public Mesh mesh;
    public Material material;
    public int itemHeld;
    [TextArea]//确定多少行描述
    public string itemInfo;//单行描述
    public bool equip;
}
