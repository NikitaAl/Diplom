using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Sprite[] numbers;

    [SerializeField] private ItemData _itemData;
    [SerializeField] private List<InventoryCell> _invCells = new List<InventoryCell>();

    private void Start()
    {
        UpdateSprites();
    }

    private void UpdateSprites()
    {
        var cellIdx = 0;

        for (int i = 0; i < _itemData.data.Count; i++)
        {
            InventoryItemData item = _itemData.data[i];
            var val = Mathf.Clamp(PlayerPrefs.GetInt(item.name, 0), 0, item.maxValue);
            PlayerPrefs.SetInt(item.name, val);

            if (val <= 0) continue;

            _invCells[cellIdx].itemIdx = i;
            _invCells[cellIdx].icon.gameObject.SetActive(true);
            _invCells[cellIdx].icon.sprite = item.sprite;
            _invCells[cellIdx].numberImage.sprite = numbers[val];
            cellIdx++;
        }

        for (int i = cellIdx; i < _invCells.Count; i++)
        {
            _invCells[cellIdx].itemIdx = -1;
            _invCells[cellIdx].icon.gameObject.SetActive(false);
            _invCells[i].icon.sprite = null;
            _invCells[i].numberImage.sprite = null;
        }
    }

    public bool TryPickItem(string itemName, int count = 1)
    {
        if (!PlayerPrefs.HasKey(itemName))
        {
            Debug.LogError("Can't get item " + itemName, this);
            return false;
        }
        var item = _itemData.data.Find(item => item.name == itemName);
        var val = PlayerPrefs.GetInt(itemName, 0);

        if (val >= item.maxValue)
        {
            return false;
        }

        val += count;

        val = Mathf.Clamp(val, 0, item.maxValue);
        PlayerPrefs.SetInt(item.name, val);
        UpdateSprites();

        return true;
    }

    public void UseItem(int cellIdx)
    {
        var itemIdx = _invCells[cellIdx].itemIdx;
        if (itemIdx < 0 || itemIdx >= _itemData.data.Count)
        {
            Debug.Log("No item or incorrect itemIdx");
            return;
        }
        var item = _itemData.data[itemIdx];
        var val = PlayerPrefs.GetInt(item.name);

        if (val > 0)
        {
            PlayerPrefs.SetInt(item.name, val - 1);
            item.onUse.Invoke();
        }

        UpdateSprites();
    }
}

[System.Serializable]
public class InventoryCell
{
    public int itemIdx = -1;
    public Image icon;
    public Image numberImage;
}
