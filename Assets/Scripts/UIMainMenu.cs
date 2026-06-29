using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class UIMainMenu : MonoBehaviour
{
    private List<CharacterData> characters = new List<CharacterData>();
    private ListView characterList;
    private Button createBtn;
    private Button editBtn;
    private Button deleteBtn;

    // Ссылка на форму
    public GameObject characterForm;
    private UICharacterForm formScript;

    private void OnEnable()
    {
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

        // Получаем скрипт формы
        if (characterForm != null)
        {
            formScript = characterForm.GetComponent<UICharacterForm>();
        }

        RefreshList();
    }

    public void RefreshList()
    {
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
        Debug.Log("OnCreateClicked вызван");
        Debug.Log("formScript = " + (formScript == null ? "NULL" : "OK"));
        Debug.Log("characterForm = " + (characterForm == null ? "NULL" : "OK"));

        if (formScript != null)
        {
            formScript.SetCreateMode();
        }
    }

    private void OnEditClicked()
    {
        int selectedIndex = characterList.selectedIndex;
        if (selectedIndex < 0 || selectedIndex >= characters.Count)
        {
            Debug.LogWarning("Выберите персонажа для редактирования");
            return;
        }
        CharacterData selected = characters[selectedIndex];
        if (formScript != null)
        {
            formScript.SetEditMode(selected); // форма сама делает SetActive(true)
        }
    }

    private void OnDeleteClicked()
    {
        int selectedIndex = characterList.selectedIndex;
        if (selectedIndex < 0 || selectedIndex >= characters.Count)
        {
            Debug.LogWarning("Выберите персонажа для удаления");
            return;
        }

        CharacterData selected = characters[selectedIndex];
        Debug.Log("Удаление персонажа: " + selected.name);

        DataManager.DeleteCharacter(selected.id);
        RefreshList();
    }
}