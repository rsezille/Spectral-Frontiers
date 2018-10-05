using System.Collections.Generic;
using UnityEngine;

namespace SF {
    public class CharacterStat {
        private readonly List<CharacterStatModifier> _statModifiers;

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
            _statModifiers = new List<CharacterStatModifier>();
            this.character = character;
            this.templateStat = templateStat;
            _value = 1;
            isDirty = true;
        }

        public void AddModifier(CharacterStatModifier modifier) {
            _statModifiers.Add(modifier);
            isDirty = true;
        }

        public void RemoveModifiersFromSource(object source) {
            int numRemovals = _statModifiers.RemoveAll(modifier => modifier.source == source);

            if (numRemovals > 0) {
                isDirty = true;
            }
        }

        private int ComputeValue() {
            int baseValue = templateStat.baseValue + (Mathf.Max(0, character.level - 1) * templateStat.perLevelValue);

            return baseValue;
        }
    }
}
