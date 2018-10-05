using System;
using UnityEngine;

namespace SF {
    [Serializable]
    public class Character {
        public CharacterTemplate template;
        public int level = 1;

        [HideInInspector]
        public BoardCharacter boardCharacter;

        [SerializeField]
        private string overloadedName;
        public string characterName {
            get {
                if (string.IsNullOrEmpty(overloadedName)) {
                    return template.characterName;
                }

                return overloadedName;
            }
        }

        public CharacterStat maxHP;
        public CharacterStat maxMP;
        public CharacterStat atk;
        public CharacterStat def;
        public CharacterStat mag;
        public CharacterStat eva;
        public CharacterStat acc;
        public CharacterStat spd;
        public CharacterStat fireRes;
        public CharacterStat waterRes;
        public CharacterStat windRes;

        public Equipment equipment;

        /*
         * Those two variables could be in BoardCharacter as they are related to battles, but putting them here is more future-proof if we
         * wanna put a mode without healing between battles for example.
         */
        public int currentHP;
        public int currentMP;

        public Item helmet;

        public Character(CharacterTemplate template, int level = 1) {
            this.template = template;
            this.level = level;

            maxHP = new CharacterStat(this, template.maxHP);
            maxMP = new CharacterStat(this, template.maxMP);
            atk = new CharacterStat(this, template.atk);
            def = new CharacterStat(this, template.def);
            mag = new CharacterStat(this, template.mag);
            eva = new CharacterStat(this, template.eva);
            acc = new CharacterStat(this, template.acc);
            spd = new CharacterStat(this, template.spd);

            currentHP = maxHP.value;
            currentMP = 0;
        }

        public Character(Mission.Enemy enemy): this(enemy.character, enemy.level) {
            overloadedName = enemy.overloadedName;
        }

        public void Equip(Item item) {
            if (item is EquippableItem) {
                switch (((EquippableItem)item).equipmentType.slot) {
                    case EquipmentSlot.Head:
                        if (equipment.head != null) {
                            maxHP.RemoveModifiersFromSource(equipment.head);
                            maxMP.RemoveModifiersFromSource(equipment.head);
                            atk.RemoveModifiersFromSource(equipment.head);
                            def.RemoveModifiersFromSource(equipment.head);
                            mag.RemoveModifiersFromSource(equipment.head);
                            eva.RemoveModifiersFromSource(equipment.head);
                            acc.RemoveModifiersFromSource(equipment.head);
                            spd.RemoveModifiersFromSource(equipment.head);
                            fireRes.RemoveModifiersFromSource(equipment.head);
                            waterRes.RemoveModifiersFromSource(equipment.head);
                            windRes.RemoveModifiersFromSource(equipment.head);
                        }

                        equipment.head = (EquippableItem)item;
                        break;
                }

                foreach (StatModifier sm in ((EquippableItem)item).statModifier) {
                    if (sm.stat == maxHP.templateStat.stat) maxHP.AddModifier(new CharacterStatModifier(sm.value, sm.type, item));
                    else if (sm.stat == maxMP.templateStat.stat) maxMP.AddModifier(new CharacterStatModifier(sm.value, sm.type, item));
                    else if (sm.stat == atk.templateStat.stat) atk.AddModifier(new CharacterStatModifier(sm.value, sm.type, item));
                    else if (sm.stat == def.templateStat.stat) def.AddModifier(new CharacterStatModifier(sm.value, sm.type, item));
                    else if (sm.stat == mag.templateStat.stat) mag.AddModifier(new CharacterStatModifier(sm.value, sm.type, item));
                    else if (sm.stat == eva.templateStat.stat) eva.AddModifier(new CharacterStatModifier(sm.value, sm.type, item));
                    else if (sm.stat == acc.templateStat.stat) acc.AddModifier(new CharacterStatModifier(sm.value, sm.type, item));
                    else if (sm.stat == spd.templateStat.stat) spd.AddModifier(new CharacterStatModifier(sm.value, sm.type, item));
                    else if (sm.stat == fireRes.templateStat.stat) fireRes.AddModifier(new CharacterStatModifier(sm.value, sm.type, item));
                    else if (sm.stat == waterRes.templateStat.stat) waterRes.AddModifier(new CharacterStatModifier(sm.value, sm.type, item));
                    else if (sm.stat == windRes.templateStat.stat) windRes.AddModifier(new CharacterStatModifier(sm.value, sm.type, item));
                }
            }
        }

        /**
         * Do a basic attack against the defender
         * @param defender The defender
         * @return Return the number of damages done
         */
        public int BasicAttack(Character defender) {
            //int damagesDone = atk.currentValue - defender.def.currentValue;
            int damagesDone = atk.value;

            defender.currentHP = defender.currentHP - damagesDone;

            return damagesDone;
        }
    }
}
