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
    public string subclass;
    public string race;
    public string background;
    public string alignment;

    public int strength;
    public int dexterity;
    public int constitution;
    public int intelligence;
    public int wisdom;
    public int charisma;

    public int armorClass;
    public int proficiencyBonus;
    public int initiative;
    public int speed;

    public SkillProficiencies skills = new SkillProficiencies();

    public string weaponsText;
    public string equipmentText;
    public string spellsText;

    public List<WeaponItem> weapons = new List<WeaponItem>();

    public List<InventoryItem> inventory = new List<InventoryItem>();

    public List<Spell> spells = new List<Spell>();

    public CharacterData()
    {
        id = Guid.NewGuid().ToString();
        level = 1;
        experience = 0;
        strength = 10;
        dexterity = 10;
        constitution = 10;
        intelligence = 10;
        wisdom = 10;
        charisma = 10;
        armorClass = 10;
        proficiencyBonus = 2;
        speed = 30;
    }
}

[System.Serializable]
public class SkillProficiencies
{
    public bool strSave;
    public bool dexSave;
    public bool conSave;
    public bool intSave;
    public bool wisSave;
    public bool chaSave;

    public bool athletics;

    public bool acrobatics;
    public bool sleightOfHand;
    public bool stealth;

    public bool perception;
    public bool survival;
    public bool medicine;
    public bool insight;
    public bool animalHandling;

    public bool analysis;
    public bool history;
    public bool magic;
    public bool nature;
    public bool religion;

    public bool performance;
    public bool intimidation;
    public bool deception;
    public bool persuasion;
}

[System.Serializable]
public class WeaponItem
{
    public string name;
    public string damage;
    public float weight;
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
    public string description;
}
