using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatusHUD : MonoBehaviour {
    public RectTransform blockMiddle;
    public RectTransform blockTop;
    public RectTransform blockBottom;
    public Text statusText;

    private Character character;
    public PlayerCharacter playerCharacter;

    public GameObject backButton;
    public GameObject quitButton;

    private float rotationSpeed = 0.2f;
    private float animationSpeed = 0.6f;
    private bool isGoingDisabled = false; // True during the disabling animation
    private bool isGoingEnabled = false;

    public BattleManager battleManager;

    private void Awake() {
        battleManager = BattleManager.instance;
    }

    private void Start() {
        backButton.AddListener(EventTriggerType.PointerClick, Hide);
        quitButton.AddListener(EventTriggerType.PointerClick, Quit);
    }

    public void Show(PlayerCharacter pc) {
        playerCharacter = pc;
        Show(pc.boardCharacter.character);
    }

    public void Show(Character c) {
        character = c;
        
        // When switching characters but staying on the status HUD (can't switch during disabling animation)
        if (gameObject.activeSelf && !isGoingDisabled) {
            blockMiddle.DORotate(new Vector3(0f, 90f, 0f), rotationSpeed).SetEase(Ease.Linear)
            .OnComplete(() => {
                UpdateText();
                blockMiddle.DORotate(new Vector3(0f, 0f, 0f), rotationSpeed).SetEase(Ease.Linear);
            });
        } else { // When activating the status HUD
            isGoingEnabled = true;
            isGoingDisabled = false; // In case we open/close the HUD too fast

            gameObject.SetActive(true);
            UpdateText();

            blockMiddle.anchoredPosition3D = new Vector3(blockMiddle.anchoredPosition3D.x, -1000f, blockMiddle.anchoredPosition3D.z);
            blockMiddle.DOAnchorPos3D(new Vector3(blockMiddle.anchoredPosition3D.x, 0f, blockMiddle.anchoredPosition3D.z), animationSpeed).SetEase(Ease.OutCubic);

            blockTop.anchoredPosition3D = new Vector3(blockTop.anchoredPosition3D.x, blockTop.sizeDelta.y, blockTop.anchoredPosition3D.z);
            blockTop.DOAnchorPos3D(new Vector3(blockTop.anchoredPosition3D.x, 0f, blockTop.anchoredPosition3D.z), animationSpeed).SetEase(Ease.OutCubic);

            blockBottom.anchoredPosition3D = new Vector3(blockBottom.anchoredPosition3D.x, -blockBottom.sizeDelta.y, blockBottom.anchoredPosition3D.z);
            blockBottom.DOAnchorPos3D(new Vector3(blockBottom.anchoredPosition3D.x, 0f, blockBottom.anchoredPosition3D.z), animationSpeed).SetEase(Ease.OutCubic);
        }
    }

    public void Hide() {
        playerCharacter = null;
        character = null;
        
        isGoingEnabled = false;
        isGoingDisabled = true;

        blockMiddle.DOAnchorPos3D(new Vector3(blockMiddle.anchoredPosition3D.x, -1000f, blockMiddle.anchoredPosition3D.z), animationSpeed).SetEase(Ease.OutCubic);

        blockTop.DOAnchorPos3D(new Vector3(blockTop.anchoredPosition3D.x, blockTop.sizeDelta.y + 10f, blockTop.anchoredPosition3D.z), animationSpeed).SetEase(Ease.OutCubic);

        blockBottom.DOAnchorPos3D(new Vector3(blockBottom.anchoredPosition3D.x, -blockBottom.sizeDelta.y, blockBottom.anchoredPosition3D.z), animationSpeed).SetEase(Ease.OutCubic)
        .OnComplete(DisableGameObject);

        battleManager.EnterTurnStepNone();
    }

    private void UpdateText() {
        statusText.text = character.name + "\n";
        statusText.text += "HP: " + character.GetCurrentHP() + "/" + character.GetMaxHP() + "\n";
        statusText.text += "SP: " + character.GetCurrentMP() + "/" + character.GetMaxMP() + "\n";
        statusText.text += "PhyAtk: " + character.GetPhysicalAttack() + "\n";
        statusText.text += "PhyDef: " + character.GetPhysicalDefense() + "\n";
        statusText.text += "MagPow: " + character.GetMagicalPower() + "\n";
        statusText.text += "MagRes: " + character.GetMagicalResistance();
    }

    private void DisableGameObject() {
        if (isGoingEnabled) return;

        isGoingDisabled = false;
        gameObject.SetActive(false);
    }

    private void Quit() {
        Application.Quit();
    }
}
