using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Main Battle manager
 * Instantiate one manager for each battle step (BattlePlacingManager, BattleFightManager, BattleVictoryManager)
 * and dispatch events and tasks to them
 */
public class BattleManager : MonoBehaviour {
    private static BattleManager _instance;

    public enum BattleStep {
        Placing, Fight, Victory
    };
    public enum TurnStep { // Placing: None or Status - Fight: None, Move, Attack, Skill, Item, Enemy, Status - Victory: None
        None, Move, Attack, Skill, Item, Enemy, Status
    };

    public BattleStep currentBattleStep;
    public TurnStep currentTurnStep;
    public int turn;

    public Board board;
    public BattleCamera battleCamera;

    public PlayerCharacter testPlayerCharacter; //TODO: Find the correct character giving the name & job
    public FloatingText floatingText;

    public RawMission mission;

    // Characters
    public List<Character> playerPlacingChars;
    public int placingCharIndex;
    public List<BoardCharacter> playerCharacters;
    public List<BoardCharacter> enemyCharacters;
    private BoardCharacter selectedPlayerBoardCharacter;

    // HUD
    public PlacingHUD placingHUD;
    public StatusHUD statusHUD;
    public FightHUD fightHUD;
    public VictoryHUD victoryHUD;

    // Events
    public delegate void EnterStepEvent();
    public event EnterStepEvent OnEnterPlacing;
    public event EnterStepEvent OnLeavingMarkStep;

    // Dedicated managers for each BattleStep
    public BattlePlacingManager placing;
    public BattleFightManager fight;
    public BattleVictoryManager victory;
    
    public List<Sequence> markedSquareAnimations = new List<Sequence>();

    public static BattleManager instance {
        get {
            if (_instance == null)
                _instance = FindObjectOfType<BattleManager>();

            return _instance;
        }
    }

    // Initialization
    private void Awake() {
        placing = new BattlePlacingManager();
        fight = new BattleFightManager();
        victory = new BattleVictoryManager();

        currentBattleStep = BattleStep.Placing;
        currentTurnStep = TurnStep.None;
        turn = 0;

        placingCharIndex = 0;
        playerPlacingChars = new List<Character>();

        // Disable all HUD by default
        placingHUD.gameObject.SetActive(false);
        statusHUD.gameObject.SetActive(false);
        fightHUD.gameObject.SetActive(false);
        victoryHUD.gameObject.SetActive(false);
    }

    private void Start() {
        mission = GameManager.instance.GetMissionToLoad();
        board.LoadMap(mission);

        battleCamera.ResetCameraSize();
        battleCamera.SetPosition(board.width / 2, board.height / 2);

        foreach (RawMission.RawEnemy enemy in mission.enemies) {
            Character enemyChar = new Character(enemy.key);

            BoardCharacter enemyTemplate = Resources.Load("Monsters/" + enemy.key, typeof(BoardCharacter)) as BoardCharacter;

            BoardCharacter enemyBC = Instantiate(enemyTemplate, board.GetSquare(enemy.posX, enemy.posY).transform.position, Quaternion.identity);
            enemyBC.character = enemyChar;
            enemyBC.side.value = Side.Type.Neutral;
            enemyBC.SetSquare(board.GetSquare(enemy.posX, enemy.posY));
            enemyCharacters.Add(enemyBC);
        }

        placing.EnterBattleStepPlacing();
    }

    // Update is called once per frame
    private void Update() {
#if UNITY_EDITOR
        // Do not use InputBinds as this code is for editor only
        if (Input.GetKeyDown(KeyCode.O)) {
            battleCamera.ResetCameraSize();
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            battleCamera.SetPosition(0, 0, true);
        }

        if (Input.GetAxis(InputBinds.Zoom) != 0) {
            battleCamera.Zoom(Input.GetAxis(InputBinds.Zoom));
        }
#endif

        switch (currentBattleStep) {
            case BattleStep.Placing:
                placing.Update();
                break;
            case BattleStep.Fight:
                fight.Update();
                break;
            case BattleStep.Victory:
                victory.Update();
                break;
        }
    }

    public void EventOnEnterPlacing() {
        OnEnterPlacing();
    }

    public void EventOnLeavingMarkStep() {
        markedSquareAnimations.Clear();

        OnLeavingMarkStep();
    }

    /**
     * Common to Placing and Fight steps
     */
    public void EnterTurnStepStatus() {
        if (currentTurnStep != TurnStep.Status) {
            TurnStep previousTurnStep = currentTurnStep;

            currentTurnStep = TurnStep.Status;

            switch (currentBattleStep) {
                case BattleStep.Placing:
                    placing.EnterTurnStepStatus(previousTurnStep);
                    break;
                case BattleStep.Fight:
                    fight.EnterTurnStepStatus(previousTurnStep);
                    break;
            }
        }
    }

    /**
     * Common to Placing, Fight and Victory steps
     */
    public void EnterTurnStepNone() {
        if (currentTurnStep != TurnStep.None) {
            TurnStep previousTurnStep = currentTurnStep;

            currentTurnStep = TurnStep.None;

            switch (currentBattleStep) {
                case BattleStep.Placing:
                    placing.EnterTurnStepNone(previousTurnStep);
                    break;
                case BattleStep.Fight:
                    fight.EnterTurnStepNone(previousTurnStep);
                    break;
                case BattleStep.Victory:
                    victory.EnterTurnStepNone(previousTurnStep);
                    break;
            }
        }
    }

    public BoardCharacter GetSelectedPlayerBoardCharacter() {
        return selectedPlayerBoardCharacter;
    }

    public void SetSelectedPlayerBoardCharacter(BoardCharacter boardCharacter) {
        if (selectedPlayerBoardCharacter != null && selectedPlayerBoardCharacter.outline != null) {
            selectedPlayerBoardCharacter.outline.enabled = false;
        }

        selectedPlayerBoardCharacter = boardCharacter;
        fightHUD.UpdateSelectedSquare();

        if (selectedPlayerBoardCharacter != null) {
            if (selectedPlayerBoardCharacter.outline != null) {
                selectedPlayerBoardCharacter.outline.enabled = true;
            }

            battleCamera.SetPosition(selectedPlayerBoardCharacter, true);
            fightHUD.Refresh();
        }
    }

    /**
     * Return true if the battle has ended
     */
    public bool CheckEndBattle() {
        bool playerAlive = false;
        bool enemyAlive = false;

        foreach (BoardCharacter playerCharacter in playerCharacters) {
            if (!playerCharacter.IsDead()) {
                playerAlive = true;
                break;
            }
        }

        foreach (BoardCharacter enemyCharacter in enemyCharacters) {
            if (!enemyCharacter.IsDead()) {
                enemyAlive = true;
                break;
            }
        }

        if (playerAlive && !enemyAlive) { // The player wins
            victory.EnterBattleStepVictory();

            return true;
        } else if (!playerAlive && enemyAlive) { // The enemy wins
            StartCoroutine(WaitGameOver());

            return true;
        } else if (!playerAlive) { // No allied and enemy chars alive, enemy wins
            StartCoroutine(WaitGameOver());

            return true;
        }

        return false;
    }

    private IEnumerator WaitGameOver() {
        markedSquareAnimations.Clear();

        yield return new WaitForSeconds(1f);

        GameManager.instance.LoadSceneAsync(Scenes.GameOver);
    }
}
