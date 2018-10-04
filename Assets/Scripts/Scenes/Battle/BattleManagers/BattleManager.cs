using SF;
using System.Collections;
using UnityEngine;

/**
 * Main Battle manager
 * Instantiate one manager for each battle step (BattlePlacingManager, BattleFightManager, BattleVictoryManager)
 * and dispatch events and tasks to them
 */
public class BattleManager : MonoBehaviour {
    [Header("Dependencies")]
    public BattleState battleState;
    public BattleCharacters battleCharacters;
    public MissionVariable missionToLoad;
    public Board board;
    public CharacterVariable currentPartyCharacter;
    public Party party;
    public BoardCharacterVariable currentFightBoardCharacter;
    public CameraPosition mainCameraPosition;
    public IntVariable turnSpeed;

    [Header("Events")]
    public GameEvent screenChange;
    public GameEvent newCharacterTurn;
    public GameEvent loadMission;

    // Dedicated managers for each BattleStep
    public BattleCutsceneManager cutscene;
    public BattlePlacingManager placing;
    public BattleFightManager fight;
    public BattleVictoryManager victory;

    private bool goingGameOver = false;

    #if UNITY_EDITOR
    private Vector2Int previousScreenResolution;
    #endif

    // Initialization
    private void Awake() {
        placing = new BattlePlacingManager(this);
        fight = new BattleFightManager(this);
        victory = new BattleVictoryManager(this);
        cutscene = new BattleCutsceneManager(this);

#if UNITY_EDITOR
        previousScreenResolution = new Vector2Int(Screen.width, Screen.height);
#endif

        battleState.ResetData();
        battleCharacters.ResetData();
        currentPartyCharacter.value = null;
        currentFightBoardCharacter.value = null;
    }

    private void Start() {
        LoadMission();

        mainCameraPosition.SetPosition(board.GetSquare(board.width / 2, board.height / 2));

        battleState.currentBattleStep = BattleState.BattleStep.Cutscene;

        Time.timeScale = PlayerOptions.GetFloat(PlayerOptions.BattleSpeed);
    }

    // Update is called once per frame
    private void Update() {
        #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.M)) {
                GameManager.instance.DialogBox.Show("prologue_01");
            }

            if (Input.GetKeyDown(KeyCode.L)) {
                GameManager.instance.DialogBox.Show(battleCharacters.player[0], "prologue_01");
            }
        #endif
        
        if (InputManager.Special1.IsKeyDown) {
            SkipCutscene();
        }

        switch (battleState.currentBattleStep) {
            case BattleState.BattleStep.Cutscene:
                cutscene.Update();
                break;
            case BattleState.BattleStep.Placing:
                placing.Update();
                break;
            case BattleState.BattleStep.Fight:
                fight.Update();
                break;
            case BattleState.BattleStep.Victory:
                victory.Update();
                break;
        }

#if UNITY_EDITOR
        if (previousScreenResolution.x != Screen.width || previousScreenResolution.y != Screen.height) {
            screenChange.Raise();
            previousScreenResolution = new Vector2Int(Screen.width, Screen.height);
        }
#endif
    }

    public void LoadMission() {
        board.LoadMap(missionToLoad);
        loadMission.Raise();
    }

    public void EndTurn() {
        fight.NewTurn();
    }

    public void SkipCutscene() {
        cutscene.SkipCutscene();
    }

    /**
     * Return true if the battle has ended
     */
    public void CheckEndBattle() {
        if (battleState.currentBattleStep != BattleState.BattleStep.Fight) {
            //return false;
        }

        if (goingGameOver) {
            //return true;
        }

        if (battleCharacters.player.Count > 0 && battleCharacters.enemy.Count == 0) { // The player wins
            battleState.currentBattleStep = BattleState.BattleStep.Victory;

            //return true;
        } else if (battleCharacters.player.Count == 0 && battleCharacters.enemy.Count > 0) { // The enemy wins
            goingGameOver = true;
            StartCoroutine(WaitGameOver());

            //return true;
        } else if (battleCharacters.player.Count == 0 && battleCharacters.enemy.Count == 0) { // No allied and enemy chars alive, enemy wins
            goingGameOver = true;
            StartCoroutine(WaitGameOver());

            //return true;
        }

        //return false;
    }

    private IEnumerator WaitGameOver() {
        board.markedSquareAnimations.Clear();

        yield return new WaitForSeconds(1f);

        GameManager.instance.LoadSceneAsync(Scenes.GameOver);
    }

    public void OnEnterBattleStepEvent(BattleState.BattleStep battleStep) {
        switch (battleStep) {
            case BattleState.BattleStep.Placing:
                placing.EnterBattleStepPlacing();
                break;
            case BattleState.BattleStep.Fight:
                fight.EnterBattleStepFight();
                break;
            case BattleState.BattleStep.Victory:
                break;
            case BattleState.BattleStep.Cutscene:
                if (battleState.previousBattleStep == BattleState.BattleStep.Victory) {
                    cutscene.EnterBattleStepCutscene(BattleCutsceneManager.Type.Ending);
                } else {
                    cutscene.EnterBattleStepCutscene(BattleCutsceneManager.Type.Opening);
                }

                break;
        }
    }

    public void OnLeaveBattleStepEvent(BattleState.BattleStep battleStep) {
        switch (battleStep) {
            case BattleState.BattleStep.Placing:
                board.RemoveAllMarks();

                // Disable outlines from the placing step
                if (currentPartyCharacter.value.boardCharacter != null) {
                    currentPartyCharacter.value.boardCharacter.glow.Disable();
                }
                break;
            case BattleState.BattleStep.Fight:
                if (currentFightBoardCharacter.value.glow != null) {
                    currentFightBoardCharacter.value.glow.Disable();
                }

                board.RemoveAllMarks();
                break;
            case BattleState.BattleStep.Victory:
                break;
            case BattleState.BattleStep.Cutscene:
                cutscene.LeaveBattleStepCutscene();
                break;
        }
    }

    public void OnEnterTurnStepEvent(BattleState.TurnStep turnStep) {
        switch (turnStep) {
            case BattleState.TurnStep.Move:
                board.MarkSquares(
                    currentFightBoardCharacter.value.GetSquare(),
                    currentFightBoardCharacter.value.movementPoints,
                    Square.MarkType.Movement,
                    currentFightBoardCharacter.value.side.value
                );
                break;
            case BattleState.TurnStep.Attack:
                board.MarkSquares(
                    currentFightBoardCharacter.value.GetSquare(),
                    1,
                    Square.MarkType.Attack,
                    currentFightBoardCharacter.value.side.value,
                    true
                ); // TODO [ALPHA] weapon range
                break;
            case BattleState.TurnStep.Direction:
                GameObject arrowsPrefab = Resources.Load<GameObject>("Arrows");

                fight.arrows = Object.Instantiate(arrowsPrefab, currentFightBoardCharacter.value.transform.position + new Vector3(arrowsPrefab.transform.position.x, arrowsPrefab.transform.position.y), Quaternion.identity);
                break;
        }
    }

    public void OnLeaveTurnStepEvent(BattleState.TurnStep turnStep) {
        switch (turnStep) {
            case BattleState.TurnStep.Move:
            case BattleState.TurnStep.Attack:
                board.RemoveAllMarks();
                break;
            case BattleState.TurnStep.Direction:
                if (fight.arrows != null) {
                    Object.Destroy(fight.arrows);
                    fight.arrows = null;
                }

                break;
        }
    }
}
