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

    public PlayerCharacter testPlayerCharacter; // TODO [ALPHA] Find the correct character giving the name & job
    public FloatingText floatingText;

    [Header("Options")]
    public bool waterReflection = true;
    public bool waterDistortion = true;

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
            enemyBC.side.value = Side.Type.Neutral;
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
        // Do not use InputBinds as this code is for editor only
        if (Input.GetKeyDown(KeyCode.O)) {
            battleCamera.ResetCameraSize();
        }

        // Do not use InputBinds as this code is for editor only
        if (Input.GetKeyDown(KeyCode.P)) {
            battleCamera.SetPosition(0, 0, true);
        }

        if (Input.GetKeyDown(KeyCode.M)) {
            GameManager.instance.DialogBox.Show("prologue_01");
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            GameManager.instance.DialogBox.Show(playerCharacters[0], "prologue_01");
        }

        if (currentBattleStep != BattleStep.Cinematic && Input.GetAxis(InputManager.Axis.Zoom) != 0) {
            battleCamera.Zoom(Input.GetAxis(InputManager.Axis.Zoom));

            OnZoomChange?.Invoke();
        }
        #endif

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
            cinematic.EnterBattleStepCinematic(BattleCinematicManager.Type.Ending);

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
