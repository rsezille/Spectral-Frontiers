using UnityEngine;
using UnityEngine.UI;

public class StatusHUD : MonoBehaviour {
    public RectTransform blockMiddle;
    public RectTransform blockTop;
    public RectTransform blockBottom;
    public Text statusText;

    private Character character;
    public BoardChar boardChar;

    public GameObject backButton;
    public GameObject quitButton;

    private float rotationSpeed = 5f;
    private bool isGoingDisabled = false; // True during the disabling animation

    void Start() {
        backButton.GetComponent<Button>().onClick.AddListener(Hide);
        quitButton.GetComponent<Button>().onClick.AddListener(Quit);
    }

    public void Show(BoardChar bc) {
        boardChar = bc;
        Show(bc.character);
    }

    public void Show(Character c) {
        character = c;

        // When switching characters but staying on the status HUD (can't switch during disabling animation)
        if (gameObject.activeSelf && !isGoingDisabled) {
            StopAllCoroutines();
            StartCoroutine(GenericCoroutine.RotateRectTransform(
                blockMiddle,
                Quaternion.Euler(0f, 90f, 0f),
                callback: () => { ToggleAndText(); },
                speed: rotationSpeed
            ));
        } else { // When activating the status HUD
            StopAllCoroutines();

            isGoingDisabled = false; // In case we open/close the HUD too fast

            gameObject.SetActive(true);
            UpdateText();

            StartCoroutine(GenericCoroutine.ToggleItem(
                blockMiddle,
                new Vector3(blockMiddle.anchoredPosition3D.x, -1000f, blockMiddle.anchoredPosition3D.z),
                new Vector3(blockMiddle.anchoredPosition3D.x, 0f, blockMiddle.anchoredPosition3D.z)
            ));
            StartCoroutine(GenericCoroutine.ToggleItem(
                blockTop,
                new Vector3(blockTop.anchoredPosition3D.x, blockTop.sizeDelta.y, blockTop.anchoredPosition3D.z),
                new Vector3(blockTop.anchoredPosition3D.x, 0f, blockTop.anchoredPosition3D.z)
            ));
            StartCoroutine(GenericCoroutine.ToggleItem(
                blockBottom,
                new Vector3(blockBottom.anchoredPosition3D.x, -blockBottom.sizeDelta.y, blockBottom.anchoredPosition3D.z),
                new Vector3(blockBottom.anchoredPosition3D.x, 0f, blockBottom.anchoredPosition3D.z)
            ));
        }
    }

    public void Hide() {
        boardChar = null;
        character = null;

        if (gameObject.activeSelf && !isGoingDisabled) {
            isGoingDisabled = true;
            StopAllCoroutines();
            StartCoroutine(GenericCoroutine.ToggleItem(
                blockMiddle,
                new Vector3(blockMiddle.anchoredPosition3D.x, 0f, blockMiddle.anchoredPosition3D.z),
                new Vector3(blockMiddle.anchoredPosition3D.x, -1000f, blockMiddle.anchoredPosition3D.z),
                DisableGameObject
            ));
            StartCoroutine(GenericCoroutine.ToggleItem(
                blockTop,
                new Vector3(blockTop.anchoredPosition3D.x, 0f, blockTop.anchoredPosition3D.z),
                new Vector3(blockTop.anchoredPosition3D.x, blockTop.sizeDelta.y + 10f, blockTop.anchoredPosition3D.z),
                DisableGameObject
            ));
            StartCoroutine(GenericCoroutine.ToggleItem(
                blockBottom,
                new Vector3(blockBottom.anchoredPosition3D.x, 0f, blockBottom.anchoredPosition3D.z),
                new Vector3(blockBottom.anchoredPosition3D.x, -blockBottom.sizeDelta.y, blockBottom.anchoredPosition3D.z),
                DisableGameObject
            ));

            if (BattleManager.instance.currentBattleStep == BattleManager.BattleStep.Fight) {
                //BattleManager.instance.fightHUD.SetActive(true);
                //BattleManager.instance.EnterTurnStepWait();
            } else if (BattleManager.instance.currentBattleStep == BattleManager.BattleStep.Placing) {
                BattleManager.instance.placingHUD.SetActive(true);
                BattleManager.instance.EnterTurnStepNone();
            }
        }
    }

    private void ToggleAndText() {
        UpdateText();

        StartCoroutine(GenericCoroutine.RotateRectTransform(
            blockMiddle,
            Quaternion.Euler(0f, 0f, 0f),
            speed: rotationSpeed
        ));
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

    void DisableGameObject() {
        isGoingDisabled = false;
        gameObject.SetActive(false);
    }

    private void Quit() {
        Application.Quit();
    }
}
