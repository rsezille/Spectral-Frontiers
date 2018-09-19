﻿using DG.Tweening;
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

    [Header("Dependencies")]
    public BattleState battleState;

    public enum BattleStep {
        Cutscene, Placing, Fight, Victory
    };
    
    private BattleStep _currentBattleStep;
    public BattleStep currentBattleStep {
        get { return _currentBattleStep; }
        set {
            BattleStep previousBattleStep = _currentBattleStep;

            switch (_currentBattleStep) {
                case BattleStep.Placing:
                    placing.LeaveBattleStepPlacing();
                    break;
                case BattleStep.Fight:
                    fight.LeaveBattleStepFight();
                    break;
                case BattleStep.Victory:
                    victory.LeaveBattleStepVictory();
                    break;
                case BattleStep.Cutscene:
                    cutscene.LeaveBattleStepCutscene();
                    break;
            }

            _currentBattleStep = value;

            switch (value) {
                case BattleStep.Placing:
                    placing.EnterBattleStepPlacing();
                    break;
                case BattleStep.Fight:
                    fight.EnterBattleStepFight();
                    break;
                case BattleStep.Victory:
                    victory.EnterBattleStepVictory();
                    break;
                case BattleStep.Cutscene:
                    if (previousBattleStep == BattleStep.Victory) {
                        cutscene.EnterBattleStepCutscene(BattleCutsceneManager.Type.Ending);
                    } else {
                        cutscene.EnterBattleStepCutscene(BattleCutsceneManager.Type.Opening);
                    }

                    break;
            }
        }
    }

    [Header("Data")]
    public int turn;

    public RawMission mission;

    // Characters
    public List<BoardCharacter> playerCharacters;
    public List<BoardCharacter> enemyCharacters;

    [Header("References")]
    public Board board;
    public BattleCamera battleCamera;
    public Background background;

    // HUD
    public CutsceneHUD cutsceneHUD;
    public PlacingHUD placingHUD;
    public StatusHUD statusHUD;
    public FightHUD fightHUD;
    public VictoryHUD victoryHUD;
    public PausedHUD pausedHUD;
    public TurnHUD turnHUD;

    public PlayerCharacter testPlayerCharacter; // TODO [ALPHA] Find the correct character giving the name & job
    public FloatingText floatingText;

    public SunLight sunLight;

    [Header("Options")]
    public bool waterReflection = true; // TODO [BETA] Implement it
    public bool waterDistortion = true; // TODO [BETA] Implement it

    // Events
    public event GameManager.SFEvent OnEnterPlacing;
    public event GameManager.SFEvent OnLeavingMarkStep;
    public event GameManager.SFEvent OnZoomChange;
    public event GameManager.SFEvent OnScreenChange;
    public event GameManager.SFEvent OnSemiTransparentReset;
    public event GameManager.SFEvent OnCheckSemiTransparent;
    public event GameManager.SFEvent OnLightChange;

    // Dedicated managers for each BattleStep
    public BattleCutsceneManager cutscene;
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
        placing = new BattlePlacingManager(this);
        fight = new BattleFightManager(this);
        victory = new BattleVictoryManager(this);
        cutscene = new BattleCutsceneManager(this);

        // Disable all HUD by default
        cutsceneHUD.gameObject.SetActive(false);
        placingHUD.gameObject.SetActive(false);
        statusHUD.gameObject.SetActive(false);
        fightHUD.gameObject.SetActive(false);
        victoryHUD.gameObject.SetActive(false);
        pausedHUD.gameObject.SetActive(false);
        turnHUD.gameObject.SetActive(false);

        #if UNITY_EDITOR
        previousScreenResolution = new Vector2Int(Screen.width, Screen.height);
        #endif
    }

    private void Start() {
        LoadMission();

        battleCamera.ResetCameraSize();
        battleCamera.SetPosition(board.width / 2, board.height / 2);
        
        battleState.currentTurnStep = BattleState.TurnStep.None;
        turn = 0;

        currentBattleStep = BattleStep.Cutscene;

        Time.timeScale = PlayerOptions.GetFloat(PlayerOptions.BattleSpeed);
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
            case BattleStep.Cutscene:
                cutscene.Update();
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

    public void EventOnLightChange() {
        OnLightChange?.Invoke();
    }

    public void LoadMission() {
        mission = GameManager.instance.GetMissionToLoad();
        board.LoadMap(mission);
        background.Load(mission.background);
        sunLight.Load(mission.lighting);
    }

    /**
     * Common to Placing and Fight steps
     */
    public void EnterTurnStepStatus() {
        if (battleState.currentTurnStep != BattleState.TurnStep.Status) {
            BattleState.TurnStep previousTurnStep = battleState.currentTurnStep;

            battleState.currentTurnStep = BattleState.TurnStep.Status;

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
        if (battleState.currentTurnStep != BattleState.TurnStep.None) {
            BattleState.TurnStep previousTurnStep = battleState.currentTurnStep;

            battleState.currentTurnStep = BattleState.TurnStep.None;

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
            currentBattleStep = BattleStep.Victory;

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
