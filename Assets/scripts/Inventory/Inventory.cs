using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
public class Inventory : MonoBehaviour
{
    //public List<ItemBase> slots;
    //public List<ItemBase> items = new List<ItemBase>();
    //public GameObject max, lucia;

    public List<string> inventory = new List<string>();
    public int gold;
    public string milk = "Milk";
    public string water = "Water";
    public string yeast = "Yeast";

    public string speedUpgrade = "Blue Charm";
    public string projectileUpgrade = "Fireball";
    public string InvincibilityUpgrade = "Pocket Dimension";

    public TextMeshProUGUI goldAmount;

    private void Start()
    {
        goldAmount = GameObject.Find("Gold Display").GetComponent<TextMeshProUGUI>();
        setGoldAmount();
        Debug.Log("Inventory: " + inventory);
    }
    public void AddItem(string itemName)
    {
        inventory.Add(itemName);
    }

    public void RemoveItem(string itemName)
    {
        inventory.Remove(itemName);
    }

    /*
    public void AddItem(ItemBase item)
     {
	     items.Add(item);
     }

     public void RemoveItem(ItemBase item)
     {
	     items.Remove(item);
     }
    */
    public void addMoney(int price)
     {
	     gold += price;
        setGoldAmount();
     }

	public void takeMoney(int price)
    {
	    gold -= price;
        setGoldAmount();
    }

    public void useItem(string itemName, GameObject itemUser)
    {  
        if (itemName == milk)
        {
            itemUser.GetComponent<PlayerMovement>().healByAmount(10);
        }

        if (itemName == water)
        {
            itemUser.GetComponent<PlayerMovement>().healByAmount(20);
        }

        if (itemName == yeast)
        {
            itemUser.GetComponent<PlayerMovement>().healByAmount(30);
        }
    }

    public void setGoldAmount()
    {
        goldAmount.text = gold.ToString();
    }
}