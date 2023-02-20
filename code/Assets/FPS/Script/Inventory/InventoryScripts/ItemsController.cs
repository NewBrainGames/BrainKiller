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
    [TextArea]//ȷ������������
    public string itemInfo;//��������
    public bool equip;
}
