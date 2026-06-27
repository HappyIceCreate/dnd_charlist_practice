using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class UIMainMenu : MonoBehaviour
{
    private List<CharacterData> characters = new List<CharacterData>();
    private ListView characterList;

    // Ссылки на кнопки
    private Button createBtn;
    private Button editBtn;
    private Button deleteBtn;

    private void OnEnable()
    {
        // Находим корневой элемент UI
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        if (root == null)
        {
            Debug.LogError("UIDocument не найден!");
            return;
        }

        characterList = root.Q<ListView>("characterList");
        createBtn = root.Q<Button>("createBtn");
        editBtn = root.Q<Button>("editBtn");
        deleteBtn = root.Q<Button>("deleteBtn");

        if (characterList == null) Debug.LogError("characterList не найден!");
        if (createBtn == null) Debug.LogError("createBtn не найден!");
        if (editBtn == null) Debug.LogError("editBtn не найден!");
        if (deleteBtn == null) Debug.LogError("deleteBtn не найден!");

        createBtn.clicked += OnCreateClicked;
        editBtn.clicked += OnEditClicked;
        deleteBtn.clicked += OnDeleteClicked;
        RefreshList();
    }

    private void RefreshList()
    {
        // Загружаем всех персонажей из папки Data/
        characters = DataManager.LoadAllCharacters();
        characterList.itemsSource = characters;
        characterList.makeItem = () => new Label();
        characterList.bindItem = (element, index) =>
        {
            var label = element as Label;
            var character = characters[index];
            label.text = character.name + " (уровень " + character.level + ")";
        };
        characterList.RefreshItems();
    }

    private void OnCreateClicked()
    {
        Debug.Log("Нажата кнопка СОЗДАТЬ");
    }

    private void OnEditClicked()
    {
        Debug.Log("Нажата кнопка РЕДАКТИРОВАТЬ");
    }

    private void OnDeleteClicked()
    {
        Debug.Log("Нажата кнопка УДАЛИТЬ");
    }
}