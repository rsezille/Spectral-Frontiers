using SF;
using TMPro;
using UnityEngine;

public class TurnHUD : MonoBehaviour {
    [Header("Direct references")]
    public TextMeshProUGUI text;

    private void Start() {
        Check();
    }

    public void Check() {
        if (BattleManager.instance.currentTurnStep == BattleManager.TurnStep.Enemy) {
            text.SetText(LanguageManager.instance.GetString("battle.hud.turn.enemy"));
        } else {
            text.SetText(LanguageManager.instance.GetString("battle.hud.turn.your"));
        }
    }
}
