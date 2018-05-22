using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {
    private static BattleManager _instance;

    public enum BattleStep {
        Placing, Fight, Victory
    };
    public enum TurnStep { // None during BattleSteps Placing & Victory ; Placing can be None or Status
        None, Wait, Move, Attack, Skill, Enemy, Status
    };

    public BattleStep currentBattleStep;
    public TurnStep currentTurnStep;
    public int turn;

    public Board board;
    public BattleCamera battleCamera;

    // Characters
    public List<Character> placingAlliedChars;
    public int placingCharIndex;
    public List<BoardChar> alliedMapChars;

    // HUD
    public PlacingHUD placingHUD;

    // Events
    public delegate void EnterStepPlacing();
    public static event EnterStepPlacing OnEnterBattleStepPlacing;

    // Dedicated managers for each BattleStep
    public BattlePlacingManager placing;
    public BattleFightManager fight;
    public BattleVictoryManager victory;

    public static BattleManager instance {
        get {
            if (_instance == null)
                _instance = FindObjectOfType<BattleManager>();

            return _instance;
        }
    }

    // Initialization
    void Awake() {
        placing = new BattlePlacingManager();
        fight = new BattleFightManager();
        victory = new BattleVictoryManager();

        board = (Board) FindObjectOfType(typeof(Board));
        battleCamera = (BattleCamera) FindObjectOfType(typeof(BattleCamera));
        placingHUD = (PlacingHUD) FindObjectOfType(typeof(PlacingHUD));

        currentBattleStep = BattleStep.Placing;
        currentTurnStep = TurnStep.None;
        turn = 1;

        placingCharIndex = 0;
        placingAlliedChars = new List<Character>();

        // Disable all HUD by default
        placingHUD.gameObject.SetActive(false);
    }

    void Start() {
        board.loadMap("001");

        battleCamera.ResetCameraSize();
        battleCamera.SetPosition(board.squares[0, 0]);

        EnterBattleStepPlacing();
    }

    // Update is called once per frame
    void Update() {
#if UNITY_EDITOR
        // Do not use InputBinds as this code is for editor only
        if (Input.GetKeyDown(KeyCode.O)) {
            battleCamera.ResetCameraSize();
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            battleCamera.SetPosition(board.squares[0, 0], true);
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

    public void EnterBattleStepPlacing() {
        currentBattleStep = BattleStep.Placing;
        placingHUD.SetActive(true);

        if (OnEnterBattleStepPlacing != null)
            OnEnterBattleStepPlacing();
    }

    public void EnterTurnStepStatusFromPlacing() {
        if (currentTurnStep != TurnStep.Status) {
            currentTurnStep = TurnStep.Status;
            placingHUD.SetActive(false);

            //statusHUD.Show(placing.GetCurrentPlacingChar());
        }
    }
}
