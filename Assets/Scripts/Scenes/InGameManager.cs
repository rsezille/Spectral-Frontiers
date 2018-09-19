using UnityEngine;
using UnityEngine.EventSystems;

public class InGameManager : MonoBehaviour {
    public GameObject testMissionButton;

    private void Start() {
        testMissionButton.AddListener(EventTriggerType.PointerClick, TestMission);
    }

    private void TestMission() {
        //GameManager.instance.missionToLoad = "story_01";
        GameManager.instance.LoadSceneAsync(Scenes.Battle);
    }
}
