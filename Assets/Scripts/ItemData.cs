using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Settings/ItemData")]
public class ItemData: ScriptableObject
{
    public List<InventoryItemData> data = new List<InventoryItemData>();
}

[System.Serializable]
public class InventoryItemData
{
    public string name;
    public int maxValue = 9;
    public Sprite sprite;
    public UnityEvent onUse;
}
