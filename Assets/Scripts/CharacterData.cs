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

    // Характеристики (6 штук)
    public int strength;
    public int dexterity;
    public int constitution;
    public int intelligence;
    public int wisdom;
    public int charisma;

    // Боевые параметры
    public int armorClass;
    public int hpCurrent;
    public int hpTemp;
    public int hpMax;
    public int proficiencyBonus;
    public int initiative;
    public int speed;
    public int passivePerception;
    public string trait;
    public string spellcastingMod;

    // Владение навыками (true = есть владение)
    public SkillProficiencies skills = new SkillProficiencies();

    // Текстовые блоки
    public string weaponsText;
    public string equipmentText;
    public string spellsText;

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
        strength = 10;
        dexterity = 10;
        constitution = 10;
        intelligence = 10;
        wisdom = 10;
        charisma = 10;
        armorClass = 10;
        hpCurrent = 0;
        hpTemp = 0;
        hpMax = 0;
        proficiencyBonus = 2;
        speed = 30;
    }
}

[System.Serializable]
public class SkillProficiencies
{
    // Спасброски
    public bool strSave;
    public bool dexSave;
    public bool conSave;
    public bool intSave;
    public bool wisSave;
    public bool chaSave;

    // Навыки силы
    public bool athletics;

    // Навыки ловкости
    public bool acrobatics;
    public bool sleightOfHand;
    public bool stealth;

    // Навыки мудрости
    public bool perception;
    public bool survival;
    public bool medicine;
    public bool insight;
    public bool animalHandling;

    // Навыки интеллекта
    public bool analysis;
    public bool history;
    public bool magic;
    public bool nature;
    public bool religion;

    // Навыки харизмы
    public bool performance;
    public bool intimidation;
    public bool deception;
    public bool persuasion;
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
