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
    private Dictionary<string, RawMonster> monsters;

    public string missionToLoad;

    private GameObject transition;
    private Image transitionImage;

    // Game initialization
    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        //TODO: do it when starting a new game (don't when loading a previously saved game)
        if (player == null) {
            player = new Player();
        }

        DontDestroyOnLoad(gameObject);

        Debug.Log("Game Manager awakes");

        //TODO: Logs + time
        InitMissions();
        InitMonsters();
    }

    private void InitMissions() {
        missions = new Dictionary<string, RawMission>();

        TextAsset[] jsonMissions = Resources.LoadAll<TextAsset>("Missions");

        foreach (TextAsset jsonMission in jsonMissions) {
            RawMission mission = JsonUtility.FromJson<RawMission>(jsonMission.text);
            missions.Add(mission.id, mission);
        }
    }

    private void InitMonsters() {
        monsters = new Dictionary<string, RawMonster>();

        TextAsset[] jsonMonsters = Resources.LoadAll<TextAsset>("Monsters");

        foreach (TextAsset jsonMonster in jsonMonsters) {
            RawMonster monster = JsonUtility.FromJson<RawMonster>(jsonMonster.text);
            monsters.Add(monster.id, monster);
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
     * immediate is used to not wait the fade in to be completed before loading the scene - can speed up scene loading
     * /!\ be aware that when using immediate = true, the speed must be less than the loading time (otherwise the fade in will not be finished when swaping scenes)
     * TODO [SCENE] Add a loading screen between scenes (or on purpose, for example when loading a mission (BattleScene)
     */
    public void LoadSceneAsync(string scene, bool immediate = false, float speed = 1f, Color? inColorN = null, Color? outColorN = null) {
        Color inColor = inColorN ?? Color.black;
        Color outColor = outColorN ?? Color.black;

        transition = new GameObject("Transition");
        transition.transform.SetParent(transform);

        Canvas myCanvas = transition.AddComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        transitionImage = transition.AddComponent<Image>();
        transitionImage.color = new Color(inColor.r, inColor.g, inColor.b, 0f);

        if (immediate) {
            StartCoroutine(LoadSceneAsyncCoroutine(scene, immediate, inColor, outColor, speed));
        } else {
            transitionImage.DOColor(inColor, speed).OnComplete(() => {
                StartCoroutine(LoadSceneAsyncCoroutine(scene, immediate, inColor, outColor, speed));
            });
        }
    }

    private IEnumerator LoadSceneAsyncCoroutine(string scene, bool immediate, Color inColor, Color outColor, float speed) {
        bool animationCompleted = !immediate;

        AsyncOperation AO = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
        AO.allowSceneActivation = false;

        if (immediate) {
            transitionImage.DOColor(inColor, speed).OnComplete(() => {
                animationCompleted = true;
            });
        }

        while (AO.progress < 0.9f || !animationCompleted) {
            yield return null;
        }

        AO.allowSceneActivation = true;

        transitionImage.DOColor(new Color(outColor.r, outColor.g, outColor.b, 0f), speed).OnComplete(() => {
            Destroy(transition);
        });
    }
}
