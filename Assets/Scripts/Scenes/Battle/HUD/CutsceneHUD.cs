using UnityEngine;

public class CutsceneHUD : MonoBehaviour {
    public GameObject skipButton;

    private void Start() {
        skipButton.AddListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, BattleManager.instance.cutscene.SkipCutscene);
    }
}
