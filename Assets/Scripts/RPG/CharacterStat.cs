using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace SF {
    public class CharacterStat {
        private readonly List<StatModifier> _statModifiers;
        public readonly ReadOnlyCollection<StatModifier> statModifiers;

        private int _value;
        public int value {
            get {
                if (isDirty) {
                    _value = ComputeValue();
                    isDirty = false;
                }

                return _value;
            }
        }

        private bool isDirty = false;

        public Character character;
        public TemplateStat templateStat;

        public CharacterStat(Character character, TemplateStat templateStat) {
            this.character = character;
            this.templateStat = templateStat;
            _value = 1;
            isDirty = true;
        }

        public void AddModifier(StatModifier modifier) {
            _statModifiers.Add(modifier);
            isDirty = true;
        }

        private int ComputeValue() {
            int baseValue = templateStat.baseValue + (Mathf.Max(0, character.level - 1) * templateStat.perLevelValue);

            return baseValue;
        }
    }
}
