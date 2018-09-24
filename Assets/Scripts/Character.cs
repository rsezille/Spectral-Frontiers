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
                if (String.IsNullOrEmpty(overloadedName)) {
                    return template.characterName;
                }

                return overloadedName;
            }
        }

        public int maxHP = 50; // TODO
        public int currentHp = 30; // TODO
        public int atk = 10; // TODO
        public int spd = 100; // TODO

        public Character(Mission.Enemy enemy) {
            template = enemy.character;
            level = enemy.level;
            overloadedName = enemy.overloadedName;
        }

        public Character(CharacterTemplate template, int level = 1) {
            this.template = template;
            this.level = level;
        }

        /**
         * Do a basic attack against the defender
         * @param defender The defender
         * @return Return the number of damages done
         */
        public int BasicAttack(Character defender) {
            //int damagesDone = atk.currentValue - defender.def.currentValue;
            int damagesDone = atk;

            defender.currentHp = defender.currentHp - damagesDone;

            return damagesDone;
        }
    }
}
