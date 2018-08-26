using SF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFightManager {
    private BattleManager battleManager; // Shortcut for BattleManager.instance

    private GameObject arrows;

    private BoardCharacter _selectedPlayerCharacter;
    public BoardCharacter selectedPlayerCharacter {
        get {
            return _selectedPlayerCharacter;
        }

        set {
            if (_selectedPlayerCharacter != null && _selectedPlayerCharacter.glow != null) {
                _selectedPlayerCharacter.glow.Disable();
            }

            _selectedPlayerCharacter = value;
            battleManager.fightHUD.UpdateSelectedSquare();

            if (_selectedPlayerCharacter != null) {
                if (_selectedPlayerCharacter.glow != null) {
                    _selectedPlayerCharacter.glow.Enable();
                }

                battleManager.battleCamera.SetPosition(_selectedPlayerCharacter, true);
                battleManager.fightHUD.Refresh();
            }
        }
    }

    public BattleFightManager() {
        battleManager = BattleManager.instance;
    }

    // Called by BattleManager
    public void Update() {
        if (InputManager.Previous.IsKeyDown) {
            switch (battleManager.currentTurnStep) {
                case BattleManager.TurnStep.Status:
                    Character characterToShow = battleManager.statusHUD.character;

                    if (GameManager.instance.player.characters.IndexOf(characterToShow) == 0) {
                        characterToShow = GameManager.instance.player.characters[GameManager.instance.player.characters.Count - 1];
                    } else {
                        characterToShow = GameManager.instance.player.characters[GameManager.instance.player.characters.IndexOf(characterToShow) - 1];
                    }

                    if (characterToShow.boardCharacter != null) {
                        selectedPlayerCharacter = characterToShow.boardCharacter;
                    }

                    battleManager.statusHUD.Show(characterToShow);
                    break;
                case BattleManager.TurnStep.None:
                    Previous();
                    break;
            }
        } else if (InputManager.Next.IsKeyDown) {
            switch (battleManager.currentTurnStep) {
                case BattleManager.TurnStep.Status:
                    Character characterToShow = battleManager.statusHUD.character;

                    if (GameManager.instance.player.characters.IndexOf(characterToShow) >= GameManager.instance.player.characters.Count - 1) {
                        characterToShow = GameManager.instance.player.characters[0];
                    } else {
                        characterToShow = GameManager.instance.player.characters[GameManager.instance.player.characters.IndexOf(characterToShow) + 1];
                    }

                    if (characterToShow.boardCharacter != null) {
                        selectedPlayerCharacter = characterToShow.boardCharacter;
                    }

                    battleManager.statusHUD.Show(characterToShow);
                    break;
                case BattleManager.TurnStep.None:
                    Next();
                    break;
            }
        }

        if (battleManager.currentTurnStep == BattleManager.TurnStep.Direction) {
            if (InputManager.Up.IsKeyDown) {
                selectedPlayerCharacter.direction = BoardCharacter.Direction.North;
            } else if (InputManager.Down.IsKeyDown) {
                selectedPlayerCharacter.direction = BoardCharacter.Direction.South;
            } else if (InputManager.Left.IsKeyDown) {
                selectedPlayerCharacter.direction = BoardCharacter.Direction.West;
            } else if (InputManager.Right.IsKeyDown) {
                selectedPlayerCharacter.direction = BoardCharacter.Direction.East;
            } else if (InputManager.Confirm.IsKeyDown) {
                EndTurnStepDirection();
            }
        }
    }

    // Called by BattleManager
    public void EnterBattleStepFight() {
        battleManager.fightHUD.SetActiveWithAnimation(true);
        battleManager.turnHUD.gameObject.SetActive(true);
        NewPlayerTurn();
    }

    // Called by BattleManager
    public void LeaveBattleStepFight() {
        if (selectedPlayerCharacter.glow != null) {
            selectedPlayerCharacter.glow.Disable();
        }

        battleManager.EventOnLeavingMarkStep();
        battleManager.statusHUD.Hide();
        battleManager.turnHUD.gameObject.SetActive(false);
        battleManager.fightHUD.SetActiveWithAnimation(false);
    }

    public void EndTurnStepDirection() {
        if (arrows != null) {
            Object.Destroy(arrows);
            arrows = null;
        }

        battleManager.fightHUD.SetActiveWithAnimation(true);
        battleManager.EnterTurnStepNone();
    }

    // Called by BattleManager
    public void EnterTurnStepNone(BattleManager.TurnStep previousTurnStep) {
        switch (previousTurnStep) {
            case BattleManager.TurnStep.Move:
            case BattleManager.TurnStep.Attack:
                battleManager.EventOnLeavingMarkStep();
                break;
            case BattleManager.TurnStep.Status:
                battleManager.fightHUD.SetActiveWithAnimation(true);
                break;
        }

        battleManager.fightHUD.Refresh();

        //battleManager.battleCamera.SetPosition(battleManager.GetSelectedPlayerBoardCharacter(), true);
    }

    // Called by BattleManager
    public void EnterTurnStepStatus(BattleManager.TurnStep previousTurnStep) {
        switch (previousTurnStep) {
            case BattleManager.TurnStep.Move:
            case BattleManager.TurnStep.Attack:
                battleManager.EventOnLeavingMarkStep();
                break;
        }

        battleManager.fightHUD.SetActiveWithAnimation(false);

        battleManager.statusHUD.Show(selectedPlayerCharacter);
    }

    // Called by FightHUD
    public void Move() {
        if (battleManager.currentTurnStep == BattleManager.TurnStep.Move) {
            battleManager.EnterTurnStepNone();
        } else {
            battleManager.fightHUD.actionMenu.SetActiveWithAnimation(false);

            if (selectedPlayerCharacter.movable != null && selectedPlayerCharacter.movable.CanMove()) {
                EnterTurnStepMove();
            }
        }
    }

    // Called by FightHUD
    public void Previous() {
        battleManager.EnterTurnStepNone();
        battleManager.fightHUD.actionMenu.SetActiveWithAnimation(false);

        SelectPreviousPlayerBoardCharacter();
    }

    // Called by FightHUD
    public void Next() {
        battleManager.EnterTurnStepNone();
        battleManager.fightHUD.actionMenu.SetActiveWithAnimation(false);

        SelectNextPlayerBoardCharacter();
    }

    // Called by FightHUD
    public void Status() {
        battleManager.EnterTurnStepStatus();
    }

    // Called by FightHUD
    public void Direction() {
        EnterTurnStepDirection();
    }

    // Called by FightHUD
    public void EndTurn() {
        battleManager.fightHUD.actionMenu.SetActiveWithAnimation(false);
        // TODO [ALPHA] FlashMessage
        // TODO [ALPHA] Disable inputs

        if (selectedPlayerCharacter.glow != null) {
            selectedPlayerCharacter.glow.Disable();
        }

        EnterTurnStepEnemy();
    }

    // Called by FightHUD
    public void Action() {
        battleManager.EnterTurnStepNone();

        if (selectedPlayerCharacter.actionable.CanDoAction()) {
            battleManager.fightHUD.actionMenu.Toggle();
        }
    }

    // Called by ActionMenu
    public void Attack() {
        if (battleManager.currentTurnStep == BattleManager.TurnStep.Attack) {
            battleManager.EnterTurnStepNone();
        } else {
            battleManager.fightHUD.actionMenu.SetActiveWithAnimation(false);

            if (selectedPlayerCharacter.actionable != null && selectedPlayerCharacter.actionable.CanDoAction()) {
                EnterTurnStepAttack();
            }
        }
    }

    private void NewPlayerTurn() {
        if (battleManager.CheckEndBattle()) {
            return;
        }

        battleManager.sunLight.NewTurn();
        battleManager.turn++;

        foreach (BoardCharacter bc in battleManager.playerCharacters) {
            bc.NewTurn();
        }

        battleManager.EnterTurnStepNone();

        selectedPlayerCharacter = battleManager.playerCharacters[0];
    }

    private void EnterTurnStepEnemy() {
        switch (battleManager.currentTurnStep) {
            case BattleManager.TurnStep.Move:
            case BattleManager.TurnStep.Attack:
                battleManager.EventOnLeavingMarkStep();
                break;
        }

        battleManager.currentTurnStep = BattleManager.TurnStep.Enemy;

        foreach (BoardCharacter enemy in battleManager.enemyCharacters) {
            enemy.NewTurn();
        }

        battleManager.StartCoroutine(StartAI());
    }

    /**
     * Process each enemy AI then start a new player turn
     */
    private IEnumerator StartAI() {
        battleManager.fightHUD.SetActiveWithAnimation(false);

        foreach (BoardCharacter enemy in battleManager.enemyCharacters) {
            if (enemy.AI != null) {
                yield return enemy.AI.Process();
            }

            yield return new WaitForSeconds(0.5f);
        }

        battleManager.fightHUD.SetActiveWithAnimation(true);
        NewPlayerTurn();

        yield return null;
    }

    private void MarkSquares(int distance, Square.MarkType markType, bool ignoreBlocking = false) {
        battleManager.EventOnLeavingMarkStep();

        List<Square> squaresHit = battleManager.board.PropagateLinear(selectedPlayerCharacter.GetSquare(), distance, selectedPlayerCharacter.side.value, ignoreBlocking);

        foreach (Square squareHit in squaresHit) {
            if (squareHit.IsNotBlocking() || ignoreBlocking) {
                squareHit.markType = markType;
            }
        }
    }

    // Mark all squares where the character can move
    private void EnterTurnStepMove() {
        battleManager.currentTurnStep = BattleManager.TurnStep.Move;

        battleManager.fightHUD.actionMenu.SetActiveWithAnimation(false);

        MarkSquares(selectedPlayerCharacter.movable.movementPoints, Square.MarkType.Movement);
    }

    // Mark all squares the character can attack
    private void EnterTurnStepAttack() {
        battleManager.currentTurnStep = BattleManager.TurnStep.Attack;

        MarkSquares(1, Square.MarkType.Attack, true); // TODO [ALPHA] weapon range
    }

    public void EnterTurnStepDirection() {
        battleManager.currentTurnStep = BattleManager.TurnStep.Direction;
        battleManager.fightHUD.SetActiveWithAnimation(false);

        GameObject arrowsPrefab = Resources.Load<GameObject>("Arrows");

        arrows = Object.Instantiate(arrowsPrefab, selectedPlayerCharacter.transform.position + new Vector3(arrowsPrefab.transform.position.x, arrowsPrefab.transform.position.y), Quaternion.identity);
    }

    private void SelectPreviousPlayerBoardCharacter() {
        if (battleManager.playerCharacters.IndexOf(selectedPlayerCharacter) == 0) {
            selectedPlayerCharacter = battleManager.playerCharacters[battleManager.playerCharacters.Count - 1];
        } else {
            selectedPlayerCharacter = battleManager.playerCharacters[battleManager.playerCharacters.IndexOf(selectedPlayerCharacter) - 1];
        }
    }

    private void SelectNextPlayerBoardCharacter() {
        if (battleManager.playerCharacters.IndexOf(selectedPlayerCharacter) >= battleManager.playerCharacters.Count - 1) {
            selectedPlayerCharacter = battleManager.playerCharacters[0];
        } else {
            selectedPlayerCharacter = battleManager.playerCharacters[battleManager.playerCharacters.IndexOf(selectedPlayerCharacter) + 1];
        }
    }
}
