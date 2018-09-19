using SF;
using TMPro;
using UnityEngine;

public class TurnHUD : MonoBehaviour {
    [Header("Dependencies")]
    public BattleState battleState;

    [Header("References")]
    public TextMeshProUGUI text;

    private void Start() {
        Check();
    }

    public void Check() {
        if (battleState.currentTurnStep == BattleState.TurnStep.Enemy) {
            text.SetText(LanguageManager.instance.GetString("battle.hud.turn.enemy"));
        } else {
            text.SetText(LanguageManager.instance.GetString("battle.hud.turn.your"));
        }
    }
}
