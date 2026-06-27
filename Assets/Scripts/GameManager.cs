using UnityEngine;

public class GameManager : MonoBehaviour
{
    private CharacterData currentCharacter;
    
    void Start()
    {
        // При запуске создаем тестового персонажа
        CreateTestCharacter();
       
        DataManager.SaveCharacter(currentCharacter);
        
        LoadAllAndPrint();
    }
    
    void CreateTestCharacter()
    {
        // Создаем нового персонажа
        currentCharacter = new CharacterData();
        currentCharacter.name = "Гимли";
        currentCharacter.level = 3;
        currentCharacter.race = "Дварф";
        currentCharacter.characterClass = "Воин";
        currentCharacter.strength = 17;
        currentCharacter.dexterity = 12;
        currentCharacter.constitution = 16;
        currentCharacter.intelligence = 10;
        currentCharacter.wisdom = 12;
        currentCharacter.charisma = 8;
        
        InventoryItem axe = new InventoryItem();
        axe.itemName = "Боевой топор";
        axe.quantity = 1;
        axe.weight = 4.5f;
        currentCharacter.inventory.Add(axe);
        
        Spell spell = new Spell();
        spell.name = "Лечение ран";
        spell.level = 1;
        spell.school = "Восстановление";
        currentCharacter.spells.Add(spell);
        
        Debug.Log("Создан персонаж: " + currentCharacter.name);
    }
    
    void LoadAllAndPrint()
    {
        // Загружаем всех персонажей из папки Data/
        var characters = DataManager.LoadAllCharacters();
        
        Debug.Log("Всего персонажей в базе: " + characters.Count);
        
        // Выводим имена каждого
        foreach (var c in characters)
        {
            Debug.Log(" - " + c.name + " (уровень " + c.level + ")");
        }
    }
}