using UnityEngine;

public class CinematicHUD : MonoBehaviour {
    public GameObject skipButton;

    private void Start() {
        skipButton.AddListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, BattleManager.instance.cinematic.SkipCinematic);
    }
}
