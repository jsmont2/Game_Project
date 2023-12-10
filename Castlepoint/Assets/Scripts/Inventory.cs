using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Inventory : ScriptableObject, IDataPersistence
{
    public void LoadData(GameData data)
    {
        currentMagic = data.currentMagic;
    }
    public void SaveData(GameData data)
    {
        data.currentMagic = currentMagic;
    }
    public Item currentItem;
    public List<Item> items = new List<Item>();
    public int numberOfKeys;
    public int coins;
    public float maxMagic = 10;
    public float currentMagic;

    public void OnEnable()
    {
        if (currentMagic == 0)
        { currentMagic = maxMagic; }

    }

    public void ReduceMagic(float magicCost)
    {
        currentMagic -= magicCost;
    }

    public bool CheckForItem(Item item)
    {
        if (items.Contains(item))
        {
            return true;
        }
        return false;
    }

    public void AddItem(Item itemToAdd)
    {
        // Is the item a key?
        if (itemToAdd.isKey)
        {
            numberOfKeys++;
        }
        else
        {
            if (!items.Contains(itemToAdd))
            {
                items.Add(itemToAdd);
            }
        }
    }

}
