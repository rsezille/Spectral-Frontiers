using DG.Tweening;
using SF;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class BattleCutsceneManager {
    public enum Type {
        Opening, Ending
    }

    private BattleManager battleManager;

    private RawMission.Action[] actions;
    private Type type;
    private List<BoardCharacter> instanciatedCharacters = new List<BoardCharacter>();

    private Coroutine cutsceneCoroutine;

    private bool skipping = false;

    public BattleCutsceneManager(BattleManager battleManager) {
        this.battleManager = battleManager;
    }

    // Called by BattleManager
    public void Update() { }

    public void SkipCutscene() {
        if (skipping || battleManager.battleState.currentBattleStep != BattleState.BattleStep.Cutscene) return;

        skipping = true;

        if (cutsceneCoroutine != null) {
            battleManager.StopCoroutine(cutsceneCoroutine);
        }

        GameManager.instance.DialogBox.Hide();
        EndCutscene();
    }

    // Called by BattleManager
    public void EnterBattleStepCutscene(Type type) {
        this.type = type;
        instanciatedCharacters.Clear();

        if (type == Type.Ending) {
            GameObject transition = new GameObject("CutsceneTransition");
            transition.transform.SetParent(GameManager.instance.transform);
            transition.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            Image transitionImage = transition.AddComponent<Image>();
            transitionImage.color = new Color(Color.black.r, Color.black.g, Color.black.b, 0f);

            transitionImage.DOColor(Color.black, 0.5f).OnComplete(() => {
                foreach (BoardCharacter playerCharacter in battleManager.battleCharacters.player) {
                    playerCharacter.Remove();
                }
                
                transitionImage.DOColor(new Color(Color.black.r, Color.black.g, Color.black.b, 0f), 0.5f).OnComplete(ProcessEnterCutscene);
            });
        } else {
            ProcessEnterCutscene();
        }
    }

    // Called by BattleManager
    public void LeaveBattleStepCutscene() {
        foreach (BoardCharacter character in instanciatedCharacters) {
            character.Remove();
        }

        instanciatedCharacters.Clear();

        battleManager.cutsceneHUD.gameObject.SetActive(false);
    }

    private void ProcessEnterCutscene() {
        skipping = false;
        battleManager.cutsceneHUD.gameObject.SetActive(true);

        actions = type == Type.Opening ? battleManager.missionToLoad.value.openingCutscene : battleManager.missionToLoad.value.endingCutscene;

        if (actions.Length > 0) {
            cutsceneCoroutine = battleManager.StartCoroutine(ProcessCutscene());
        } else {
            EndCutscene();
        }
    }

    private IEnumerator ProcessCutscene() {
        foreach (RawMission.Action action in actions) {
            yield return ProcessAction(action);
        }

        EndCutscene();

        yield return null;
    }

    private void EndCutscene() {
        if (type == Type.Opening) {
            GameObject transition = new GameObject("CutsceneTransition");
            transition.transform.SetParent(GameManager.instance.transform);
            transition.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            Image transitionImage = transition.AddComponent<Image>();
            transitionImage.color = new Color(Color.black.r, Color.black.g, Color.black.b, 0f);

            transitionImage.DOColor(Color.black, 0.5f).OnComplete(() => {
                battleManager.battleState.currentBattleStep = BattleState.BattleStep.Placing;

                transitionImage.DOColor(new Color(Color.black.r, Color.black.g, Color.black.b, 0f), 0.5f).OnComplete(() => {
                    Object.Destroy(transition);
                });
            });
        } else {
            GameManager.instance.LoadSceneAsync(Scenes.InGame);
        }
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

        BoardCharacter boardCharacterPrefab = Resources.Load<BoardCharacter>("CutsceneBoardCharacter");
        boardCharacterPrefab.enemyOrNeutralSpritePrefab = Resources.Load<GameObject>("CharacterSprites/" + sprite);

        float squareHeight = (float)battleManager.board.GetSquare(parsedX, parsedY).Height / Globals.TileHeight;

        BoardCharacter boardCharacter = Object.Instantiate(boardCharacterPrefab, BoardUtil.CoordToWorldPosition(fromX, fromY, battleManager.board.GetSquare(parsedX, parsedY).GetWorldHeight()), Quaternion.identity);
        boardCharacter.character = new Character(name);
        boardCharacter.direction = direction;
        boardCharacter.sprite.color = new Color(boardCharacter.sprite.color.r, boardCharacter.sprite.color.g, boardCharacter.sprite.color.b, 0f);
        boardCharacter.sprite.DOColor(new Color(boardCharacter.sprite.color.r, boardCharacter.sprite.color.g, boardCharacter.sprite.color.b, 1f), 1f);

        if (boardCharacter.shadow != null) {
            boardCharacter.shadow.Show(1f);
        }

        Tween move = boardCharacter.transform.DOMove(BoardUtil.CoordToWorldPosition(battleManager.board.GetSquare(parsedX, parsedY)), 1f).SetEase(Ease.Linear).OnComplete(() => {
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
            yield return instanciatedCharacters[int.Parse(characterIndex)].CutsceneMoveTo(battleManager.board.GetSquare(int.Parse(x), int.Parse(y)), true);
        }

        yield return null;
    }

    private IEnumerator ActionDialogbox(NameValueCollection args) {
        string dialogId = args["id"] ?? Globals.FallbackDialog;
        string target = args["target"] ?? "global";
        string strPosition = args["position"] ?? "bottom";
        string characterIndex = args["charIndex"] ?? "0";
        string characterName = args["charName"] ?? "";
        string strStyle = args["style"] ?? "normal";
        DialogStyle style = EnumUtil.ParseEnum(strStyle, DialogStyle.Normal);

        DialogBox.Position position = strPosition == "bottom" ? DialogBox.Position.Bottom : DialogBox.Position.Top;

        if (target == "global") {
            yield return new WaitForCustom(GameManager.instance.DialogBox.Show(dialogId, position, style, 0, characterName));
        } else if (target == "character") {
            if (int.Parse(characterIndex) < instanciatedCharacters.Count) {
                yield return new WaitForCustom(GameManager.instance.DialogBox.Show(instanciatedCharacters[int.Parse(characterIndex)], dialogId, position, style));
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
