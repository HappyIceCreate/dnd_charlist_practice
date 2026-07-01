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

        characterList    = root.Q<ListView>("characterList");
        createBtn        = root.Q<Button>("createBtn");
        editBtn          = root.Q<Button>("editBtn");
        deleteBtn        = root.Q<Button>("deleteBtn");

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

        if (deleteDialog != null)
            deleteDialog.style.display = DisplayStyle.None;

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

        if (characterList == null) return;

        characterList.itemsSource = characters;

        characterList.makeItem = () =>
        {
            // Карточка персонажа в списке
            var card = new VisualElement();
            card.style.backgroundColor = new Color(0.72f, 0.63f, 0.39f, 1f); // #C9B375
            card.style.borderTopLeftRadius = 8;
            card.style.borderTopRightRadius = 8;
            card.style.borderBottomLeftRadius = 8;
            card.style.borderBottomRightRadius = 8;
            card.style.paddingTop = 8;
            card.style.paddingBottom = 8;
            card.style.paddingLeft = 12;
            card.style.paddingRight = 12;
            card.style.marginBottom = 0;
            card.style.marginTop = 0;
            card.style.borderBottomWidth = 2;
            card.style.borderBottomColor = new Color(0.24f, 0.17f, 0.12f, 0.3f);
            card.style.marginLeft = 4;
            card.style.marginRight = 4;

            var nameLabel = new Label();
            nameLabel.name = "cardName";
            nameLabel.style.color = new Color(0.24f, 0.17f, 0.12f, 1f); // #3D2B1F
            nameLabel.style.fontSize = 16;
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            card.Add(nameLabel);

            var detailsRow = new VisualElement();
            detailsRow.style.flexDirection = FlexDirection.Row;
            detailsRow.style.marginTop = 2;

            var raceLabel = new Label();
            raceLabel.name = "cardRace";
            raceLabel.style.color = new Color(0.24f, 0.17f, 0.12f, 1f);
            raceLabel.style.fontSize = 12;
            raceLabel.style.marginRight = 8;
            detailsRow.Add(raceLabel);

            var classLabel = new Label();
            classLabel.name = "cardClass";
            classLabel.style.color = new Color(0.24f, 0.17f, 0.12f, 1f);
            classLabel.style.fontSize = 12;
            detailsRow.Add(classLabel);

            card.Add(detailsRow);
            return card;
        };

        characterList.bindItem = (element, index) =>
        {
            var character = characters[index];
            var nameLabel  = element.Q<Label>("cardName");
            var raceLabel  = element.Q<Label>("cardRace");
            var classLabel = element.Q<Label>("cardClass");

            if (nameLabel  != null) nameLabel.text  = character.name + "  (ур. " + character.level + ")";
            if (raceLabel  != null) raceLabel.text  = character.race;
            if (classLabel != null) classLabel.text = character.characterClass;
        };

        characterList.fixedItemHeight = 72;
        characterList.style.paddingTop = 4;
        characterList.style.paddingBottom = 4;
        characterList.RefreshItems();
    }

    private void OnCreateClicked()
    {
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
