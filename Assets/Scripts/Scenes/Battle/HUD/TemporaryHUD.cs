using SF;
using UnityEngine;
using UnityEngine.UI;

public class TemporaryHUD : MonoBehaviour {
    private BattleManager battleManager;

    [Header("Dependencies")]
    public BattleState battleState;

    [Header("References")]
    public Text text;

    private void Awake() {
        battleManager = BattleManager.instance;
    }

    private void Update() {
        text.text = "Version: " + Application.version + "\n";
        text.text += "-------------------\n";
        text.text += "Battle step: " + battleManager.currentBattleStep + "\n";

        if (battleManager.currentBattleStep == BattleManager.BattleStep.Fight) {
            text.text += "Fight step: " + battleState.currentTurnStep + "\n";
            text.text += "Turn: " + battleManager.turn + "\n";
        } else if (battleManager.currentBattleStep == BattleManager.BattleStep.Placing) {
            text.text += "Placed characters: " + battleManager.playerCharacters.Count + "/" + battleManager.mission.maxPlayerCharacters + "\n";
        }
    }
}
