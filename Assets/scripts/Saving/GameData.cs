using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    //public List<string> inventory;
    public int partyGold, currentFloor;
    public int maxLevel, maxExp, maxMaxHealth, maxCurrentHealth, maxStr;
               //luciaLevel, luciaExp, luciaMaxHealth, luciaCurrentHealth, luciaStr;
    
    public GameData()
    {
        //inventory = new List<string>();
        this.partyGold = 100; this.currentFloor = 2; // First level is 2, Yeaston is 1
        this.maxLevel = 1; this.maxExp = 0; this.maxMaxHealth = 3;
        this.maxCurrentHealth = 3; this.maxStr = 1;

        /*
        this.luciaLevel = 1; this.luciaExp = 0; this.luciaMaxHealth = 20;
        this.luciaCurrentHealth = 20; this.luciaStr = 1;
        */
    }
}
