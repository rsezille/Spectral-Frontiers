using SF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFightManager {
    private BattleManager battleManager;

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

    public BattleFightManager(BattleManager battleManager) {
        this.battleManager = battleManager;
    }

    // Called by BattleManager
    public void Update() {
        if (InputManager.Previous.IsKeyDown) {
            switch (battleManager.battleState.currentTurnStep) {
                case BattleState.TurnStep.Status:
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
                case BattleState.TurnStep.None:
                    Previous();
                    break;
            }
        } else if (InputManager.Next.IsKeyDown) {
            switch (battleManager.battleState.currentTurnStep) {
                case BattleState.TurnStep.Status:
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
                case BattleState.TurnStep.None:
                    Next();
                    break;
            }
        }

        if (battleManager.battleState.currentTurnStep == BattleState.TurnStep.Direction) {
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

        battleManager.board.RemoveAllMarks();
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
    public void EnterTurnStepNone(BattleState.TurnStep previousTurnStep) {
        switch (previousTurnStep) {
            case BattleState.TurnStep.Move:
            case BattleState.TurnStep.Attack:
                battleManager.board.RemoveAllMarks();
                break;
            case BattleState.TurnStep.Status:
                battleManager.fightHUD.SetActiveWithAnimation(true);
                break;
        }

        battleManager.fightHUD.Refresh();

        //battleManager.battleCamera.SetPosition(battleManager.GetSelectedPlayerBoardCharacter(), true);
    }

    // Called by BattleManager
    public void EnterTurnStepStatus(BattleState.TurnStep previousTurnStep) {
        switch (previousTurnStep) {
            case BattleState.TurnStep.Move:
            case BattleState.TurnStep.Attack:
                battleManager.board.RemoveAllMarks();
                break;
        }

        battleManager.fightHUD.SetActiveWithAnimation(false);

        battleManager.statusHUD.Show(selectedPlayerCharacter);
    }

    // Called by FightHUD
    public void Move() {
        if (battleManager.battleState.currentTurnStep == BattleState.TurnStep.Move) {
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
        if (battleManager.battleState.currentTurnStep == BattleState.TurnStep.Attack) {
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

        //battleManager.sunLight.NewTurn();

        foreach (BoardCharacter bc in battleManager.battleCharacters.player) {
            bc.NewTurn();
        }

        battleManager.EnterTurnStepNone();

        selectedPlayerCharacter = battleManager.battleCharacters.player[0];
    }

    private void EnterTurnStepEnemy() {
        switch (battleManager.battleState.currentTurnStep) {
            case BattleState.TurnStep.Move:
            case BattleState.TurnStep.Attack:
                battleManager.board.RemoveAllMarks();
                break;
        }

        battleManager.battleState.currentTurnStep = BattleState.TurnStep.Enemy;

        foreach (BoardCharacter enemy in battleManager.battleCharacters.enemy) {
            enemy.NewTurn();
        }

        battleManager.StartCoroutine(StartAI());
    }

    /**
     * Process each enemy AI then start a new player turn
     */
    private IEnumerator StartAI() {
        battleManager.fightHUD.SetActiveWithAnimation(false);

        foreach (BoardCharacter enemy in battleManager.battleCharacters.enemy) {
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
        battleManager.board.RemoveAllMarks();

        List<Square> squaresHit = battleManager.board.PropagateLinear(selectedPlayerCharacter.GetSquare(), distance, selectedPlayerCharacter.side.value, ignoreBlocking);

        foreach (Square squareHit in squaresHit) {
            if (squareHit.IsNotBlocking() || ignoreBlocking) {
                squareHit.markType = markType;
            }
        }
    }

    // Mark all squares where the character can move
    private void EnterTurnStepMove() {
        battleManager.battleState.currentTurnStep = BattleState.TurnStep.Move;

        battleManager.fightHUD.actionMenu.SetActiveWithAnimation(false);

        MarkSquares(selectedPlayerCharacter.movable.movementPoints, Square.MarkType.Movement);
    }

    // Mark all squares the character can attack
    private void EnterTurnStepAttack() {
        battleManager.battleState.currentTurnStep = BattleState.TurnStep.Attack;

        MarkSquares(1, Square.MarkType.Attack, true); // TODO [ALPHA] weapon range
    }

    public void EnterTurnStepDirection() {
        battleManager.battleState.currentTurnStep = BattleState.TurnStep.Direction;
        battleManager.fightHUD.SetActiveWithAnimation(false);

        GameObject arrowsPrefab = Resources.Load<GameObject>("Arrows");

        arrows = Object.Instantiate(arrowsPrefab, selectedPlayerCharacter.transform.position + new Vector3(arrowsPrefab.transform.position.x, arrowsPrefab.transform.position.y), Quaternion.identity);
    }

    private void SelectPreviousPlayerBoardCharacter() {
        if (battleManager.battleCharacters.player.IndexOf(selectedPlayerCharacter) == 0) {
            selectedPlayerCharacter = battleManager.battleCharacters.player[battleManager.battleCharacters.player.Count - 1];
        } else {
            selectedPlayerCharacter = battleManager.battleCharacters.player[battleManager.battleCharacters.player.IndexOf(selectedPlayerCharacter) - 1];
        }
    }

    private void SelectNextPlayerBoardCharacter() {
        if (battleManager.battleCharacters.player.IndexOf(selectedPlayerCharacter) >= battleManager.battleCharacters.player.Count - 1) {
            selectedPlayerCharacter = battleManager.battleCharacters.player[0];
        } else {
            selectedPlayerCharacter = battleManager.battleCharacters.player[battleManager.battleCharacters.player.IndexOf(selectedPlayerCharacter) + 1];
        }
    }
}
