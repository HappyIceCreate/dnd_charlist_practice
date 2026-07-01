using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class UICharacterForm : MonoBehaviour
{
    private VisualElement root;
    private bool isEditMode = false;
    private string editingCharacterId = null;

    private static readonly List<string> Races = new List<string>
    {
        "Человек", "Эльф", "Дварф", "Полурослик", "Гном",
        "Полуэльф", "Полуорк", "Тифлинг", "Драконорождённый"
    };

    private static readonly List<string> Classes = new List<string>
    {
        "Воин", "Волшебник", "Плут", "Жрец", "Друид",
        "Бард", "Паладин", "Следопыт", "Монах",
        "Варвар", "Чародей", "Колдун"
    };

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetCreateMode()
    {
        gameObject.SetActive(true);
        StartCoroutine(FillFormDelayed(null));
    }

    public void SetEditMode(CharacterData character)
    {
        gameObject.SetActive(true);
        StartCoroutine(FillFormDelayed(character));
    }

    private IEnumerator FillFormDelayed(CharacterData character)
    {
        yield return null; // ждём кадр чтобы UIDocument проинициализировался

        root = GetComponent<UIDocument>().rootVisualElement;

        // Заполняем выпадающие списки
        var raceField = root.Q<DropdownField>("raceField");
        var classField = root.Q<DropdownField>("classField");
        if (raceField != null) raceField.choices = Races;
        if (classField != null) classField.choices = Classes;

        if (character == null)
        {
            isEditMode = false;
            editingCharacterId = null;
            FillEmptyForm();
        }
        else
        {
            isEditMode = true;
            editingCharacterId = character.id;
            FillFormFromCharacter(character);
        }

        ClearError();
        RecalculateAll();
        ApplyBackgrounds();
        SubscribeEvents();
    }

    private void ApplyBackgrounds()
    {
        SetBlockBg("headerBlock", "form_bg_header", false);
        SetBlockBg("levelBlock", "form_bg_level", false);
        SetBlockBg("acBlock", "form_bg_ac", false);
        SetBlockBg("hpBlock", "form_bg_hp", false);

        // Вторая полоса
        SetBlockBg("profBonusField", "form_bg_second_row", true);

        // Блоки характеристик
        SetBlockBg("strBlock", "form_bg_stat_short", false);
        SetBlockBg("conBlock", "form_bg_stat_short", false);
        SetBlockBg("dexBlock", "form_bg_stat_tall", false);
        SetBlockBg("wisBlock", "form_bg_stat_tall", false);
        SetBlockBg("intBlock", "form_bg_stat_tall", false);
        SetBlockBg("chaBlock", "form_bg_stat_tall", false);

        // Правая колонка
        SetBlockBg("weaponsField", "form_bg_weapons", true);
        SetBlockBg("equipmentField", "form_bg_equipment", true);
        SetBlockBg("spellsField", "form_bg_spells", true);
    }

    private void SetBlockBg(string elementName, string textureName, bool searchParent)
    {
        var el = root.Q<VisualElement>(elementName);
        if (el == null) { Debug.LogWarning($"Элемент не найден: {elementName}"); return; }

        var target = searchParent && el.parent != null ? el.parent : el;

        var tex = Resources.Load<Texture2D>("UI/" + textureName);
        if (tex != null)
        {
            target.style.backgroundImage = new StyleBackground(tex);
            target.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
            target.style.backgroundColor = StyleKeyword.None;
        }
        else
        {
            Debug.LogWarning($"Текстура не найдена: UI/{textureName}");
        }
    }

    private void FillEmptyForm()
    {
        SetText("nameField", "");
        SetText("backgroundField", "");
        SetText("subclassField", "");
        SetDropdown("raceField", "");
        SetDropdown("classField", "");

        SetInt("levelField", 1);
        SetInt("experienceField", 0);
        SetInt("acField", 10);
        SetInt("hpCurrentField", 0);
        SetInt("hpTempField", 0);
        SetInt("hpMaxField", 0);

        SetInt("profBonusField", 2);
        SetInt("initiativeField", 0);
        SetInt("speedField", 30);
        SetInt("passivePerceptionField", 10);
        SetText("traitField", "");
        SetText("spellModField", "");

        SetInt("strField", 10);
        SetInt("dexField", 10);
        SetInt("conField", 10);
        SetInt("intField", 10);
        SetInt("wisField", 10);
        SetInt("chaField", 10);

        SetText("weaponsField", "");
        SetText("equipmentField", "");
        SetText("spellsField", "");

        // Сброс всех галочек владения
        foreach (var toggleName in AllProficiencyToggleNames())
            SetToggle(toggleName, false);
    }

    private void FillFormFromCharacter(CharacterData c)
    {
        SetText("nameField", c.name);
        SetText("backgroundField", c.background);
        SetText("subclassField", c.subclass);
        SetDropdown("raceField", c.race);
        SetDropdown("classField", c.characterClass);

        SetInt("levelField", c.level);
        SetInt("experienceField", c.experience);
        SetInt("acField", c.armorClass);
        SetInt("hpCurrentField", c.hpCurrent);
        SetInt("hpTempField", c.hpTemp);
        SetInt("hpMaxField", c.hpMax);

        SetInt("profBonusField", c.proficiencyBonus);
        SetInt("initiativeField", c.initiative);
        SetInt("speedField", c.speed);
        SetInt("passivePerceptionField", c.passivePerception);
        SetText("traitField", c.trait);
        SetText("spellModField", c.spellcastingMod);

        SetInt("strField", c.strength);
        SetInt("dexField", c.dexterity);
        SetInt("conField", c.constitution);
        SetInt("intField", c.intelligence);
        SetInt("wisField", c.wisdom);
        SetInt("chaField", c.charisma);

        SetText("weaponsField", c.weaponsText);
        SetText("equipmentField", c.equipmentText);
        SetText("spellsField", c.spellsText);

        var s = c.skills ?? new SkillProficiencies();
        SetToggle("strSaveProf", s.strSave);
        SetToggle("dexSaveProf", s.dexSave);
        SetToggle("conSaveProf", s.conSave);
        SetToggle("intSaveProf", s.intSave);
        SetToggle("wisSaveProf", s.wisSave);
        SetToggle("chaSaveProf", s.chaSave);

        SetToggle("athleticsProf", s.athletics);
        SetToggle("acrobaticsProf", s.acrobatics);
        SetToggle("sleightOfHandProf", s.sleightOfHand);
        SetToggle("stealthProf", s.stealth);
        SetToggle("perceptionProf", s.perception);
        SetToggle("survivalProf", s.survival);
        SetToggle("medicineProf", s.medicine);
        SetToggle("insightProf", s.insight);
        SetToggle("animalHandlingProf", s.animalHandling);
        SetToggle("analysisProf", s.analysis);
        SetToggle("historyProf", s.history);
        SetToggle("magicProf", s.magic);
        SetToggle("natureProf", s.nature);
        SetToggle("religionProf", s.religion);
        SetToggle("performanceProf", s.performance);
        SetToggle("intimidationProf", s.intimidation);
        SetToggle("deceptionProf", s.deception);
        SetToggle("persuasionProf", s.persuasion);
    }

    // ===== Подписка на события для авто-пересчёта =====

    private void SubscribeEvents()
    {
        // Пересчёт при изменении любой характеристики
        foreach (var statName in new[] { "strField", "dexField", "conField", "intField", "wisField", "chaField" })
        {
            var field = root.Q<IntegerField>(statName);
            if (field != null)
            {
                field.UnregisterValueChangedCallback(OnStatChanged);
                field.RegisterValueChangedCallback(OnStatChanged);
            }
        }

        // Пересчёт при изменении уровня (влияет на бонус владения)
        var levelField = root.Q<IntegerField>("levelField");
        if (levelField != null)
        {
            levelField.UnregisterValueChangedCallback(OnStatChanged);
            levelField.RegisterValueChangedCallback(OnStatChanged);
        }

        // Пересчёт при переключении любого чекбокса владения
        foreach (var toggleName in AllProficiencyToggleNames())
        {
            var toggle = root.Q<Toggle>(toggleName);
            if (toggle != null)
            {
                toggle.UnregisterValueChangedCallback(OnToggleChanged);
                toggle.RegisterValueChangedCallback(OnToggleChanged);
            }
        }

        // Кнопки
        var saveBtn = root.Q<Button>("saveBtn");
        var cancelBtn = root.Q<Button>("cancelBtn");
        if (saveBtn != null) { saveBtn.clicked -= OnSaveClicked; saveBtn.clicked += OnSaveClicked; }
        if (cancelBtn != null) { cancelBtn.clicked -= OnCancelClicked; cancelBtn.clicked += OnCancelClicked; }
    }

    private void OnStatChanged(ChangeEvent<int> evt) => RecalculateAll();
    private void OnToggleChanged(ChangeEvent<bool> evt) => RecalculateAll();

    // ===== Расчёт модификаторов и производных значений =====

    private int Modifier(int statValue) => Mathf.FloorToInt((statValue - 10) / 2f);

    private void RecalculateAll()
    {
        if (root == null) return;

        int str = GetInt("strField");
        int dex = GetInt("dexField");
        int con = GetInt("conField");
        int intel = GetInt("intField");
        int wis = GetInt("wisField");
        int cha = GetInt("chaField");
        int level = GetInt("levelField");

        int strMod = Modifier(str);
        int dexMod = Modifier(dex);
        int conMod = Modifier(con);
        int intMod = Modifier(intel);
        int wisMod = Modifier(wis);
        int chaMod = Modifier(cha);

        int profBonus = 2 + (level - 1) / 4; // 2 на 1-4 уровне, 3 на 5-8, и т.д.
        SetInt("profBonusField", profBonus);

        SetModifierLabel("strModifier", strMod);
        SetModifierLabel("dexModifier", dexMod);
        SetModifierLabel("conModifier", conMod);
        SetModifierLabel("intModifier", intMod);
        SetModifierLabel("wisModifier", wisMod);
        SetModifierLabel("chaModifier", chaMod);

        // Спасброски
        SetSkillValue("strSaveValue", "strSaveProf", strMod, profBonus);
        SetSkillValue("dexSaveValue", "dexSaveProf", dexMod, profBonus);
        SetSkillValue("conSaveValue", "conSaveProf", conMod, profBonus);
        SetSkillValue("intSaveValue", "intSaveProf", intMod, profBonus);
        SetSkillValue("wisSaveValue", "wisSaveProf", wisMod, profBonus);
        SetSkillValue("chaSaveValue", "chaSaveProf", chaMod, profBonus);

        // Навыки силы
        SetSkillValue("athleticsValue", "athleticsProf", strMod, profBonus);

        // Навыки ловкости
        SetSkillValue("acrobaticsValue", "acrobaticsProf", dexMod, profBonus);
        SetSkillValue("sleightOfHandValue", "sleightOfHandProf", dexMod, profBonus);
        SetSkillValue("stealthValue", "stealthProf", dexMod, profBonus);

        // Навыки мудрости
        SetSkillValue("perceptionValue", "perceptionProf", wisMod, profBonus);
        SetSkillValue("survivalValue", "survivalProf", wisMod, profBonus);
        SetSkillValue("medicineValue", "medicineProf", wisMod, profBonus);
        SetSkillValue("insightValue", "insightProf", wisMod, profBonus);
        SetSkillValue("animalHandlingValue", "animalHandlingProf", wisMod, profBonus);

        // Навыки интеллекта
        SetSkillValue("analysisValue", "analysisProf", intMod, profBonus);
        SetSkillValue("historyValue", "historyProf", intMod, profBonus);
        SetSkillValue("magicValue", "magicProf", intMod, profBonus);
        SetSkillValue("natureValue", "natureProf", intMod, profBonus);
        SetSkillValue("religionValue", "religionProf", intMod, profBonus);

        // Навыки харизмы
        SetSkillValue("performanceValue", "performanceProf", chaMod, profBonus);
        SetSkillValue("intimidationValue", "intimidationProf", chaMod, profBonus);
        SetSkillValue("deceptionValue", "deceptionProf", chaMod, profBonus);
        SetSkillValue("persuasionValue", "persuasionProf", chaMod, profBonus);

        // Инициатива = модификатор ловкости
        SetInt("initiativeField", dexMod);

        // Пассивное восприятие = 10 + восприятие
        bool perceptionProf = GetToggle("perceptionProf");
        int perceptionTotal = wisMod + (perceptionProf ? profBonus : 0);
        SetInt("passivePerceptionField", 10 + perceptionTotal);
    }

    private void SetSkillValue(string labelName, string toggleName, int statMod, int profBonus)
    {
        bool hasProf = GetToggle(toggleName);
        int total = statMod + (hasProf ? profBonus : 0);
        SetSkillLabel(labelName, total);
    }

    private void SetModifierLabel(string name, int mod)
    {
        var label = root.Q<Label>(name);
        if (label != null) label.text = (mod >= 0 ? "+" : "") + mod;
    }

    private void SetSkillLabel(string name, int value)
    {
        var label = root.Q<Label>(name);
        if (label != null) label.text = (value >= 0 ? "+" : "") + value;
    }

    // ===== Вспомогательные геттеры/сеттеры =====

    private void SetText(string name, string value)
    {
        var f = root.Q<TextField>(name);
        if (f != null) f.value = value ?? "";
    }

    private string GetText(string name)
    {
        var f = root.Q<TextField>(name);
        return f != null ? f.value : "";
    }

    private void SetInt(string name, int value)
    {
        var f = root.Q<IntegerField>(name);
        if (f != null) f.SetValueWithoutNotify(value);
    }

    private int GetInt(string name)
    {
        var f = root.Q<IntegerField>(name);
        return f != null ? f.value : 0;
    }

    private void SetDropdown(string name, string value)
    {
        var f = root.Q<DropdownField>(name);
        if (f != null) f.value = value;
    }

    private string GetDropdown(string name)
    {
        var f = root.Q<DropdownField>(name);
        return f != null ? f.value : "";
    }

    private void SetToggle(string name, bool value)
    {
        var f = root.Q<Toggle>(name);
        if (f != null) f.SetValueWithoutNotify(value);
    }

    private bool GetToggle(string name)
    {
        var f = root.Q<Toggle>(name);
        return f != null && f.value;
    }

    private static IEnumerable<string> AllProficiencyToggleNames()
    {
        yield return "strSaveProf";
        yield return "dexSaveProf";
        yield return "conSaveProf";
        yield return "intSaveProf";
        yield return "wisSaveProf";
        yield return "chaSaveProf";
        yield return "athleticsProf";
        yield return "acrobaticsProf";
        yield return "sleightOfHandProf";
        yield return "stealthProf";
        yield return "perceptionProf";
        yield return "survivalProf";
        yield return "medicineProf";
        yield return "insightProf";
        yield return "animalHandlingProf";
        yield return "analysisProf";
        yield return "historyProf";
        yield return "magicProf";
        yield return "natureProf";
        yield return "religionProf";
        yield return "performanceProf";
        yield return "intimidationProf";
        yield return "deceptionProf";
        yield return "persuasionProf";
    }

    // ===== Сохранение =====

    private void OnSaveClicked()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        string errors = "";

        string name = GetText("nameField");
        string race = GetDropdown("raceField");
        string charClass = GetDropdown("classField");
        int level = GetInt("levelField");

        if (string.IsNullOrEmpty(name))
            errors += "• Имя обязательно для заполнения\n";
        if (string.IsNullOrEmpty(race))
            errors += "• Раса обязательна для заполнения\n";
        if (string.IsNullOrEmpty(charClass))
            errors += "• Класс обязателен для заполнения\n";
        if (level < 1 || level > 20)
            errors += "• Уровень должен быть от 1 до 20\n";

        int str = GetInt("strField");
        int dex = GetInt("dexField");
        int con = GetInt("conField");
        int intel = GetInt("intField");
        int wis = GetInt("wisField");
        int cha = GetInt("chaField");

        if (str < 1 || str > 30) errors += "• Сила должна быть от 1 до 30\n";
        if (dex < 1 || dex > 30) errors += "• Ловкость должна быть от 1 до 30\n";
        if (con < 1 || con > 30) errors += "• Телосложение должно быть от 1 до 30\n";
        if (intel < 1 || intel > 30) errors += "• Интеллект должен быть от 1 до 30\n";
        if (wis < 1 || wis > 30) errors += "• Мудрость должна быть от 1 до 30\n";
        if (cha < 1 || cha > 30) errors += "• Харизма должна быть от 1 до 30\n";

        int experience = GetInt("experienceField");
        if (experience < 0) errors += "• Опыт не может быть отрицательным\n";

        if (!string.IsNullOrEmpty(errors))
        {
            ShowError(errors.TrimEnd());
            return;
        }

        ClearError();

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

        character.name = name;
        character.background = GetText("backgroundField");
        character.subclass = GetText("subclassField");
        character.race = race;
        character.characterClass = charClass;

        character.level = level;
        character.experience = experience;
        character.armorClass = GetInt("acField");
        character.hpCurrent = GetInt("hpCurrentField");
        character.hpTemp = GetInt("hpTempField");
        character.hpMax = GetInt("hpMaxField");

        character.proficiencyBonus = GetInt("profBonusField");
        character.initiative = GetInt("initiativeField");
        character.speed = GetInt("speedField");
        character.passivePerception = GetInt("passivePerceptionField");
        character.trait = GetText("traitField");
        character.spellcastingMod = GetText("spellModField");

        character.strength = str;
        character.dexterity = dex;
        character.constitution = con;
        character.intelligence = intel;
        character.wisdom = wis;
        character.charisma = cha;

        character.weaponsText = GetText("weaponsField");
        character.equipmentText = GetText("equipmentField");
        character.spellsText = GetText("spellsField");

        character.skills = new SkillProficiencies
        {
            strSave = GetToggle("strSaveProf"),
            dexSave = GetToggle("dexSaveProf"),
            conSave = GetToggle("conSaveProf"),
            intSave = GetToggle("intSaveProf"),
            wisSave = GetToggle("wisSaveProf"),
            chaSave = GetToggle("chaSaveProf"),
            athletics = GetToggle("athleticsProf"),
            acrobatics = GetToggle("acrobaticsProf"),
            sleightOfHand = GetToggle("sleightOfHandProf"),
            stealth = GetToggle("stealthProf"),
            perception = GetToggle("perceptionProf"),
            survival = GetToggle("survivalProf"),
            medicine = GetToggle("medicineProf"),
            insight = GetToggle("insightProf"),
            animalHandling = GetToggle("animalHandlingProf"),
            analysis = GetToggle("analysisProf"),
            history = GetToggle("historyProf"),
            magic = GetToggle("magicProf"),
            nature = GetToggle("natureProf"),
            religion = GetToggle("religionProf"),
            performance = GetToggle("performanceProf"),
            intimidation = GetToggle("intimidationProf"),
            deception = GetToggle("deceptionProf"),
            persuasion = GetToggle("persuasionProf"),
        };

        DataManager.SaveCharacter(character);
        Debug.Log("Сохранён: " + character.name);

        gameObject.SetActive(false);

        var mainMenu = FindAnyObjectByType<UIMainMenu>();
        if (mainMenu != null) mainMenu.RefreshList();
    }

    private void OnCancelClicked()
    {
        ClearError();
        gameObject.SetActive(false);
    }

    private void ShowError(string message)
    {
        var errorLabel = root.Q<Label>("errorLabel");
        if (errorLabel != null) errorLabel.text = message;
    }

    private void ClearError()
    {
        var errorLabel = root.Q<Label>("errorLabel");
        if (errorLabel != null) errorLabel.text = "";
    }
}
