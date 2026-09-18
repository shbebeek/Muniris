using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Inventory : ScriptableObject
{
    public GameObject currentLeftItem;
    public GameObject currentRightItem;
    public List<GameObject> leftItems = new List<GameObject>();
    public List<GameObject> rightItems = new List<GameObject>();
    public List<Item> items = new List<Item>();
    public int numberOfKeys;
    public int taps; // standard coin
    public int joules; // rare currency
    public int maxHealth = 100;
    public int maxStamina = 100;
    public float maxMagic = 10;
    public float currentMagic;
    private InventoryMenu invMenu;

    public void OnEnable()
    {
        currentMagic = maxMagic;

    }

    public void ReduceMagic(float magicCost)
    {
        currentMagic -= magicCost;
    }

    public void GainMagic(float magicAmount)
    {
        if(currentMagic <= maxMagic)
        {
            currentMagic += magicAmount;
        }
        else
        {
            currentMagic = maxMagic;
        }
    }

    public bool CheckForItem(Item item)
    {
        if (items.Contains(item))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddItem(Item itemToAdd)
    {
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

    public void AddLeftHandItem(GameObject itemToAdd)
    {
        if (!leftItems.Contains(itemToAdd))
        {
            leftItems.Add(itemToAdd);
        }
    }

    public void AddRightHandItem(GameObject itemToAdd)
    {
        if (!rightItems.Contains(itemToAdd))
        {
            rightItems.Add(itemToAdd);
        }
    }

    public void SetCurrentHandItem(bool left, GameObject weapon)
    {
        if (left)
        {
            currentLeftItem = weapon;
        }
        else
        {
            currentRightItem = weapon;
        }
    }
}
