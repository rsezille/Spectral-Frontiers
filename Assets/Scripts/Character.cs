using UnityEngine;

/**
 * Represent a character, which can be an allie or an enemy
 */
public class Character {
    public const int MAXIMUM_LEVEL = 50;

    public enum Status {
        None, Poisoned, Frozen, Slowed, Silenced, Sleeping
    };

    public string name;

    public string sprite; //TODO: move this to job + type ?

    // Global
    public int level = 1;
    public int experience = 0;
    public Status status = Status.None;

    // Health points
    private int currentHP = 30; // Current health points (can be higher than the baseHP)
    private int baseHP = 30; // Base health points without bonus (equipement...)
    private int maximumHP; // Computed when changing equipement, taking a hit, and so on

    // Magic points
    private int currentMP = 0; // Current magic points (can be higher than the baseMP)
    private int baseMP = 0; // Base magic points without bonus (equipement...)
    private int maximumMP; // Computed when changing equipement, using a spell, and so on

    public int basePhysicalAttack = 10;
    public int basePhysicalDefense = 0;
    public int baseMagicalPower = 10;
    public int baseMagicalResistance = 0;

    public int actionTokens = 1; // Number of times the character can do an action (basic attack/skill/spell/item) per turn
    public int movementTokens = 1; // Number of times the character can move per turn

    public int movementPoints = 3; // Number of squares the character can travel with one movement token

    public BoardCharacter boardCharacter; //TODO: its ugly but I don't care

    public Character(string name) {
        this.name = name;
        sprite = "ss_south"; //TODO

        currentHP = ComputeMaxHP();
    }

    public int ComputeMaxHP() {
        //TODO: Equipment impact
        maximumHP = baseHP;

        return maximumHP;
    }

    public int GetMaxHP() {
        return maximumHP;
    }

    public int GetCurrentHP() {
        return currentHP;
    }

    public void SetCurrentHP(int hp) {
        currentHP = Mathf.Clamp(hp, 0, GetMaxHP());
    }

    public int ComputeMaxMP() {
        //TODO: Equipement impact
        maximumMP = baseMP;

        return maximumMP;
    }

    public int GetMaxMP() {
        return maximumMP;
    }

    public int GetCurrentMP() {
        return currentMP;
    }

    public int GetPhysicalAttack() {
        //TODO: Equipment impact
        return basePhysicalAttack;
    }

    public int GetPhysicalDefense() {
        //TODO: Equipment impact
        return basePhysicalDefense;
    }

    public int GetMagicalPower() {
        //TODO: Equipment impact
        return baseMagicalPower;
    }

    public int GetMagicalResistance() {
        //TODO: Equipment impact
        return baseMagicalResistance;
    }
}
