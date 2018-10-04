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
            /*if (item is EquippableItem) {
                foreach (StatModifier sm in ((EquippableItem)item).statModifier) {
                    if (sm.stat == def.stat) def.AddModifier(sm);
                }
            }*/
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
