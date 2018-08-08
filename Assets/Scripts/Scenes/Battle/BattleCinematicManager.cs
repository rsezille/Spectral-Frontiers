﻿using DG.Tweening;
using SF;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class BattleCinematicManager {
    public enum Type {
        Opening, Ending
    }

    private BattleManager battleManager; // Shortcut for BattleManager.instance

    private RawMission.Action[] actions;
    private Type type;
    private List<BoardCharacter> instanciatedCharacters = new List<BoardCharacter>();

    private Coroutine cinematicCoroutine;

    private bool skipping = false;

    public BattleCinematicManager() {
        battleManager = BattleManager.instance;
    }

    // Called by BattleManager
    public void Update() {
        if (InputManager.Special1.IsKeyDown) {
            SkipCinematic();
        }
    }

    public void SkipCinematic() {
        if (skipping || battleManager.currentBattleStep != BattleManager.BattleStep.Cinematic) return;

        skipping = true;

        if (cinematicCoroutine != null) {
            battleManager.StopCoroutine(cinematicCoroutine);
        }

        GameManager.instance.DialogBox.Hide();
        EndCinematic();
    }

    public void EnterBattleStepCinematic(Type type) {
        if (battleManager.currentBattleStep == BattleManager.BattleStep.Fight) {
            battleManager.statusHUD.Hide();
            battleManager.fightHUD.SetActiveWithAnimation(false);
            battleManager.EventOnLeavingMarkStep();
        }

        skipping = false;
        battleManager.cinematicHUD.gameObject.SetActive(true);

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
        foreach (RawMission.Action action in actions) {
            yield return ProcessAction(action);
        }

        EndCinematic();

        yield return null;
    }

    private void EndCinematic() {
        GameObject transition = new GameObject("CinematicTransition");
        transition.transform.SetParent(GameManager.instance.transform);
        transition.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        Image transitionImage = transition.AddComponent<Image>();
        transitionImage.color = new Color(Color.black.r, Color.black.g, Color.black.b, 0f);

        transitionImage.DOColor(Color.black, 0.5f).OnComplete(() => {
            foreach (BoardCharacter character in instanciatedCharacters) {
                character.Remove();
            }

            battleManager.cinematicHUD.gameObject.SetActive(false);

            if (type == Type.Opening) {
                battleManager.placing.EnterBattleStepPlacing();
            } else {
                battleManager.victory.EnterBattleStepVictory();
            }

            transitionImage.DOColor(new Color(Color.black.r, Color.black.g, Color.black.b, 0f), 0.5f).OnComplete(() => {
                Object.Destroy(transition);
            });
        });
    }

    private IEnumerator ProcessAction(RawMission.Action action) {
        NameValueCollection args = new NameValueCollection();

        foreach (string arg in action.args) {
            string[] keyValue = arg.Split(':');

            args.Add(keyValue[0], keyValue[1]);
        }

        switch (action.type) {
            case "wait":
                yield return ActionWait(args);
                break;
            case "show":
                yield return ActionShow(args);
                break;
            case "move":
                yield return ActionMove(args);
                break;
            case "dialogbox":
                yield return ActionDialogbox(args);
                break;
            case "camera":
                yield return ActionCamera(args);
                break;
            case "direction":
                yield return ActionDirection(args);
                break;
            default:
                yield return null;
                break;
        }
    }

    private IEnumerator ActionWait(NameValueCollection args) {
        float time = args["time"] != null ? float.Parse(args["time"], CultureInfo.InvariantCulture) : 1f;

        yield return new WaitForSeconds(time);
    }

    private IEnumerator ActionShow(NameValueCollection args) {
        string sprite = args["sprite"] ?? Globals.FallbackSpritePrefab;
        string name = args["name"] ?? "";
        string x = args["x"] ?? "0";
        string y = args["y"] ?? "0";
        string from = args["from"] ?? "south";
        string strDirection = args["direction"] ?? "east";
        BoardCharacter.Direction direction = EnumUtil.ParseEnum(strDirection, BoardCharacter.Direction.East);

        int parsedX = int.Parse(x);
        int parsedY = int.Parse(y);

        int fromX = parsedX;
        int fromY = parsedY;

        switch (from) {
            case "north":
                fromX++;
                break;
            case "east":
                fromY--;
                break;
            case "west":
                fromY++;
                break;
            case "south":
                fromX--;
                break;
            default:
                break;
        }

        BoardCharacter boardCharacterPrefab = Resources.Load<BoardCharacter>("CinematicBoardCharacter");
        boardCharacterPrefab.enemyOrNeutralSpritePrefab = Resources.Load<GameObject>("CharacterSprites/" + sprite);

        BoardCharacter boardCharacter = Object.Instantiate(boardCharacterPrefab, BoardUtil.CoordToWorldPosition(fromX, fromY), Quaternion.identity);
        boardCharacter.character = new Character(name);
        boardCharacter.direction = direction;
        boardCharacter.sprite.color = new Color(boardCharacter.sprite.color.r, boardCharacter.sprite.color.g, boardCharacter.sprite.color.b, 0);
        boardCharacter.sprite.DOColor(new Color(boardCharacter.sprite.color.r, boardCharacter.sprite.color.g, boardCharacter.sprite.color.b, 1), 1f);

        Tween move = boardCharacter.transform.DOMove(BoardUtil.CoordToWorldPosition(parsedX, parsedY), 1f).SetEase(Ease.Linear).OnComplete(() => {
            boardCharacter.SetSquare(battleManager.board.GetSquare(parsedX, parsedY));
        });

        instanciatedCharacters.Add(boardCharacter);

        yield return move.WaitForCompletion();
    }

    private IEnumerator ActionMove(NameValueCollection args) {
        string characterIndex = args["charIndex"] ?? "0";
        string x = args["x"] ?? "0";
        string y = args["y"] ?? "0";

        if (int.Parse(characterIndex) < instanciatedCharacters.Count) {
            yield return instanciatedCharacters[int.Parse(characterIndex)].CineMoveTo(battleManager.board.GetSquare(int.Parse(x), int.Parse(y)), true);
        }

        yield return null;
    }

    private IEnumerator ActionDialogbox(NameValueCollection args) {
        string dialogId = args["id"] ?? Globals.FallbackDialog;
        string target = args["target"] ?? "global";
        string strPosition = args["position"] ?? "bottom";
        string characterIndex = args["charIndex"] ?? "0";
        string characterName = args["charName"] ?? "";

        DialogBox.Position position = strPosition == "bottom" ? DialogBox.Position.Bottom : DialogBox.Position.Top;

        if (target == "global") {
            yield return new WaitForCustom(GameManager.instance.DialogBox.Show(dialogId, position, 0, characterName));
        } else if (target == "character") {
            if (int.Parse(characterIndex) < instanciatedCharacters.Count) {
                yield return new WaitForCustom(GameManager.instance.DialogBox.Show(instanciatedCharacters[int.Parse(characterIndex)], dialogId, position));
            }
        }

        yield return null;
    }

    private IEnumerator ActionCamera(NameValueCollection args) {
        string target = args["target"] ?? "square";
        string x = args["x"] ?? "0";
        string y = args["y"] ?? "0";
        string characterIndex = args["charIndex"] ?? "0";

        if (target == "square") {
            yield return battleManager.battleCamera.SetPosition(int.Parse(x), int.Parse(y), true).WaitForCompletion();
        } else if (target == "character") {
            if (int.Parse(characterIndex) < instanciatedCharacters.Count) {
                yield return battleManager.battleCamera.SetPosition(instanciatedCharacters[int.Parse(characterIndex)], true).WaitForCompletion();
            }
        }

        yield return null;
    }

    private IEnumerator ActionDirection(NameValueCollection args) {
        string characterIndex = args["charIndex"] ?? "0";
        string strDirection = args["direction"] ?? "east";
        BoardCharacter.Direction direction = EnumUtil.ParseEnum(strDirection, BoardCharacter.Direction.East);

        if (int.Parse(characterIndex) < instanciatedCharacters.Count) {
            instanciatedCharacters[int.Parse(characterIndex)].direction = direction;
        }

        yield return null;
    }
}
