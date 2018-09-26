using SF;
using UnityEngine;
using UnityEngine.UI;

public class TemporaryHUD : MonoBehaviour {
    [Header("Dependencies")]
    public BattleState battleState;
    public BattleCharacters battleCharacters;
    public MissionVariable missionToLoad;

    [Header("References")]
    public Text text;

    private void Update() {
        text.text = "Version: " + Application.version + "\n";
        text.text += "-------------------\n";
        text.text += "Battle step: " + battleState.currentBattleStep + "\n";

        if (battleState.currentBattleStep == BattleState.BattleStep.Fight) {
            text.text += "Fight step: " + battleState.currentTurnStep + "\n";
        } else if (battleState.currentBattleStep == BattleState.BattleStep.Placing) {
            text.text += "Placed characters: " + battleCharacters.player.Count + "/" + missionToLoad.value.maxPlayerCharacters + "\n";
        }

        battleCharacters.player.ForEach(boardCharacter => {
            text.text += boardCharacter.character.characterName + "   : " + boardCharacter.tick + "\n";
        });

        battleCharacters.enemy.ForEach(boardCharacter => {
            text.text += boardCharacter.character.characterName + "   : " + boardCharacter.tick + "\n";
        });
    }
}
