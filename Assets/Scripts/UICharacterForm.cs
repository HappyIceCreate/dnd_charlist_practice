using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UICharacterForm : MonoBehaviour
{
    private VisualElement root;
    private bool isEditMode = false;
    private bool isViewMode = false;
    private string editingCharacterId = null;

    private List<InventoryItem> inventoryList = new List<InventoryItem>();
    private List<Spell> spellsList = new List<Spell>();
    private List<WeaponItem> weaponsList = new List<WeaponItem>();
    private int[] appliedRaceBonus = new int[6];

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

    private class SpellDefinition
    {
        public string name;
        public int level;
        public string description;
        public List<string> classes;
    }

    private static readonly List<SpellDefinition> SpellDatabase = new List<SpellDefinition>
    {
        new SpellDefinition { name = "Злая насмешка", level = 0, description = "Заговор барда. Насмешка причиняет психический урон и накладывает помеху на следующую атаку цели.", classes = new List<string>{ "Бард" } },
        new SpellDefinition { name = "Огненный снаряд", level = 0, description = "Заговор, метающий огненный снаряд, причиняющий урон огнём.", classes = new List<string>{ "Волшебник", "Чародей" } },
        new SpellDefinition { name = "Волшебная рука", level = 0, description = "Создаёт спектральную руку для манипуляции предметами на расстоянии.", classes = new List<string>{ "Бард", "Волшебник", "Чародей", "Колдун" } },
        new SpellDefinition { name = "Лечащее слово", level = 1, description = "Бонусным действием исцеляет существо на расстоянии.", classes = new List<string>{ "Бард", "Жрец", "Друид" } },
        new SpellDefinition { name = "Лечение ран", level = 1, description = "Касанием восстанавливает хиты существу.", classes = new List<string>{ "Бард", "Жрец", "Друид", "Паладин", "Следопыт" } },
        new SpellDefinition { name = "Феерический огонь", level = 1, description = "Подсвечивает существ в области, лишая их преимущества невидимости.", classes = new List<string>{ "Друид" } },
        new SpellDefinition { name = "Духовное оружие", level = 2, description = "Создаёт спектральное оружие, атакующее бонусным действием.", classes = new List<string>{ "Жрец" } },
        new SpellDefinition { name = "Малое восстановление", level = 2, description = "Снимает одну болезнь или состояние (глухота, отравление, паралич, слепота).", classes = new List<string>{ "Бард", "Жрец", "Друид", "Паладин", "Следопыт" } },
        new SpellDefinition { name = "Тёмное зрение", level = 2, description = "Даёт цели тёмное зрение на 8 часов.", classes = new List<string>{ "Волшебник", "Чародей", "Следопыт" } },
        new SpellDefinition { name = "Гипнотический узор", level = 3, description = "Завораживает существ в области, они получают состояние заворожён.", classes = new List<string>{ "Бард", "Волшебник", "Чародей", "Колдун" } },
        new SpellDefinition { name = "Возрождение", level = 3, description = "Возвращает к жизни существо, мёртвое не больше минуты, с 1 хитом.", classes = new List<string>{ "Жрец", "Друид", "Паладин", "Следопыт" } },
        new SpellDefinition { name = "Аура жизни", level = 4, description = "Аура даёт сопротивление некротическому урону и не даёт снижаться максимуму хитов союзников.", classes = new List<string>{ "Жрец", "Паладин" } },
        new SpellDefinition { name = "Принуждение", level = 4, description = "Существа в области должны двигаться в указанном направлении под угрозой психического урона.", classes = new List<string>{ "Бард" } },
        new SpellDefinition { name = "Водоворот", level = 5, description = "Создаёт водоворот, затягивающий и наносящий дробящий урон существам поблизости.", classes = new List<string>{ "Друид" } },
        new SpellDefinition { name = "Облако смерти", level = 5, description = "Ядовитое облако в области, наносящее урон ядом каждый ход.", classes = new List<string>{ "Волшебник", "Чародей" } },
        new SpellDefinition { name = "Мираж", level = 7, description = "Изменяет ландшафт местности иллюзорно на большой площади.", classes = new List<string>{ "Бард", "Друид", "Волшебник" } },
    };

    private class WeaponDefinition
    {
        public string name;
        public string damage;
        public float weight;
        public List<string> classes;
    }

    private static readonly List<WeaponDefinition> WeaponDatabase = new List<WeaponDefinition>
    {
        new WeaponDefinition { name = "Кинжал",       damage = "1к4 колющий",   weight = 1f, classes = new List<string>{ "Воин","Волшебник","Плут","Жрец","Друид","Бард","Паладин","Следопыт","Монах","Варвар","Чародей","Колдун" } },
        new WeaponDefinition { name = "Лёгкий арбалет", damage = "1к8 колющий",  weight = 5f, classes = new List<string>{ "Воин","Волшебник","Плут","Жрец","Бард","Паладин","Следопыт","Монах","Варвар","Чародей","Колдун" } },
        new WeaponDefinition { name = "Секира",       damage = "1к8 рубящий",   weight = 4f, classes = new List<string>{ "Воин","Паладин","Следопыт","Варвар" } },
        new WeaponDefinition { name = "Ручной топор", damage = "1к6 рубящий",   weight = 2f, classes = new List<string>{ "Воин","Плут","Жрец","Бард","Паладин","Следопыт","Монах","Варвар","Колдун" } },
        new WeaponDefinition { name = "Боевой посох", damage = "1к6 дробящий",  weight = 4f, classes = new List<string>{ "Воин","Волшебник","Плут","Жрец","Друид","Бард","Паладин","Следопыт","Монах","Варвар","Чародей","Колдун" } },
        new WeaponDefinition { name = "Скимитар",     damage = "1к6 рубящий",   weight = 3f, classes = new List<string>{ "Воин","Друид","Паладин","Следопыт","Варвар" } },
        new WeaponDefinition { name = "Короткий меч", damage = "1к6 колющий",   weight = 2f, classes = new List<string>{ "Воин","Плут","Бард","Паладин","Следопыт","Монах","Варвар" } },
        new WeaponDefinition { name = "Булава",       damage = "1к6 дробящий",  weight = 4f, classes = new List<string>{ "Воин","Плут","Жрец","Друид","Бард","Паладин","Следопыт","Монах","Варвар","Колдун" } },
        new WeaponDefinition { name = "Боевой молот", damage = "1к8 дробящий",  weight = 2f, classes = new List<string>{ "Воин","Паладин","Следопыт","Варвар" } },
        new WeaponDefinition { name = "Рапира",       damage = "1к8 колющий",   weight = 2f, classes = new List<string>{ "Воин","Плут","Бард","Паладин","Следопыт","Варвар" } },
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
        StartCoroutine(FillFormDelayed(character, false));
    }

    public void SetViewMode(CharacterData character)
    {
        gameObject.SetActive(true);
        StartCoroutine(FillFormDelayed(character, true));
    }

    private IEnumerator FillFormDelayed(CharacterData character, bool viewMode = false)
    {
        yield return null;

        root = GetComponent<UIDocument>().rootVisualElement;
        isViewMode = viewMode;

        var raceField = root.Q<DropdownField>("raceField");
        var classField = root.Q<DropdownField>("classField");
        if (raceField != null) raceField.choices = Races;
        if (classField != null) classField.choices = Classes;

        if (character == null)
        {
            isEditMode = false;
            editingCharacterId = null;
            FillEmptyForm();
            inventoryList = new List<InventoryItem>();
            spellsList = new List<Spell>();
            weaponsList = new List<WeaponItem>();
            appliedRaceBonus = new int[6];
        }
        else
        {
            isEditMode = true;
            editingCharacterId = character.id;
            FillFormFromCharacter(character);
            inventoryList = character.inventory != null ? new List<InventoryItem>(character.inventory) : new List<InventoryItem>();
            spellsList = character.spells != null ? new List<Spell>(character.spells) : new List<Spell>();
            weaponsList = character.weapons != null ? new List<WeaponItem>(character.weapons) : new List<WeaponItem>();
            appliedRaceBonus = GetRaceBonus(character.race);
        }

        ClearError();
        RecalculateAll();
        RebuildInventoryUI();
        RebuildSpellsUI();
        RebuildWeaponsUI();
        RefreshSpellPicker();
        RefreshWeaponPicker();
        SubscribeEvents();
        ApplyViewModeState();
    }

    private void ApplyViewModeState()
    {
        var saveBtn = root.Q<Button>("saveBtn");
        var cancelBtn = root.Q<Button>("cancelBtn");
        var rootContainer = root.Q<VisualElement>("rootContainer");

        if (isViewMode)
        {
            if (rootContainer != null)
            {
                var buttonRow = cancelBtn != null ? cancelBtn.parent : null;
                foreach (var child in rootContainer.Children().ToList())
                {
                    if (child == buttonRow) continue;
                    child.SetEnabled(false);
                    RemoveDisabledDimming(child);
                }
            }
            if (cancelBtn != null)
            {
                cancelBtn.SetEnabled(true);
                cancelBtn.text = "Закрыть";
            }
            if (saveBtn != null) saveBtn.style.display = DisplayStyle.None;
        }
        else
        {
            if (rootContainer != null)
            {
                foreach (var child in rootContainer.Children().ToList())
                    child.SetEnabled(true);
            }
            if (cancelBtn != null) cancelBtn.text = "Отмена";
            if (saveBtn != null) saveBtn.style.display = DisplayStyle.Flex;
        }
    }

    private void RemoveDisabledDimming(VisualElement ve)
    {
        ve.style.opacity = 1f;
        foreach (var child in ve.Children())
            RemoveDisabledDimming(child);
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

        SetInt("profBonusField", 2);
        SetInt("initiativeField", 0);
        SetInt("speedField", 30);

        SetInt("strField", 10);
        SetInt("dexField", 10);
        SetInt("conField", 10);
        SetInt("intField", 10);
        SetInt("wisField", 10);
        SetInt("chaField", 10);

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

        SetInt("profBonusField", c.proficiencyBonus);
        SetInt("initiativeField", c.initiative);
        SetInt("speedField", c.speed);

        SetInt("strField", c.strength);
        SetInt("dexField", c.dexterity);
        SetInt("conField", c.constitution);
        SetInt("intField", c.intelligence);
        SetInt("wisField", c.wisdom);
        SetInt("chaField", c.charisma);

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

    private void RebuildInventoryUI()
    {
        var container = root.Q<VisualElement>("inventoryContainer");
        if (container == null) return;
        container.Clear();

        for (int i = 0; i < inventoryList.Count; i++)
        {
            int index = i;
            var item = inventoryList[i];

            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.marginBottom = 4;
            row.style.alignItems = Align.Center;

            var nameField = new TextField();
            nameField.value = item.itemName;
            nameField.style.width = 280;
            nameField.style.marginRight = 4;
            nameField.RegisterValueChangedCallback(evt => { inventoryList[index].itemName = evt.newValue; });

            var qtyField = new IntegerField();
            qtyField.value = item.quantity;
            qtyField.style.width = 50;
            qtyField.style.marginRight = 4;
            qtyField.RegisterValueChangedCallback(evt => { inventoryList[index].quantity = evt.newValue; UpdateInventoryWeightLabel(); });

            var weightField = new FloatField();
            weightField.value = item.weight;
            weightField.style.width = 55;
            weightField.style.marginRight = 4;
            weightField.RegisterValueChangedCallback(evt => { inventoryList[index].weight = evt.newValue; UpdateInventoryWeightLabel(); });

            var removeBtn = new Button(() =>
            {
                inventoryList.RemoveAt(index);
                RebuildInventoryUI();
            });
            removeBtn.text = "✕";
            removeBtn.style.width = 26;
            removeBtn.style.height = 26;
            removeBtn.style.paddingLeft = 0;
            removeBtn.style.paddingRight = 0;

            row.Add(nameField);
            row.Add(qtyField);
            row.Add(weightField);
            row.Add(removeBtn);
            container.Add(row);
        }

        UpdateInventoryWeightLabel();
    }

    private void UpdateInventoryWeightLabel()
    {
        var label = root.Q<Label>("inventoryWeightLabel");
        if (label == null) return;

        float totalWeight = inventoryList.Sum(i => i.weight * i.quantity) + weaponsList.Sum(w => w.weight);
        float capacity = GetInt("strField") * 15;

        label.text = $"Вес: {totalWeight:0.##} / {capacity:0.##} фунтов (с учётом оружия)";
        label.style.color = totalWeight > capacity
            ? new Color(0.85f, 0.25f, 0.25f)
            : new Color(0.24f, 0.17f, 0.12f);
    }

    private void RebuildWeaponsUI()
    {
        var container = root.Q<VisualElement>("weaponsContainer");
        if (container == null) return;
        container.Clear();

        for (int i = 0; i < weaponsList.Count; i++)
        {
            int index = i;
            var weapon = weaponsList[i];

            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.marginBottom = 4;
            row.style.alignItems = Align.Center;

            var nameLabel = new TextField();
            nameLabel.value = weapon.name;
            nameLabel.style.width = 160;
            nameLabel.style.marginRight = 4;
            nameLabel.SetEnabled(false);

            var damageLabel = new TextField();
            damageLabel.value = weapon.damage;
            damageLabel.style.width = 110;
            damageLabel.style.marginRight = 4;
            damageLabel.SetEnabled(false);

            var weightLabel = new FloatField();
            weightLabel.value = weapon.weight;
            weightLabel.style.width = 55;
            weightLabel.style.marginRight = 4;
            weightLabel.SetEnabled(false);

            var removeBtn = new Button(() =>
            {
                weaponsList.RemoveAt(index);
                RebuildWeaponsUI();
                UpdateInventoryWeightLabel();
            });
            removeBtn.text = "✕";
            removeBtn.style.width = 26;
            removeBtn.style.height = 26;
            removeBtn.style.paddingLeft = 0;
            removeBtn.style.paddingRight = 0;

            row.Add(nameLabel);
            row.Add(damageLabel);
            row.Add(weightLabel);
            row.Add(removeBtn);
            container.Add(row);
        }
    }

    private string FormatWeaponChoice(WeaponDefinition def)
    {
        return $"{def.name} ({def.damage}, {def.weight:0.#} фн.)";
    }

    private void RefreshWeaponPicker()
    {
        var picker = root.Q<DropdownField>("weaponPickerField");
        if (picker == null) return;

        string currentClass = GetDropdown("classField");

        var available = string.IsNullOrEmpty(currentClass)
            ? WeaponDatabase
            : WeaponDatabase.Where(w => w.classes.Contains(currentClass)).ToList();

        var choices = available.Select(FormatWeaponChoice).ToList();
        if (choices.Count == 0) choices.Add("(нет доступного оружия)");

        picker.choices = choices;
        picker.index = 0;
    }

    private void OnAddWeaponClicked()
    {
        var picker = root.Q<DropdownField>("weaponPickerField");
        if (picker == null || string.IsNullOrEmpty(picker.value)) return;

        var def = WeaponDatabase.FirstOrDefault(w => FormatWeaponChoice(w) == picker.value);
        if (def == null) return;

        if (weaponsList.Any(w => w.name == def.name)) return;

        weaponsList.Add(new WeaponItem { name = def.name, damage = def.damage, weight = def.weight });
        RebuildWeaponsUI();
        UpdateInventoryWeightLabel();
    }

    private void RebuildSpellsUI()
    {
        var container = root.Q<VisualElement>("spellsContainer");
        if (container == null) return;
        container.Clear();

        for (int i = 0; i < spellsList.Count; i++)
        {
            int index = i;
            var spell = spellsList[i];
            bool isLocked = !string.IsNullOrEmpty(spell.name) && SpellDatabase.Any(s => s.name == spell.name);

            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.marginBottom = 4;
            row.style.alignItems = Align.Center;

            var nameField = new TextField();
            nameField.value = spell.name;
            nameField.style.width = 220;
            nameField.style.marginRight = 4;
            nameField.isReadOnly = isLocked;
            nameField.RegisterValueChangedCallback(evt => { spellsList[index].name = evt.newValue; });

            var levelField = new IntegerField();
            levelField.value = spell.level;
            levelField.style.width = 40;
            levelField.style.marginRight = 4;
            levelField.SetEnabled(!isLocked);
            levelField.RegisterValueChangedCallback(evt =>
            {
                spellsList[index].level = evt.newValue;
                RebuildSpellHistogram();
            });

            var descField = new TextField();
            descField.value = spell.description;
            descField.style.width = 220;
            descField.style.marginRight = 4;
            descField.isReadOnly = isLocked;
            descField.RegisterValueChangedCallback(evt => { spellsList[index].description = evt.newValue; });

            var removeBtn = new Button(() =>
            {
                spellsList.RemoveAt(index);
                RebuildSpellsUI();
                RebuildSpellHistogram();
            });
            removeBtn.text = "✕";
            removeBtn.style.width = 26;
            removeBtn.style.height = 26;
            removeBtn.style.paddingLeft = 0;
            removeBtn.style.paddingRight = 0;

            row.Add(nameField);
            row.Add(levelField);
            row.Add(descField);
            row.Add(removeBtn);
            container.Add(row);
        }

        RebuildSpellHistogram();
    }

    private void RebuildSpellHistogram()
    {
        var histogram = root.Q<VisualElement>("spellHistogram");
        if (histogram == null) return;
        histogram.Clear();

        int[] counts = new int[10];
        foreach (var spell in spellsList)
        {
            if (spell.level >= 0 && spell.level <= 9)
                counts[spell.level]++;
        }

        int maxCount = counts.Max();
        if (maxCount == 0) maxCount = 1;

        const float barAreaHeight = 110f;

        for (int lvl = 0; lvl <= 9; lvl++)
        {
            var col = new VisualElement();
            col.style.flexGrow = 1;
            col.style.alignItems = Align.Center;
            col.style.justifyContent = Justify.FlexEnd;
            col.style.height = new Length(100, LengthUnit.Percent);

            var countLabel = new Label(counts[lvl].ToString());
            countLabel.style.fontSize = 15;
            countLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            countLabel.style.color = new Color(1f, 1f, 1f);
            countLabel.style.marginBottom = 3;

            var bar = new VisualElement();
            float barHeight = counts[lvl] == 0 ? 4f : Mathf.Max((float)counts[lvl] / maxCount * barAreaHeight, 8f);
            bar.style.width = 28;
            bar.style.height = barHeight;
            bar.style.backgroundColor = counts[lvl] == 0
                ? new Color(0.45f, 0.4f, 0.32f)
                : new Color(0.95f, 0.8f, 0.4f);
            bar.style.borderTopLeftRadius = 4;
            bar.style.borderTopRightRadius = 3;

            var lvlLabel = new Label(lvl.ToString());
            lvlLabel.style.fontSize = 12;
            lvlLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            lvlLabel.style.color = new Color(1f, 1f, 1f);
            lvlLabel.style.marginTop = 4;

            col.Add(countLabel);
            col.Add(bar);
            col.Add(lvlLabel);
            histogram.Add(col);
        }
    }

    private void SubscribeEvents()
    {
        foreach (var statName in new[] { "strField", "dexField", "conField", "intField", "wisField", "chaField" })
        {
            var field = root.Q<IntegerField>(statName);
            if (field != null)
            {
                field.UnregisterValueChangedCallback(OnStatChanged);
                field.RegisterValueChangedCallback(OnStatChanged);
            }
        }

        var levelField = root.Q<IntegerField>("levelField");
        if (levelField != null)
        {
            levelField.UnregisterValueChangedCallback(OnLevelChanged);
            levelField.RegisterValueChangedCallback(OnLevelChanged);
        }

        var experienceField = root.Q<IntegerField>("experienceField");
        if (experienceField != null)
        {
            experienceField.UnregisterValueChangedCallback(OnExperienceChanged);
            experienceField.RegisterValueChangedCallback(OnExperienceChanged);
        }

        var raceDropdown = root.Q<DropdownField>("raceField");
        if (raceDropdown != null)
        {
            raceDropdown.UnregisterValueChangedCallback(OnRaceChanged);
            raceDropdown.RegisterValueChangedCallback(OnRaceChanged);
        }

        var classDropdown = root.Q<DropdownField>("classField");
        if (classDropdown != null)
        {
            classDropdown.UnregisterValueChangedCallback(OnClassChangedForSpells);
            classDropdown.RegisterValueChangedCallback(OnClassChangedForSpells);
        }

        foreach (var toggleName in AllProficiencyToggleNames())
        {
            var toggle = root.Q<Toggle>(toggleName);
            if (toggle != null)
            {
                toggle.UnregisterValueChangedCallback(OnToggleChanged);
                toggle.RegisterValueChangedCallback(OnToggleChanged);
            }
        }

        var saveBtn = root.Q<Button>("saveBtn");
        var cancelBtn = root.Q<Button>("cancelBtn");
        if (saveBtn != null) { saveBtn.clicked -= OnSaveClicked; saveBtn.clicked += OnSaveClicked; }
        if (cancelBtn != null) { cancelBtn.clicked -= OnCancelClicked; cancelBtn.clicked += OnCancelClicked; }

        var addInvBtn = root.Q<Button>("addInventoryBtn");
        if (addInvBtn != null)
        {
            addInvBtn.clicked -= OnAddInventoryClicked;
            addInvBtn.clicked += OnAddInventoryClicked;
        }

        var addSpellBtn = root.Q<Button>("addSpellBtn");
        if (addSpellBtn != null)
        {
            addSpellBtn.clicked -= OnAddSpellClicked;
            addSpellBtn.clicked += OnAddSpellClicked;
        }

        var addWeaponBtn = root.Q<Button>("addWeaponBtn");
        if (addWeaponBtn != null)
        {
            addWeaponBtn.clicked -= OnAddWeaponClicked;
            addWeaponBtn.clicked += OnAddWeaponClicked;
        }
    }

    private void OnAddInventoryClicked()
    {
        inventoryList.Add(new InventoryItem { itemName = "", quantity = 1, weight = 0f });
        RebuildInventoryUI();
    }

    private const string NewSpellOption = "➕ Новое заклинание (своё)";

    private void OnAddSpellClicked()
    {
        var picker = root.Q<DropdownField>("spellPickerField");
        if (picker == null || string.IsNullOrEmpty(picker.value)) return;

        if (picker.value == NewSpellOption)
        {
            spellsList.Add(new Spell { name = "", level = 0, description = "" });
            RebuildSpellsUI();
            return;
        }

        var def = SpellDatabase.FirstOrDefault(s => FormatSpellChoice(s) == picker.value);
        if (def == null) return;

        if (spellsList.Any(s => s.name == def.name)) return;

        spellsList.Add(new Spell { name = def.name, level = def.level, description = def.description });
        RebuildSpellsUI();
    }

    private string FormatSpellChoice(SpellDefinition def)
    {
        return $"{def.name} (ур. {def.level})";
    }

    private void RefreshSpellPicker()
    {
        var picker = root.Q<DropdownField>("spellPickerField");
        if (picker == null) return;

        string currentClass = GetDropdown("classField");

        var available = string.IsNullOrEmpty(currentClass)
            ? SpellDatabase
            : SpellDatabase.Where(s => s.classes.Contains(currentClass)).ToList();

        var choices = available.Select(FormatSpellChoice).ToList();
        choices.Insert(0, NewSpellOption);

        picker.choices = choices;
        picker.index = 0;
    }

    private void OnClassChangedForSpells(ChangeEvent<string> evt)
    {
        RefreshSpellPicker();
        RefreshWeaponPicker();
    }

    private void ApplyRaceBonusToFields(string race)
    {
        int[] newBonus = GetRaceBonus(race);
        string[] fields = { "strField", "dexField", "conField", "intField", "wisField", "chaField" };
        for (int i = 0; i < 6; i++)
        {
            int current = GetInt(fields[i]);
            int updated = current - appliedRaceBonus[i] + newBonus[i];
            SetInt(fields[i], updated);
        }
        appliedRaceBonus = newBonus;
    }

    private static readonly int[] XpThresholds =
    {
        0, 0, 300, 900, 2700, 6500, 14000, 23000, 34000, 48000, 64000,
        85000, 100000, 120000, 140000, 165000, 195000, 225000, 265000, 305000, 355000
    };

    private int MaxSpellLevelForCharacterLevel(int charLevel)
    {
        if (charLevel < 1) return 0;
        return Mathf.Min((charLevel + 1) / 2, 9);
    }

    private int LevelFromXP(int xp)
    {
        for (int lvl = 20; lvl >= 1; lvl--)
        {
            if (xp >= XpThresholds[lvl]) return lvl;
        }
        return 1;
    }

    private int MinXPForLevel(int level)
    {
        level = Mathf.Clamp(level, 1, 20);
        return XpThresholds[level];
    }

    private void OnLevelChanged(ChangeEvent<int> evt)
    {
        SetInt("experienceField", MinXPForLevel(evt.newValue));
        RecalculateAll();
    }

    private void OnExperienceChanged(ChangeEvent<int> evt)
    {
        SetInt("levelField", LevelFromXP(evt.newValue));
        RecalculateAll();
    }

    private void OnStatChanged(ChangeEvent<int> evt) => RecalculateAll();
    private void OnToggleChanged(ChangeEvent<bool> evt) => RecalculateAll();
    private void OnRaceChanged(ChangeEvent<string> evt)
    {
        ApplyRaceBonusToFields(evt.newValue);
        RecalculateAll();
    }

    private int Modifier(int statValue) => Mathf.FloorToInt((statValue - 10) / 2f);

    private static readonly Dictionary<string, int[]> RaceBonuses = new Dictionary<string, int[]>
    {
        { "Человек",            new[] { 1, 1, 1, 1, 1, 1 } },
        { "Эльф",                new[] { 0, 2, 0, 1, 0, 0 } },
        { "Дварф",               new[] { 0, 0, 2, 0, 1, 0 } },
        { "Полурослик",          new[] { 0, 2, 0, 0, 0, 1 } },
        { "Гном",                new[] { 0, 1, 0, 2, 0, 0 } },
        { "Полуэльф",            new[] { 0, 1, 1, 0, 0, 2 } },
        { "Полуорк",             new[] { 2, 0, 1, 0, 0, 0 } },
        { "Тифлинг",             new[] { 0, 0, 0, 1, 0, 2 } },
        { "Драконорождённый",    new[] { 2, 0, 0, 0, 0, 1 } },
    };

    private static readonly HashSet<string> SlowRaces = new HashSet<string> { "Гном", "Дварф", "Полурослик" };

    private int[] GetRaceBonus(string race)
    {
        if (!string.IsNullOrEmpty(race) && RaceBonuses.TryGetValue(race, out var bonus))
            return bonus;
        return new[] { 0, 0, 0, 0, 0, 0 };
    }

    private int GetRaceSpeed(string race)
    {
        return SlowRaces.Contains(race) ? 25 : 30;
    }

    private void RecalculateAll()
    {
        if (root == null) return;

        string race = GetDropdown("raceField");
        int[] rb = GetRaceBonus(race);

        int str = GetInt("strField");
        int dex = GetInt("dexField");
        int con = GetInt("conField");
        int intel = GetInt("intField");
        int wis = GetInt("wisField");
        int cha = GetInt("chaField");
        int level = GetInt("levelField");

        SetInt("speedField", GetRaceSpeed(race));

        int strMod = Modifier(str);
        int dexMod = Modifier(dex);
        int conMod = Modifier(con);
        int intMod = Modifier(intel);
        int wisMod = Modifier(wis);
        int chaMod = Modifier(cha);

        int profBonus = 2 + (level - 1) / 4;
        SetInt("profBonusField", profBonus);

        SetModifierLabel("strModifier", strMod);
        SetModifierLabel("dexModifier", dexMod);
        SetModifierLabel("conModifier", conMod);
        SetModifierLabel("intModifier", intMod);
        SetModifierLabel("wisModifier", wisMod);
        SetModifierLabel("chaModifier", chaMod);

        SetSkillValue("strSaveValue", "strSaveProf", strMod, profBonus);
        SetSkillValue("dexSaveValue", "dexSaveProf", dexMod, profBonus);
        SetSkillValue("conSaveValue", "conSaveProf", conMod, profBonus);
        SetSkillValue("intSaveValue", "intSaveProf", intMod, profBonus);
        SetSkillValue("wisSaveValue", "wisSaveProf", wisMod, profBonus);
        SetSkillValue("chaSaveValue", "chaSaveProf", chaMod, profBonus);

        SetSkillValue("athleticsValue", "athleticsProf", strMod, profBonus);
        SetSkillValue("acrobaticsValue", "acrobaticsProf", dexMod, profBonus);
        SetSkillValue("sleightOfHandValue", "sleightOfHandProf", dexMod, profBonus);
        SetSkillValue("stealthValue", "stealthProf", dexMod, profBonus);
        SetSkillValue("perceptionValue", "perceptionProf", wisMod, profBonus);
        SetSkillValue("survivalValue", "survivalProf", wisMod, profBonus);
        SetSkillValue("medicineValue", "medicineProf", wisMod, profBonus);
        SetSkillValue("insightValue", "insightProf", wisMod, profBonus);
        SetSkillValue("animalHandlingValue", "animalHandlingProf", wisMod, profBonus);
        SetSkillValue("analysisValue", "analysisProf", intMod, profBonus);
        SetSkillValue("historyValue", "historyProf", intMod, profBonus);
        SetSkillValue("magicValue", "magicProf", intMod, profBonus);
        SetSkillValue("natureValue", "natureProf", intMod, profBonus);
        SetSkillValue("religionValue", "religionProf", intMod, profBonus);
        SetSkillValue("performanceValue", "performanceProf", chaMod, profBonus);
        SetSkillValue("intimidationValue", "intimidationProf", chaMod, profBonus);
        SetSkillValue("deceptionValue", "deceptionProf", chaMod, profBonus);
        SetSkillValue("persuasionValue", "persuasionProf", chaMod, profBonus);

        SetInt("initiativeField", dexMod);

        SetInt("acField", 10 + dexMod);

        UpdateInventoryWeightLabel();
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

    private void OnSaveClicked()
    {
        if (isViewMode) return;

        root = GetComponent<UIDocument>().rootVisualElement;

        string errors = "";

        string name = GetText("nameField");
        string race = GetDropdown("raceField");
        string charClass = GetDropdown("classField");
        int level = GetInt("levelField");
        int experience = GetInt("experienceField");
        int speed = GetInt("speedField");
        int ac = GetInt("acField");

        if (string.IsNullOrEmpty(name))
            errors += "• Имя обязательно для заполнения\n";
        if (string.IsNullOrEmpty(race))
            errors += "• Раса обязательна для заполнения\n";
        if (string.IsNullOrEmpty(charClass))
            errors += "• Класс обязателен для заполнения\n";
        if (level < 1 || level > 20)
            errors += "• Уровень должен быть от 1 до 20\n";
        if (experience < 0)
            errors += "• Опыт не может быть отрицательным\n";
        if (speed < 0)
            errors += "• Скорость не может быть отрицательной\n";
        if (ac < 0)
            errors += "• КД не может быть отрицательным\n";

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

        foreach (var item in inventoryList)
        {
            if (string.IsNullOrEmpty(item.itemName))
            {
                errors += "• У предмета в инвентаре должно быть название\n";
                break;
            }
        }
        foreach (var item in inventoryList)
        {
            if (item.quantity < 1)
            {
                errors += "• Количество предмета \"" + item.itemName + "\" должно быть не менее 1\n";
            }
            if (item.weight < 0)
            {
                errors += "• Вес предмета \"" + item.itemName + "\" не может быть отрицательным\n";
            }
        }

        var seenItemNames = new HashSet<string>();
        foreach (var item in inventoryList)
        {
            if (string.IsNullOrEmpty(item.itemName)) continue;
            string key = item.itemName.Trim().ToLowerInvariant();
            if (!seenItemNames.Add(key))
                errors += "• Предмет \"" + item.itemName + "\" дублируется в инвентаре\n";
        }

        float totalInventoryWeight = inventoryList.Sum(i => i.weight * i.quantity) + weaponsList.Sum(w => w.weight);
        float carryCapacity = str * 15;
        if (totalInventoryWeight > carryCapacity)
            errors += $"• Общий вес инвентаря и оружия ({totalInventoryWeight:0.##} фунтов) превышает грузоподъёмность ({carryCapacity:0.##} фунтов)\n";

        foreach (var spell in spellsList)
        {
            if (string.IsNullOrEmpty(spell.name))
            {
                errors += "• У заклинания должно быть название\n";
                break;
            }
        }
        foreach (var spell in spellsList)
        {
            if (spell.level < 0 || spell.level > 9)
            {
                errors += "• Уровень заклинания \"" + spell.name + "\" должен быть от 0 до 9\n";
            }
        }

        int maxSpellLevel = MaxSpellLevelForCharacterLevel(level);
        foreach (var spell in spellsList)
        {
            if (spell.level > 0 && spell.level > maxSpellLevel)
            {
                errors += $"• Заклинание \"{spell.name}\" ({spell.level} ур.) недоступно персонажу {level} уровня (максимум {maxSpellLevel} ур.)\n";
            }
        }

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
        character.armorClass = ac;

        character.proficiencyBonus = GetInt("profBonusField");
        character.initiative = GetInt("initiativeField");
        character.speed = speed;

        character.strength = str;
        character.dexterity = dex;
        character.constitution = con;
        character.intelligence = intel;
        character.wisdom = wis;
        character.charisma = cha;

        character.weapons = new List<WeaponItem>(weaponsList);
        character.inventory = new List<InventoryItem>(inventoryList);
        character.spells = new List<Spell>(spellsList);

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
        isViewMode = false;
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
