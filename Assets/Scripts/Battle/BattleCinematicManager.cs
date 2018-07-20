using System.Collections;
using UnityEngine;

public class BattleCinematicManager {
    public enum Type {
        Opening, Ending
    }

    private BattleManager battleManager; // Shortcut for BattleManager.instance

    private string[] actions;
    private Type type;

    public BattleCinematicManager() {
        battleManager = BattleManager.instance;
    }

    // Called by BattleManager
    public void Update() {
    }

    public void EnterBattleStepCinematic(Type type) {
        battleManager.currentBattleStep = BattleManager.BattleStep.Cinematic;

        // Disable all HUD by default
        battleManager.placingHUD.SetActiveWithAnimation(false);
        battleManager.statusHUD.Hide();
        battleManager.fightHUD.SetActiveWithAnimation(false);
        battleManager.victoryHUD.SetActiveWithAnimation(false);

        this.type = type;
        actions = type == Type.Opening ? battleManager.mission.openingCinematic : battleManager.mission.endingCinematic;

        if (actions.Length > 0) {
            battleManager.StartCoroutine(ProcessCinematic());
        } else {
            EndCinematic();
        }
    }

    private IEnumerator ProcessCinematic() {
        foreach (string action in actions) {
            Debug.Log(action);
        }

        EndCinematic();

        yield return null;
    }

    private void EndCinematic() {
        if (type == Type.Opening) {
            battleManager.placing.EnterBattleStepPlacing();
        } else {
            // TODO [ALPHA] End of battle
        }
    }
}
