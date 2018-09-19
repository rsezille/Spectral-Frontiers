using SF;
using UnityEngine;
using UnityEngine.UI;

public class TemporaryHUD : MonoBehaviour {
    private BattleManager battleManager;

    [Header("Dependencies")]
    public BattleState battleState;
    public BattleCharacters battleCharacters;

    [Header("References")]
    public Text text;

    private void Awake() {
        battleManager = BattleManager.instance;
    }

    private void Update() {
        text.text = "Version: " + Application.version + "\n";
        text.text += "-------------------\n";
        text.text += "Battle step: " + battleState.currentBattleStep + "\n";

        if (battleState.currentBattleStep == BattleState.BattleStep.Fight) {
            text.text += "Fight step: " + battleState.currentTurnStep + "\n";
            text.text += "Turn: " + battleManager.turn + "\n";
        } else if (battleState.currentBattleStep == BattleState.BattleStep.Placing) {
            text.text += "Placed characters: " + battleCharacters.player.Count + "/" + battleManager.mission.maxPlayerCharacters + "\n";
        }
    }
}
