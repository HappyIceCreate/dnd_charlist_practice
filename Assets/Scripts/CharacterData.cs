using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public string id;
    public string name;
    public int level;
    public int experience;
    public string characterClass;
    public string race;
    public string background;
    public string alignment;

    // Характеристики (6 штук)
    public int strength;
    public int dexterity;
    public int constitution;
    public int intelligence;
    public int wisdom;
    public int charisma;

    // Инвентарь (список предметов)
    public List<InventoryItem> inventory = new List<InventoryItem>();

    // Заклинания (список)
    public List<Spell> spells = new List<Spell>();

    // Конструктор (создает нового персонажа с ID)
    public CharacterData()
    {
        id = Guid.NewGuid().ToString();
        level = 1;
        experience = 0;
    }
}

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public int quantity;
    public float weight;
}

[System.Serializable]
public class Spell
{
    public string name;
    public int level;
    public string school;
}