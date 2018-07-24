using DG.Tweening;
using SF;
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
        Cinematic, Placing, Fight, Victory
    };
    public enum TurnStep { // Placing: None or Status - Fight: None, Move, Attack, Skill, Item, Enemy, Status, Direction - Victory: None
        None, Move, Attack, Skill, Item, Enemy, Status, Direction
    };

    [Header("Information")]
    public BattleStep currentBattleStep;
    public TurnStep currentTurnStep;
    public int turn;

    public RawMission mission;

    // Characters
    public List<BoardCharacter> playerCharacters;
    public List<BoardCharacter> enemyCharacters;

    [Header("Direct references")]
    public Board board;
    public BattleCamera battleCamera;

    // HUD
    public PlacingHUD placingHUD;
    public StatusHUD statusHUD;
    public FightHUD fightHUD;
    public VictoryHUD victoryHUD;
    public PausedHUD pausedHUD;

    public PlayerCharacter testPlayerCharacter; // TODO [ALPHA] Find the correct character giving the name & job
    public FloatingText floatingText;

    [Header("Options")]
    public bool waterReflection = true; // TODO [BETA] Implement it
    public bool waterDistortion = true; // TODO [BETA] Implement it

    // Events
    public delegate void SFEvent();
    public event SFEvent OnEnterPlacing;
    public event SFEvent OnLeavingMarkStep;
    public event SFEvent OnZoomChange;
    public event SFEvent OnScreenChange;
    public event SFEvent OnSemiTransparentReset;
    public event SFEvent OnCheckSemiTransparent;

    // Dedicated managers for each BattleStep
    public BattleCinematicManager cinematic;
    public BattlePlacingManager placing;
    public BattleFightManager fight;
    public BattleVictoryManager victory;
    
    public List<Sequence> markedSquareAnimations = new List<Sequence>();

    private bool goingGameOver = false;

    #if UNITY_EDITOR
    private Vector2Int previousScreenResolution;
    #endif

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
        cinematic = new BattleCinematicManager();

        // Disable all HUD by default
        placingHUD.gameObject.SetActive(false);
        statusHUD.gameObject.SetActive(false);
        fightHUD.gameObject.SetActive(false);
        victoryHUD.gameObject.SetActive(false);
        pausedHUD.gameObject.SetActive(false);

        #if UNITY_EDITOR
        previousScreenResolution = new Vector2Int(Screen.width, Screen.height);
        #endif
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
            enemyBC.side.value = Side.Type.Enemy;
            enemyBC.SetSquare(board.GetSquare(enemy.posX, enemy.posY));
            enemyCharacters.Add(enemyBC);
        }
        
        currentTurnStep = TurnStep.None;
        turn = 0;

        cinematic.EnterBattleStepCinematic(BattleCinematicManager.Type.Opening);
    }

    // Update is called once per frame
    private void Update() {
        #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.M)) {
                GameManager.instance.DialogBox.Show("prologue_01");
            }

            if (Input.GetKeyDown(KeyCode.L)) {
                GameManager.instance.DialogBox.Show(playerCharacters[0], "prologue_01");
            }
        #endif

        if (InputManager.Pause.IsKeyDown) {
            if (pausedHUD.gameObject.activeSelf) {
                pausedHUD.Resume();
            } else {
                pausedHUD.gameObject.SetActive(true);
                Time.timeScale = 0;
            }
        }

        switch (currentBattleStep) {
            case BattleStep.Cinematic:
                cinematic.Update();
                break;
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

        #if UNITY_EDITOR
            if (previousScreenResolution.x != Screen.width || previousScreenResolution.y != Screen.height) {
                OnScreenChange?.Invoke();
                previousScreenResolution = new Vector2Int(Screen.width, Screen.height);
            }
        #endif
    }

    public void EventOnEnterPlacing() {
        OnEnterPlacing?.Invoke();
    }

    public void EventOnZoomChange() {
        OnZoomChange?.Invoke();
    }

    public void EventOnSemiTransparentReset() {
        OnSemiTransparentReset?.Invoke();
        OnCheckSemiTransparent?.Invoke();
    }

    public void EventOnLeavingMarkStep() {
        markedSquareAnimations.Clear();

        OnLeavingMarkStep?.Invoke();
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

    /**
     * Return true if the battle has ended
     */
    public bool CheckEndBattle() {
        if (currentBattleStep != BattleStep.Fight) {
            return false;
        }

        if (goingGameOver) {
            return true;
        }

        if (playerCharacters.Count > 0 && enemyCharacters.Count == 0) { // The player wins
            cinematic.EnterBattleStepCinematic(BattleCinematicManager.Type.Ending);

            return true;
        } else if (playerCharacters.Count == 0 && enemyCharacters.Count > 0) { // The enemy wins
            goingGameOver = true;
            StartCoroutine(WaitGameOver());

            return true;
        } else if (playerCharacters.Count == 0 && enemyCharacters.Count == 0) { // No allied and enemy chars alive, enemy wins
            goingGameOver = true;
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
