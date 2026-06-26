using UnityEngine;

public class TestDataManager : MonoBehaviour
{
    void Start()
    {
        // Создаем тестового персонажа
        CharacterData testChar = new CharacterData();
        testChar.name = "Тестовый персонаж";
        testChar.level = 5;
        testChar.race = "Человек";
        testChar.characterClass = "Воин";
        testChar.strength = 16;
        testChar.dexterity = 14;
        testChar.constitution = 15;
        testChar.intelligence = 10;
        testChar.wisdom = 12;
        testChar.charisma = 8;

        // Добавляем предмет в инвентарь
        testChar.inventory.Add(new InventoryItem
        {
            itemName = "Меч",
            quantity = 1,
            weight = 3.5f
        });

        // Добавляем заклинание
        testChar.spells.Add(new Spell
        {
            name = "Огненный шар",
            level = 3,
            school = "Эвокация"
        });

        // Сохраняем
        DataManager.SaveCharacter(testChar);
        Debug.Log("Персонаж сохранен!");

        // Загружаем всех
        var characters = DataManager.LoadAllCharacters();
        Debug.Log("Загружено персонажей: " + characters.Count);

        foreach (var c in characters)
        {
            Debug.Log(" - " + c.name + " (Уровень " + c.level + ")");
        }
    }
}