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

    // Диалог удаления
    private VisualElement deleteDialog;
    private Label deleteDialogLabel;
    private Button deleteConfirmBtn;
    private Button deleteCancelBtn;
    private CharacterData pendingDelete = null;

    public GameObject characterForm;
    private UICharacterForm formScript;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        if (root == null) { Debug.LogError("UIDocument не найден!"); return; }

        characterList = root.Q<ListView>("characterList");
        createBtn     = root.Q<Button>("createBtn");
        editBtn       = root.Q<Button>("editBtn");
        deleteBtn     = root.Q<Button>("deleteBtn");

        deleteDialog      = root.Q<VisualElement>("deleteDialog");
        deleteDialogLabel = root.Q<Label>("deleteDialogLabel");
        deleteConfirmBtn  = root.Q<Button>("deleteConfirmBtn");
        deleteCancelBtn   = root.Q<Button>("deleteCancelBtn");

        if (characterList    == null) Debug.LogError("characterList не найден!");
        if (createBtn        == null) Debug.LogError("createBtn не найден!");
        if (editBtn          == null) Debug.LogError("editBtn не найден!");
        if (deleteBtn        == null) Debug.LogError("deleteBtn не найден!");
        if (deleteDialog     == null) Debug.LogError("deleteDialog не найден!");
        if (deleteConfirmBtn == null) Debug.LogError("deleteConfirmBtn не найден!");
        if (deleteCancelBtn  == null) Debug.LogError("deleteCancelBtn не найден!");

        // Прячем диалог при старте
        if (deleteDialog != null) deleteDialog.style.display = DisplayStyle.None;

        createBtn.clicked += OnCreateClicked;
        editBtn.clicked   += OnEditClicked;
        deleteBtn.clicked += OnDeleteClicked;

        if (deleteConfirmBtn != null) deleteConfirmBtn.clicked += OnDeleteConfirmed;
        if (deleteCancelBtn  != null) deleteCancelBtn.clicked  += OnDeleteCancelled;

        if (characterForm != null)
            formScript = characterForm.GetComponent<UICharacterForm>();

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
            formScript.SetCreateMode();
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
            formScript.SetEditMode(selected);
    }

    private void OnDeleteClicked()
    {
        int selectedIndex = characterList.selectedIndex;
        if (selectedIndex < 0 || selectedIndex >= characters.Count)
        {
            Debug.LogWarning("Выберите персонажа для удаления");
            return;
        }

        pendingDelete = characters[selectedIndex];

        // Показываем диалог подтверждения
        if (deleteDialogLabel != null)
            deleteDialogLabel.text = "Удалить персонажа \"" + pendingDelete.name + "\"?";

        if (deleteDialog != null)
            deleteDialog.style.display = DisplayStyle.Flex;
    }

    private void OnDeleteConfirmed()
    {
        if (pendingDelete != null)
        {
            DataManager.DeleteCharacter(pendingDelete.id);
            pendingDelete = null;
            Debug.Log("Персонаж удалён");
        }

        if (deleteDialog != null)
            deleteDialog.style.display = DisplayStyle.None;

        RefreshList();
    }

    private void OnDeleteCancelled()
    {
        pendingDelete = null;
        if (deleteDialog != null)
            deleteDialog.style.display = DisplayStyle.None;
    }
}
