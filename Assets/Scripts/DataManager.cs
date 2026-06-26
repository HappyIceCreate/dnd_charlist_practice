using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public static class DataManager
{
    private static string dataPath = Application.dataPath + "/Data/";

    // Сохранить персонажа
    public static void SaveCharacter(CharacterData character)
    {
        if (!Directory.Exists(dataPath))
            Directory.CreateDirectory(dataPath);

        string json = JsonConvert.SerializeObject(character, Formatting.Indented);
        string filePath = dataPath + character.id + ".json";
        File.WriteAllText(filePath, json);

        Debug.Log("Сохранен персонаж: " + character.name);
    }

    // Загрузить всех персонажей
    public static List<CharacterData> LoadAllCharacters()
    {
        List<CharacterData> characters = new List<CharacterData>();

        if (!Directory.Exists(dataPath))
            return characters;

        string[] files = Directory.GetFiles(dataPath, "*.json");

        foreach (string file in files)
        {
            try
            {
                string json = File.ReadAllText(file);
                CharacterData character = JsonConvert.DeserializeObject<CharacterData>(json);
                if (character != null)
                    characters.Add(character);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Ошибка загрузки файла " + file + ": " + e.Message);
            }
        }

        return characters;
    }

    // Загрузить одного персонажа по ID
    public static CharacterData LoadCharacter(string id)
    {
        string filePath = dataPath + id + ".json";
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Файл не найден: " + filePath);
            return null;
        }

        string json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<CharacterData>(json);
    }

    // Удалить персонажа
    public static void DeleteCharacter(string id)
    {
        string filePath = dataPath + id + ".json";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Удален персонаж с ID: " + id);
        }
    }

    // Проверка существует ли персонаж
    public static bool CharacterExists(string id)
    {
        string filePath = dataPath + id + ".json";
        return File.Exists(filePath);
    }
}