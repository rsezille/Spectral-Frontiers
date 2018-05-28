﻿using System.Collections.Generic;
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
    public enum TurnStep { // Placing: None or Status - Fight: None, Move, Attack, Skill, Enemy, Status - Victory: None
        None, Move, Attack, Skill, Enemy, Status
    };

    public BattleStep currentBattleStep;
    public TurnStep currentTurnStep;
    public int turn;

    public Board board;
    public BattleCamera battleCamera;

    public BoardChar testBoardChar; //TODO

    // Characters
    public List<Character> placingAlliedChars;
    public int placingCharIndex;
    public List<BoardChar> playerBoardChars;
    public List<BoardChar> enemyBoardChars;
    public BoardChar currentBoardChar;

    // HUD
    public PlacingHUD placingHUD;
    public StatusHUD statusHUD;
    public FightHUD fightHUD;

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
    private void Awake() {
        placing = new BattlePlacingManager();
        fight = new BattleFightManager();
        victory = new BattleVictoryManager();

        currentBattleStep = BattleStep.Placing;
        currentTurnStep = TurnStep.None;
        turn = 1;

        placingCharIndex = 0;
        placingAlliedChars = new List<Character>();

        // Disable all HUD by default
        placingHUD.gameObject.SetActive(false);
        statusHUD.gameObject.SetActive(false);
        fightHUD.gameObject.SetActive(false);
    }

    private void Start() {
        board.loadMap("001");

        battleCamera.ResetCameraSize();
        battleCamera.SetPosition(board.squares[0, 0]);

        EnterBattleStepPlacing();
    }

    // Update is called once per frame
    private void Update() {
#if UNITY_EDITOR
        // Do not use InputBinds as this code is for editor only
        if (Input.GetKeyDown(KeyCode.O)) {
            battleCamera.ResetCameraSize();
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            battleCamera.SetPosition(board.squares[0, 0], true);
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

    public void EnterBattleStepPlacing() {
        currentBattleStep = BattleStep.Placing;
        placingHUD.SetActive(true);

        if (OnEnterBattleStepPlacing != null)
            OnEnterBattleStepPlacing();
    }

    public void EnterBattleStepFight() {
        if (playerBoardChars.Count > 0) {
            if (placingAlliedChars[placingCharIndex].boardChar != null) {
                placingAlliedChars[placingCharIndex].boardChar.outline.enabled = false;
            }

            currentBattleStep = BattleStep.Fight;
            placingHUD.gameObject.SetActive(false);
            fightHUD.gameObject.SetActive(true);
            NewPlayerTurn();
        }
    }

    public void EnterBattleStepVictory() {
        currentBattleStep = BattleStep.Victory;
    }

    /**
     * Common to Placing and Fight steps
     */
    public void EnterTurnStepStatus() {
        if (currentTurnStep != TurnStep.Status) {
            currentTurnStep = TurnStep.Status;

            switch (currentBattleStep) {
                case BattleStep.Placing:
                    placing.EnterTurnStepStatus();
                    break;
                case BattleStep.Fight:
                    fight.EnterTurnStepStatus();
                    break;
            }
        }
    }

    /**
     * Common to Placing, Fight and Victory steps
     */
    public void EnterTurnStepNone() {
        currentTurnStep = TurnStep.None;

        switch (currentBattleStep) {
            case BattleStep.Placing:
                placing.EnterTurnStepNone();
                break;
            case BattleStep.Fight:
                fight.EnterTurnStepNone();
                break;
            case BattleStep.Victory:
                victory.EnterTurnStepNone();
                break;
        }
    }

    public void NewPlayerTurn() {
        currentBoardChar = playerBoardChars[0];

        foreach (BoardChar bc in playerBoardChars) {
            bc.NewTurn();
        }

        EnterTurnStepNone();
    }
}
