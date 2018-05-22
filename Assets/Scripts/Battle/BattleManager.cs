using UnityEngine;
using System.IO;
using RawSquare = RawMap.RawSquare;

public class BattleManager : MonoBehaviour {
    public enum BattleStep {
        Placing, Fight, Victory
    };
    public enum TurnStep { // None during BattleSteps Placing & Victory (and Status during Placing)
        None, Wait, Move, Attack, Skill, Enemy, Status
    };

    public BattleStep currentBattleStep;
    public TurnStep currentTurnStep;
    public int turn;

    public Board board;
    public BattleCamera battleCamera;

    // Dedicated managers for each BattleStep
    public BattlePlacingManager placing;
    public BattleFightManager fight;
    public BattleVictoryManager victory;

    // Initialization
    void Awake() {
        placing = new BattlePlacingManager();
        fight = new BattleFightManager();
        victory = new BattleVictoryManager();

        board = (Board) FindObjectOfType(typeof(Board));
        battleCamera = (BattleCamera) FindObjectOfType(typeof(BattleCamera));

        currentBattleStep = BattleStep.Placing;
        currentTurnStep = TurnStep.None;
        turn = 1;
    }

    void Start() {
        board.loadMap("001");

        battleCamera.ResetCameraSize();
        battleCamera.SetPosition(board.squares[0, 0]);
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
}
