using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class UIMainMenu : MonoBehaviour
{
    private List<CharacterData> characters = new List<CharacterData>();
    private ListView characterList;
    private Button createBtn;
    private Button editBtn;
    private Button deleteBtn;

    private VisualElement deleteDialog;
    private Label deleteDialogLabel;
    private Button deleteConfirmBtn;
    private Button deleteCancelBtn;
    private CharacterData pendingDelete = null;

    public GameObject characterForm;
    private UICharacterForm formScript;

    private static readonly Color CardTextColor = new Color(0.24f, 0.17f, 0.12f, 1f);
    private static readonly Color SelectedBorderColor = new Color(0.98f, 0.85f, 0.4f, 1f);

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

        if (deleteDialog != null)
            deleteDialog.style.display = DisplayStyle.None;

        if (createBtn != null) createBtn.clicked += OnCreateClicked;
        if (editBtn   != null) editBtn.clicked   += OnEditClicked;
        if (deleteBtn != null) deleteBtn.clicked += OnDeleteClicked;

        if (deleteConfirmBtn != null) deleteConfirmBtn.clicked += OnDeleteConfirmed;
        if (deleteCancelBtn  != null) deleteCancelBtn.clicked  += OnDeleteCancelled;

        if (characterForm != null)
            formScript = characterForm.GetComponent<UICharacterForm>();

        var headerBar = root.Q<VisualElement>("headerBar");
        if (headerBar != null)
        {
            var barTex = Resources.Load<Texture2D>("UI/bg_header");
            if (barTex != null)
            {
                headerBar.style.backgroundImage = new StyleBackground(barTex);
                headerBar.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
            }
        }

        var titleLogo = root.Q<VisualElement>("titleLogo");
        if (titleLogo != null)
        {
            var logoTex = Resources.Load<Texture2D>("UI/title_logo");
            if (logoTex != null)
            {
                titleLogo.style.backgroundImage = new StyleBackground(logoTex);
                titleLogo.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            }
        }

        RefreshList();
    }

    public void RefreshList()
    {
        characters = DataManager.LoadAllCharacters();

        if (characterList == null) return;

        characterList.itemsSource = characters;

        characterList.makeItem = () =>
        {
            var container = new VisualElement();
            container.style.paddingTop = 4;
            container.style.paddingBottom = 4;
            container.style.paddingLeft = 4;
            container.style.paddingRight = 4;
            container.style.backgroundColor = new Color(0, 0, 0, 0);

            var card = new VisualElement();
            card.name = "card";

            var cardBg = Resources.Load<Texture2D>("UI/card_character");
            if (cardBg != null)
            {
                card.style.backgroundImage = new StyleBackground(cardBg);
                card.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
            }
            else
            {
                card.style.backgroundColor = new Color(0.72f, 0.63f, 0.39f, 1f);
            }

            card.style.borderTopLeftRadius = 8;
            card.style.borderTopRightRadius = 8;
            card.style.borderBottomLeftRadius = 8;
            card.style.borderBottomRightRadius = 8;
            card.style.paddingTop = 14;
            card.style.paddingBottom = 14;
            card.style.paddingLeft = 16;
            card.style.paddingRight = 16;
            card.style.flexGrow = 1;
            card.style.justifyContent = Justify.Center;

            card.style.borderTopWidth = 3;
            card.style.borderBottomWidth = 3;
            card.style.borderLeftWidth = 3;
            card.style.borderRightWidth = 3;
            card.style.borderTopColor = Color.clear;
            card.style.borderBottomColor = Color.clear;
            card.style.borderLeftColor = Color.clear;
            card.style.borderRightColor = Color.clear;

            var nameLabel = new Label();
            nameLabel.name = "cardName";
            nameLabel.style.color = CardTextColor;
            nameLabel.style.fontSize = 20;
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            card.Add(nameLabel);

            var detailsRow = new VisualElement();
            detailsRow.style.flexDirection = FlexDirection.Row;
            detailsRow.style.marginTop = 4;

            var raceLabel = new Label();
            raceLabel.name = "cardRace";
            raceLabel.style.color = CardTextColor;
            raceLabel.style.fontSize = 14;
            raceLabel.style.marginRight = 10;
            detailsRow.Add(raceLabel);

            var classLabel = new Label();
            classLabel.name = "cardClass";
            classLabel.style.color = CardTextColor;
            classLabel.style.fontSize = 14;
            detailsRow.Add(classLabel);

            card.Add(detailsRow);
            container.Add(card);
            return container;
        };

        characterList.bindItem = (element, index) =>
        {
            var character = characters[index];
            var card       = element.Q<VisualElement>("card");
            var nameLabel  = element.Q<Label>("cardName");
            var raceLabel  = element.Q<Label>("cardRace");
            var classLabel = element.Q<Label>("cardClass");

            if (nameLabel  != null) nameLabel.text  = character.name + "  (ур. " + character.level + ")";
            if (raceLabel  != null) raceLabel.text  = character.race;
            if (classLabel != null) classLabel.text = character.characterClass;

            bool isSelected = index == characterList.selectedIndex;
            if (card != null)
            {
                Color borderColor = isSelected ? SelectedBorderColor : Color.clear;
                card.style.borderTopColor = borderColor;
                card.style.borderBottomColor = borderColor;
                card.style.borderLeftColor = borderColor;
                card.style.borderRightColor = borderColor;
            }
        };

        characterList.fixedItemHeight = 92;

        characterList.selectionChanged -= OnSelectionChanged;
        characterList.selectionChanged += OnSelectionChanged;

        characterList.itemsChosen -= OnItemsChosen;
        characterList.itemsChosen += OnItemsChosen;

        characterList.RefreshItems();
    }

    private void OnSelectionChanged(IEnumerable<object> _)
    {
        characterList.RefreshItems();
    }

    private void OnItemsChosen(IEnumerable<object> chosen)
    {
        var character = chosen.FirstOrDefault() as CharacterData;
        if (character != null && formScript != null)
            formScript.SetViewMode(character);
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
