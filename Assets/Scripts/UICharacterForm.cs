using UnityEngine;
using UnityEngine.UIElements;

public class UICharacterForm : MonoBehaviour
{
    private VisualElement root;
    private bool isEditMode = false;
    private string editingCharacterId = null;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetCreateMode()
    {
        Debug.Log("SetCreateMode вызван!");
        
        gameObject.SetActive(true);
        
        // Ждём один кадр чтобы UIDocument инициализировался
        StartCoroutine(SetCreateModeDelayed());
    }

    private System.Collections.IEnumerator SetCreateModeDelayed()
    {
        yield return null;

        root = GetComponent<UIDocument>().rootVisualElement;

        var formTitle = root.Q<Label>("formTitle");
        var nameField = root.Q<TextField>("nameField");
        var levelField = root.Q<IntegerField>("levelField");
        var raceField = root.Q<DropdownField>("raceField");
        var classField = root.Q<DropdownField>("classField");
        var strField = root.Q<IntegerField>("strField");
        var dexField = root.Q<IntegerField>("dexField");
        var conField = root.Q<IntegerField>("conField");
        var intField = root.Q<IntegerField>("intField");
        var wisField = root.Q<IntegerField>("wisField");
        var chaField = root.Q<IntegerField>("chaField");
        var errorLabel = root.Q<Label>("errorLabel");

        // Заполняем выпадающие списки
        raceField.choices = new System.Collections.Generic.List<string>
    {
        "Человек", "Эльф", "Дварф", "Полурослик", "Гном",
        "Полуэльф", "Полуорк", "Тифлинг", "Драконорождённый"
    };
        classField.choices = new System.Collections.Generic.List<string>
    {
        "Воин", "Волшебник", "Плут", "Жрец", "Друид",
        "Бард", "Паладин", "Следопыт", "Монах",
        "Варвар", "Чародей", "Колдун"
    };

        isEditMode = false;
        editingCharacterId = null;

        if (formTitle != null) formTitle.text = "Новый персонаж";
        if (nameField != null) nameField.value = "";
        if (levelField != null) levelField.value = 1;
        if (raceField != null) raceField.index = -1;
        if (classField != null) classField.index = -1;
        if (strField != null) strField.value = 10;
        if (dexField != null) dexField.value = 10;
        if (conField != null) conField.value = 10;
        if (intField != null) intField.value = 10;
        if (wisField != null) wisField.value = 10;
        if (chaField != null) chaField.value = 10;
        if (errorLabel != null) errorLabel.text = "";

        var saveBtn = root.Q<Button>("saveBtn");
        var cancelBtn = root.Q<Button>("cancelBtn");
        if (saveBtn != null) { saveBtn.clicked -= OnSaveClicked; saveBtn.clicked += OnSaveClicked; }
        if (cancelBtn != null) { cancelBtn.clicked -= OnCancelClicked; cancelBtn.clicked += OnCancelClicked; }
    }

    public void SetEditMode(CharacterData character)
    {
        gameObject.SetActive(true);
        StartCoroutine(SetEditModeDelayed(character));
    }

    private System.Collections.IEnumerator SetEditModeDelayed(CharacterData character)
    {
        yield return null;

        root = GetComponent<UIDocument>().rootVisualElement;

        var formTitle = root.Q<Label>("formTitle");
        var nameField = root.Q<TextField>("nameField");
        var levelField = root.Q<IntegerField>("levelField");
        var raceField = root.Q<DropdownField>("raceField");
        var classField = root.Q<DropdownField>("classField");
        var strField = root.Q<IntegerField>("strField");
        var dexField = root.Q<IntegerField>("dexField");
        var conField = root.Q<IntegerField>("conField");
        var intField = root.Q<IntegerField>("intField");
        var wisField = root.Q<IntegerField>("wisField");
        var chaField = root.Q<IntegerField>("chaField");
        var errorLabel = root.Q<Label>("errorLabel");

        var races = new System.Collections.Generic.List<string>
    {
        "Человек", "Эльф", "Дварф", "Полурослик", "Гном",
        "Полуэльф", "Полуорк", "Тифлинг", "Драконорождённый"
    };
        var classes = new System.Collections.Generic.List<string>
    {
        "Воин", "Волшебник", "Плут", "Жрец", "Друид",
        "Бард", "Паладин", "Следопыт", "Монах",
        "Варвар", "Чародей", "Колдун"
    };

        if (raceField != null) raceField.choices = races;
        if (classField != null) classField.choices = classes;

        isEditMode = true;
        editingCharacterId = character.id;

        if (formTitle != null) formTitle.text = "Редактирование персонажа";
        if (nameField != null) nameField.value = character.name;
        if (levelField != null) levelField.value = character.level;
        if (raceField != null) raceField.value = character.race;
        if (classField != null) classField.value = character.characterClass;
        if (strField != null) strField.value = character.strength;
        if (dexField != null) dexField.value = character.dexterity;
        if (conField != null) conField.value = character.constitution;
        if (intField != null) intField.value = character.intelligence;
        if (wisField != null) wisField.value = character.wisdom;
        if (chaField != null) chaField.value = character.charisma;
        if (errorLabel != null) errorLabel.text = "";

        var saveBtn = root.Q<Button>("saveBtn");
        var cancelBtn = root.Q<Button>("cancelBtn");
        if (saveBtn != null) { saveBtn.clicked -= OnSaveClicked; saveBtn.clicked += OnSaveClicked; }
        if (cancelBtn != null) { cancelBtn.clicked -= OnCancelClicked; cancelBtn.clicked += OnCancelClicked; }
    }

    private void OnSaveClicked()
    {
        Debug.Log("OnSaveClicked вызван!");
        root = GetComponent<UIDocument>().rootVisualElement;
        
        var nameField = root.Q<TextField>("nameField");
        var levelField = root.Q<IntegerField>("levelField");
        var raceField = root.Q<DropdownField>("raceField");
        var classField = root.Q<DropdownField>("classField");
        var strField = root.Q<IntegerField>("strField");
        var dexField = root.Q<IntegerField>("dexField");
        var conField = root.Q<IntegerField>("conField");
        var intField = root.Q<IntegerField>("intField");
        var wisField = root.Q<IntegerField>("wisField");
        var chaField = root.Q<IntegerField>("chaField");
        var errorLabel = root.Q<Label>("errorLabel");

        string errors = "";

        if (nameField == null || string.IsNullOrEmpty(nameField.value))
            errors += "• Имя обязательно для заполнения\n";
        if (raceField == null || raceField.index < 0)
            errors += "• Раса обязательна для заполнения\n";
        if (classField == null || classField.index < 0)
            errors += "• Класс обязателен для заполнения\n";

        int level = levelField != null ? levelField.value : 0;
        if (level < 1 || level > 20)
            errors += "• Уровень должен быть от 1 до 20\n";

        if (!string.IsNullOrEmpty(errors))
        {
            if (errorLabel != null) errorLabel.text = errors.TrimEnd();
            return;
        }

        if (errorLabel != null) errorLabel.text = "";

        CharacterData character;
        if (isEditMode)
        {
            character = DataManager.LoadCharacter(editingCharacterId);
            if (character == null) character = new CharacterData();
        }
        else
        {
            character = new CharacterData();
        }

        character.name = nameField.value;
        character.level = level;
        character.race = raceField != null ? raceField.value : "";
        character.characterClass = classField != null ? classField.value : "";
        character.strength = strField != null ? strField.value : 10;
        character.dexterity = dexField != null ? dexField.value : 10;
        character.constitution = conField != null ? conField.value : 10;
        character.intelligence = intField != null ? intField.value : 10;
        character.wisdom = wisField != null ? wisField.value : 10;
        character.charisma = chaField != null ? chaField.value : 10;

        DataManager.SaveCharacter(character);
        Debug.Log("Сохранён: " + character.name);

        gameObject.SetActive(false);

        var mainMenu = FindAnyObjectByType<UIMainMenu>();
        if (mainMenu != null) mainMenu.RefreshList();
    }

    private void OnCancelClicked()
    {
        gameObject.SetActive(false);
    }
}