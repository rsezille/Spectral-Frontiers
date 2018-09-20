using DG.Tweening;
using SF;
using System.Collections;
using UnityEngine;

/**
 * Main Battle manager
 * Instantiate one manager for each battle step (BattlePlacingManager, BattleFightManager, BattleVictoryManager)
 * and dispatch events and tasks to them
 */
public class BattleManager : MonoBehaviour {
    private enum LightingType {
        Day, Night, Turn, Auto
    };

    private static BattleManager _instance;

    [Header("Dependencies")]
    public BattleState battleState;
    public BattleCharacters battleCharacters;
    public MissionVariable missionToLoad;
    public Board board;
    public FloatVariable sunIntensity;
    public SunSettings sunSettings;

    [Header("Events")]
    public GameEvent screenChange;
    public GameEvent checkSemiTransparent;
    public GameEvent lightChange;

    [Header("References")]
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

    //public SunLight sunLight;

    [Header("Options")]
    public bool waterReflection = true; // TODO [BETA] Implement it
    public bool waterDistortion = true; // TODO [BETA] Implement it

    // Dedicated managers for each BattleStep
    public BattleCutsceneManager cutscene;
    public BattlePlacingManager placing;
    public BattleFightManager fight;
    public BattleVictoryManager victory;

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

        battleState.ResetData();
        battleCharacters.ResetData();
    }

    private void Start() {
        LoadMission();

        battleCamera.ResetCameraSize();
        battleCamera.SetPosition(board.width / 2, board.height / 2);

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

        if (InputManager.Pause.IsKeyDown) {
            if (pausedHUD.gameObject.activeSelf) {
                pausedHUD.Resume();
            } else {
                pausedHUD.gameObject.SetActive(true);
                Time.timeScale = 0;
            }
        }
        
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
        board.LoadMap(missionToLoad.value);
        background.Load(missionToLoad.value.background);

        // TODO: Move this to SunController?
        LightingType lightingType = EnumUtil.ParseEnum(missionToLoad.value.lighting, LightingType.Day);

        sunSettings.turnType = false;

        switch (lightingType) {
            case LightingType.Day:
                sunIntensity.value = sunSettings.dayIntensity;
                break;
            case LightingType.Night:
                sunIntensity.value = sunSettings.nightIntensity;
                break;
            case LightingType.Turn:
                sunIntensity.value = sunSettings.dayIntensity;
                sunSettings.turnType = true;
                break;
            case LightingType.Auto: // Mainly for testing purpose
                sunIntensity.value = sunSettings.nightIntensity;

                float speed = 1f;
                DOTween
                    .Sequence()
                    .Append(DOTween.To(() => sunIntensity.value, x => sunIntensity.value = x, sunSettings.dayIntensity, speed).SetEase(Ease.Linear).OnUpdate(lightChange.Raise))
                    .AppendInterval(speed * 4f)
                    .Append(DOTween.To(() => sunIntensity.value, x => sunIntensity.value = x, sunSettings.nightIntensity, speed).SetEase(Ease.Linear).OnUpdate(lightChange.Raise))
                    .AppendInterval(speed * 4f)
                    .SetLoops(-1);
                break;
        }

        lightChange.Raise();
    }

    public void SkipCutscene() {
        cutscene.SkipCutscene();
    }

    /**
     * Common to Placing and Fight steps
     */
    public void EnterTurnStepStatus() {
        if (battleState.currentTurnStep != BattleState.TurnStep.Status) {
            BattleState.TurnStep previousTurnStep = battleState.currentTurnStep;

            battleState.currentTurnStep = BattleState.TurnStep.Status;

            switch (battleState.currentBattleStep) {
                case BattleState.BattleStep.Placing:
                    placing.EnterTurnStepStatus(previousTurnStep);
                    break;
                case BattleState.BattleStep.Fight:
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

            switch (battleState.currentBattleStep) {
                case BattleState.BattleStep.Placing:
                    placing.EnterTurnStepNone(previousTurnStep);
                    break;
                case BattleState.BattleStep.Fight:
                    fight.EnterTurnStepNone(previousTurnStep);
                    break;
                case BattleState.BattleStep.Victory:
                    victory.EnterTurnStepNone(previousTurnStep);
                    break;
            }
        }
    }

    /**
     * Return true if the battle has ended
     */
    public bool CheckEndBattle() {
        if (battleState.currentBattleStep != BattleState.BattleStep.Fight) {
            return false;
        }

        if (goingGameOver) {
            return true;
        }

        if (battleCharacters.player.Count > 0 && battleCharacters.enemy.Count == 0) { // The player wins
            battleState.currentBattleStep = BattleState.BattleStep.Victory;

            return true;
        } else if (battleCharacters.player.Count == 0 && battleCharacters.enemy.Count > 0) { // The enemy wins
            goingGameOver = true;
            StartCoroutine(WaitGameOver());

            return true;
        } else if (battleCharacters.player.Count == 0 && battleCharacters.enemy.Count == 0) { // No allied and enemy chars alive, enemy wins
            goingGameOver = true;
            StartCoroutine(WaitGameOver());

            return true;
        }

        return false;
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
                victory.EnterBattleStepVictory();
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
                placing.LeaveBattleStepPlacing();
                break;
            case BattleState.BattleStep.Fight:
                fight.LeaveBattleStepFight();
                break;
            case BattleState.BattleStep.Victory:
                victory.LeaveBattleStepVictory();
                break;
            case BattleState.BattleStep.Cutscene:
                cutscene.LeaveBattleStepCutscene();
                break;
        }
    }
}
