﻿using DG.Tweening;
using SF;
using System.Collections;
using System.Globalization;
using UnityEngine;

public class BattleCinematicManager {
    public enum Type {
        Opening, Ending
    }

    private BattleManager battleManager; // Shortcut for BattleManager.instance

    private string[] actions;
    private Type type;

    private Coroutine cinematicCoroutine;

    public BattleCinematicManager() {
        battleManager = BattleManager.instance;
    }

    // Called by BattleManager
    public void Update() {
        if (InputManager.Special1.IsKeyDown) {
            battleManager.StopCoroutine(cinematicCoroutine);
            GameManager.instance.DialogBox.Hide();
            EndCinematic();
        }
    }

    public void EnterBattleStepCinematic(Type type) {
        if (battleManager.currentBattleStep == BattleManager.BattleStep.Fight) {
            battleManager.statusHUD.Hide();
            battleManager.fightHUD.SetActiveWithAnimation(false);
            battleManager.EventOnLeavingMarkStep();
        }

        battleManager.currentBattleStep = BattleManager.BattleStep.Cinematic;

        this.type = type;
        actions = type == Type.Opening ? battleManager.mission.openingCinematic : battleManager.mission.endingCinematic;

        if (actions.Length > 0) {
            cinematicCoroutine = battleManager.StartCoroutine(ProcessCinematic());
        } else {
            EndCinematic();
        }
    }

    private IEnumerator ProcessCinematic() {
        foreach (string action in actions) {
            string[] substrings = action.Split(':');

            yield return ProcessAction(substrings);
        }

        EndCinematic();

        yield return null;
    }

    private void EndCinematic() {
        if (type == Type.Opening) {
            battleManager.placing.EnterBattleStepPlacing();
        } else {
            battleManager.victory.EnterBattleStepVictory();
        }
    }

    private IEnumerator ProcessAction(string[] splitAction) {
        string key = splitAction[0];
        string option = splitAction[1];
        string value = splitAction[2];

        switch (key) {
            case "wait":
                yield return new WaitForSeconds(float.Parse(value, CultureInfo.InvariantCulture));
                break;
            case "dialogbox":
                string[] options = option.Split(';');
                string scope = options[0];

                if (scope == "global") {
                    string strPosition = options.Length >= 2 ? options[1] : "bottom";
                    DialogBox.Position position = strPosition == "bottom" ? DialogBox.Position.Bottom : DialogBox.Position.Top;

                    yield return new WaitForCustom(GameManager.instance.DialogBox.Show(value, position));
                } else if (scope == "enemy") {
                    string enemyIndex = options[1];
                    string strPosition = options.Length >= 3 ? options[2] : "bottom";
                    DialogBox.Position position = strPosition == "bottom" ? DialogBox.Position.Bottom : DialogBox.Position.Top;

                    yield return new WaitForCustom(GameManager.instance.DialogBox.Show(battleManager.enemyCharacters[int.Parse(enemyIndex)], value, position));
                }
                break;
            case "camera":
                if (option == "square") {
                    string[] squarePosition = value.Split(',');

                    yield return battleManager.battleCamera.SetPosition(int.Parse(squarePosition[0]), int.Parse(squarePosition[1]), true).WaitForCompletion();
                } else if (option == "enemy") {
                    yield return battleManager.battleCamera.SetPosition(battleManager.enemyCharacters[int.Parse(value)], true).WaitForCompletion();
                }
                break;
            default:
                yield return null;
                break;
        }
    }
}