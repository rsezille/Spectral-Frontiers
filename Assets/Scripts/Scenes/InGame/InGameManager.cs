using UnityEngine;

namespace SF {
    public class InGameManager : MonoBehaviour {
        public Mission testMission;

        [Header("Dependencies")]
        public MissionVariable missionToLoad;

        public void LoadTestMission() {
            missionToLoad.value = testMission;
            GameManager.instance.LoadSceneAsync(Scenes.Battle);
        }
    }
}
