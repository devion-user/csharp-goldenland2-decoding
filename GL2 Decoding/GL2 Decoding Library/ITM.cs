using System;
using System.Globalization;

namespace GL2DecodingLibrary
{
    [Serializable]
    public class ITM
    {
        public ITM(string filename)
        {
            var reader = new Reader(filename);
            if (reader.ReadInt() != 4)
                return;

            id = reader.ReadInt();
            typeID = reader.ReadInt();
            flags = reader.ReadInt();
            maxCount = reader.ReadInt();
            puppetPath = reader.ReadString();
            iconPath = reader.ReadString();
            _unknown1 = reader.ReadInt();
            _unknown2 = reader.ReadInt();
            price = reader.ReadInt();
            weightInteger = reader.ReadInt();
            _unknown3 = reader.ReadInt();
            actionPoints = reader.ReadInt();
            materialID = reader.ReadInt();
            subtypeID = reader.ReadInt();
            shieldArmorLevel = reader.ReadInt();
            _unknown4 = reader.ReadInt();
            materialTextID = reader.ReadInt();

            var hasSpecialString = ((subtypeID >= 40 && typeID != 5) || (typeID == 9) || (typeID == 10));
            specialExecute = hasSpecialString ? reader.ReadString() : "";

            requiredLevel = reader.ReadInt();

            requiredStrength = reader.ReadInt();
            requiredWisdom = reader.ReadInt();
            requiredConstitution = reader.ReadInt();
            requiredIntellect = reader.ReadInt();
            requiredAttention = reader.ReadInt();
            requiredAgility = reader.ReadInt();

            if (subtypeID >= 6) //Оружие
            {
                weaponMinDrobDamage = reader.ReadInt();
                weaponMaxDrobDamage = reader.ReadInt();
                weaponMinRubDamage = reader.ReadInt();
                weaponMaxRubDamage = reader.ReadInt();
                weaponMinKolDamage = reader.ReadInt();
                weaponMaxKolDamage = reader.ReadInt();
                if (type == Type.Staff) //Посох
                {
                    magicID = reader.ReadInt();
                    reader.ReadInt();
                    staffChargesCount = reader.ReadInt();
                }
            }
            else if (typeID >= 8 && typeID <= 10) //Болты, патроны, стрелы
            {
                ammoDamageType = reader.ReadInt();
            }
            else if (subtypeID == 5) //Зелье, еда
            {
                recoveryHealth = reader.ReadInt();
                recoveryEnergy = reader.ReadInt();
            }
            else if (typeID >= 11 && typeID <= 14) //Броня
            {
                armorDrob = reader.ReadInt();
                reader.ReadInt();
                armorRub = reader.ReadInt();
                reader.ReadInt();
                armorKol = reader.ReadInt();
                reader.ReadInt();
            }
            else if (type == Type.Recipe) //Рецепт
            {
                recipeReagentItemID1 = reader.ReadInt();
                recipeReagentItemID2 = reader.ReadInt();
                recipeResultItemID = reader.ReadInt();
            }
            else if (type == Type.Book) //Книга
            {
                magicID = reader.ReadInt();
            }
            else if (type == Type.Scroll) //Свиток
            {
                magicID = reader.ReadInt();
                reader.ReadInt();
            }

            var effectsCount = reader.ReadInt();
            if (effectsCount >= 30)
                return;

            effects = new Effect[effectsCount];
            for (int i = 0; i < effectsCount; i++)
            {
                effects[i] = new Effect
                {
                    id = reader.ReadInt(),
                    value = reader.ReadInt(),
                    flags = reader.ReadInt(),
                    actionTimeMinutes = reader.ReadByte(),
                    actionTimeSeconds = reader.ReadByte(),
                    waitTimeMinutes = reader.ReadByte(),
                    waitTimeSeconds = reader.ReadByte()
                };
                if (i > 0)
                {
                    var curr = effects[i];
                    if (curr.hasActionTime && curr.actionTime == 0)
                    {
                        var prev = effects[i - 1];
                        curr.actionTimeMinutes = prev.actionTimeMinutes;
                        curr.actionTimeSeconds = prev.actionTimeSeconds;
                    }
                }
            }
        }


        public int id;
    
        public int typeID;
        public Type type { get { return (Type) typeID; }}

        public int flags;
        public int maxCount;
        public string puppetPath;
        public string iconPath;
        //?
        public int _unknown1;
        //?
        public int _unknown2;

        public int price;
        public int weightInteger;
        //?
        public int _unknown3;

        public int actionPoints;
        public int materialID;
        public int subtypeID;
        public int shieldArmorLevel;
        //?
        public int _unknown4;

        public int materialTextID = -1;
        public string specialExecute;
        public int requiredLevel;
        public int requiredStrength;
        public int requiredWisdom;
        public int requiredConstitution;
        public int requiredIntellect;
        public int requiredAttention;
        public int requiredAgility;

        public bool isWeapon { get { return subtypeID >= 6; }}
        public bool isAmmo { get { return typeID >= 8 && typeID <= 10; } }
        public bool isRecovery { get { return subtypeID == 5; } }
        public bool isArmor { get { return typeID >= 11 && typeID <= 14; } }
        //damage
        public int weaponMinDrobDamage;
        public int weaponMaxDrobDamage;
        public int weaponMinRubDamage;
        public int weaponMaxRubDamage;
        public int weaponMinKolDamage;
        public int weaponMaxKolDamage;

        //armor
        public int armorDrob;
        public int armorRub;
        public int armorKol;

        public int magicID = -1;
        public int staffChargesCount;
    
        public int ammoDamageType;

        public int recoveryHealth;
        public int recoveryEnergy;

        public int recipeReagentItemID1 = -1;
        public int recipeReagentItemID2 = -1;
        public int recipeResultItemID = -1;

        public Effect[] effects = new Effect[0];

        //after all
        public string magicTitle;
        public string filename;

        public float weight { get { return weightInteger/10f; }}
    
        public bool isTwoHanded { get { return ((flags & (0x1 << 0)) != 0); } }//true - Двуручный предмет
        public bool isBow { get { return ((flags & (0x1 << 4)) != 0); } }//true - Лук или арбалет
        public bool isDestructible { get { return ((flags & (0x1 << 3)) == 0) && ((flags & (0x1 << 7)) != 0); } }//true - Неразрушимая вещь

        public bool wordMany { get { return ((flags & (0x1 << 30)) != 0) && ((flags & (0x1 << 31)) != 0); } }//Множественное число
        public bool wordFeminine { get { return ((flags & (0x1 << 30)) != 0) && ((flags & (0x1 << 31)) == 0); } }//Единственное число, женский род
        public bool wordNeuter { get { return ((flags & (0x1 << 30)) == 0) && ((flags & (0x1 << 31)) != 0); } }//Единственное число, средний род
        public bool wordMasculine { get { return ((flags & (0x1 << 30)) == 0) && ((flags & (0x1 << 31)) == 0); } }//Единственное число, мужской род

        public bool isUnical { get { return ((flags & (0x1 << 5)) != 0); } }//Уникальный
        public bool isCursed { get { return ((flags & (0x1 << 6)) != 0); } }//Проклятый
        public bool isForged { get { return ((flags & (0x1 << 8)) != 0); } }//Кованый

        public string materialText { get { return materialTextID != -1 ? materialTexts[materialTextID] : ""; }}

        private static readonly string[] materialTexts;
        static ITM()
        {
            materialTexts = new string[15];
            materialTexts[0] = "Алмаз";
            materialTexts[1] = "Бронза";
            materialTexts[2] = "Булат";
            materialTexts[3] = "Чёрный металл";
            materialTexts[4] = "Железо";
            materialTexts[5] = "Кровавый металл";
            materialTexts[6] = "Платина";
            materialTexts[7] = "Серебро";
            materialTexts[8] = "Золото";
            materialTexts[9] = "Небесный металл";
            materialTexts[10] = "Органика";
            materialTexts[11] = "Метеорит";
            materialTexts[12] = "Таулин";
            materialTexts[13] = "Камень";
            materialTexts[14] = "Дерево";
        }

        public enum Type
        {
            Sword = 0,
            Axe = 1,
            Spear = 2,
            Bow = 3,
            CrossBow = 4,
            Gun = 5,
            Staff = 6,
            Club = 7,
            Ammo = 8,
            Bolts = 9,
            Arrows = 10,
            Helm = 11,
            Armour = 12,
            Shield = 13,
            Elbarms = 14,
            Bracer = 15,
            Amulet = 16,
            Ring = 17,
            Money = 18,
            Potion = 19,
            Food = 20,
            Book = 21,
            Scroll = 22,
            Material = 23,
            Reagent = 24,
            Quest = 25,
            Recipe = 26,
            Garbage = 27,
            Stone = 28
        }

        [Serializable]
        public class Effect
        {
            public int id;
            public int flags;
            public int value;
            public byte actionTimeMinutes;
            public byte actionTimeSeconds;
            public byte waitTimeMinutes;
            public byte waitTimeSeconds;

            public int actionTime { get { return actionTimeMinutes*60 + actionTimeSeconds; }}
            public int waitTime { get { return waitTimeMinutes*60 + waitTimeSeconds; }}

            //Действует всегда
            public bool isAlwaysTime { get { return ((flags & (0x1 << 0)) != 0); } }
                //Действует только днём
            public bool isDayTime { get { return ((flags & (0x1 << 1)) != 0);} }
            //Действует только ночью
            public bool isNightTime { get { return ((flags & (0x1 << 2)) != 0); } }

            //Коэффициент - целое число
            public bool isInteger { get { return ((flags & (0x1 << 6)) != 0); } }
            //Коэффициент - процент
            public bool isPercent { get { return ((flags & (0x1 << 7)) != 0); } }

            //Имеет ли эффект время действия
            public bool hasActionTime { get { return ((flags & (0x1 << 4)) != 0 || hasWaitTime); } }
            //Имеет ли эффект время ожидания перед действием
            public bool hasWaitTime { get { return ((flags & (0x1 << 5)) != 0); } }

            public override string ToString()
            {
                var percent = isPercent ? "%" : "";
                var stringValue = GetValueString();
                return string.Format(formats[id], stringValue + percent);
            }
    
            string GetValueString()
            {
                var sign = value > 0 ? "+" : "";

                if (InRange(0, 2))
                    return value.ToString(CultureInfo.InvariantCulture);
                if (id == 30)
                    return sign + (value/10f).ToString("#####0.#");
                if (InRange(33, 35))
                    return (value/2) + "-" + value;
                if (InRange(36, 41))
                    return (value/2) + "-" + ((value/2 + 1)*2 + value%2);
                return sign + value;
            }

            bool InRange(int minIndex, int maxIndex)
            {
                return id >= minIndex && id <= maxIndex;
            }

            private static readonly string[] formats;
            static Effect()
            {
                formats = new string[58];
                formats[0] = "Повреждение ядом 1-{0}";
                formats[1] = "Повреждение холодом 1-{0}";
                formats[2] = "Повреждение огнем 1-{0}";
                formats[3] = "Сопротивляемость яду {0}";
                formats[4] = "Сопротивляемость холоду {0}";
                formats[5] = "Сопротивляемость огню {0}";
                formats[6] = "Сила {0}";
                formats[7] = "Телосложение {0}";
                formats[8] = "Внимание {0}";
                formats[9] = "Ловкость {0}";
                formats[10] = "Интеллект {0}";
                formats[11] = "Мудрость {0}";
                formats[12] = "Удача {0}";
                formats[13] = "Сопротивляемость магии Богов {0}";
                formats[14] = "Сопротивляемость магии Стихий {0}";
                formats[15] = "Сопротивляемость магии Света {0}";
                formats[16] = "Cопротивляемость магии Тьмы {0}";
                formats[17] = "Сопротивляемость магии Теней {0}";
                formats[18] = "Сопротивляемость магии Природы {0}";
                formats[19] = "Сопротивляемость руб. повреждениям {0}";
                formats[20] = "Сопротивляемость дроб. повреждениям {0}";
                formats[21] = "Сопротивляемость кол. повреждениям {0}";
                formats[22] = "Максимальное количество здоровья {0}";
                formats[23] = "Максимальное количество энергии {0}";
                formats[24] = "Инициатива {0}";
                formats[25] = "Скорость восстановления здоровья {0}";
                formats[26] = "Скорость восстановления энергии {0}";
                formats[27] = "Очки действия {0}";
                formats[28] = "Слава {0}";
                formats[29] = "Класс брони {0}";
                formats[30] = "Переносимый вес {0}";
                formats[31] = "Воровство жизни {0}";
                formats[32] = "Воровство энергии {0}";
                formats[33] = "Руб. повреждения {0}";
                formats[34] = "Дроб. повреждения {0}";
                formats[35] = "Кол. повреждения {0}";
                formats[36] = "Повреждения магией Богов {0}";
                formats[37] = "Повреждения магией Стихий {0}";
                formats[38] = "Повреждения магией Света {0}";
                formats[39] = "Повреждения магией Тьмы {0}";
                formats[40] = "Повреждения магией Теней {0}";
                formats[41] = "Повреждения магией Природы {0}";
                formats[42] = "Иммунитет к магии Богов {0}";
                formats[43] = "Иммунитет к магии Стихий {0}";
                formats[44] = "Иммунитет к магии Света {0}";
                formats[45] = "Иммунитет к магии Тьмы {0}";
                formats[46] = "Иммунитет к магии Теней {0}";
                formats[47] = "Иммунитет к магии Природы {0}";

                formats[48] = "";
                formats[49] = "";
                formats[50] = "Точность {0}";
                formats[51] = "Шанс на критический удар {0}";
                formats[52] = "Повреждения при критическом ударе {0}";
                formats[53] = "Шанс на критический промах {0}";
                formats[54] = "Разговорчивость {0}";
                formats[55] = "Осторожность {0}";
                formats[56] = "Меткость выстрела {0}";
                formats[57] = "Стрелковые повреждения {0}";
            }
        }
    }
}