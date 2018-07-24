using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * GameManager is the same accross all scenes (DontDestroyOnLoad) and is instantiated by the loader
 * Initialize missions, translations, monsters, etc.
 * Keep a reference to the player (which is the same across all scenes)
 * Used to make transitions & load scenes
 */
public class GameManager : MonoBehaviour {
    public static GameManager instance = null;

    public Player player;

    private Dictionary<string, RawMission> missions;

    [HideInInspector]
    public string missionToLoad;

    [SerializeField]
    private DialogBox dialogBoxPrefab;

    private DialogBox dialogBox; // The instantiated game object

    // Transitions between scenes
    private GameObject transition;
    private Image transitionImage;

    // Game initialization
    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        // TODO [BETA] do it when starting a new game (don't when loading a previously saved game)
        if (player == null) {
            player = new Player();
        }

        DontDestroyOnLoad(gameObject);

        Debug.Log("Game Manager awakes");

        // TODO [BETA] Logs + time
        InitMissions();
    }

    private void InitMissions() {
        missions = new Dictionary<string, RawMission>();

        TextAsset[] jsonMissions = Resources.LoadAll<TextAsset>("Missions");

        foreach (TextAsset jsonMission in jsonMissions) {
            RawMission mission = JsonUtility.FromJson<RawMission>(jsonMission.text);
            missions.Add(mission.id, mission);
        }
    }

    public RawMission GetMissionToLoad() {
        if (missions.ContainsKey(missionToLoad)) {
            return missions[missionToLoad];
        } else {
            Debug.LogError("MissionToLoad not found");

            return null;
        }
    }

    /**
     * TODO [BETA] Add a loading screen between scenes (or on purpose, for example when loading a mission (BattleScene)
     */
    public void LoadSceneAsync(string scene, float speed = 1f, Color? inColorN = null, Color? outColorN = null) {
        Color inColor = inColorN ?? Color.black;
        Color outColor = outColorN ?? Color.black;

        transition = new GameObject("Transition");
        transition.transform.SetParent(transform);

        Canvas myCanvas = transition.AddComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        transitionImage = transition.AddComponent<Image>();
        transitionImage.color = new Color(inColor.r, inColor.g, inColor.b, 0f);
        
        transitionImage.DOColor(inColor, speed).OnComplete(() => {
            StartCoroutine(LoadSceneAsyncCoroutine(scene, inColor, outColor, speed));
        });
    }

    private IEnumerator LoadSceneAsyncCoroutine(string scene, Color inColor, Color outColor, float speed) {
        AsyncOperation AO = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
        AO.allowSceneActivation = false;

        while (!AO.isDone) {
            if (AO.progress >= 0.9f && !AO.allowSceneActivation) {
                AO.allowSceneActivation = true;
            }

            yield return null;
        }

        transitionImage.DOColor(new Color(outColor.r, outColor.g, outColor.b, 0f), speed).OnComplete(() => {
            Destroy(transition);
        });
    }

    public DialogBox DialogBox {
        get {
            if (dialogBox == null) {
                dialogBox = Instantiate(dialogBoxPrefab, transform) as DialogBox;
            }

            return dialogBox;
        }
    }

    public void QuitGame() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
