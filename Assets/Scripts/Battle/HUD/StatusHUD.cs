﻿using DG.Tweening;
using SF;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatusHUD : MonoBehaviour {
    public RectTransform blockMiddle;
    public RectTransform blockTop;
    public RectTransform blockBottom;
    public Text statusText;

    public Character character { private set; get; }
    public BoardCharacter boardCharacter;

    public GameObject backButton;
    public GameObject quitButton;

    private float rotationSpeed = 0.2f;
    private bool isGoingDisabled = false; // True during the disabling animation
    private bool isGoingEnabled = false;

    private void Start() {
        backButton.AddListener(EventTriggerType.PointerClick, () => Hide());
        quitButton.AddListener(EventTriggerType.PointerClick, Quit);
    }

    public void Show(BoardCharacter bc, HUD.Speed speed = HUD.Speed.Normal) {
        boardCharacter = bc;
        Show(boardCharacter.character, speed);
    }

    public void Show(Character c, HUD.Speed speed = HUD.Speed.Normal) {
        float fSpeed = (int)speed / 1000f;

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
            blockMiddle.DOAnchorPos3D(new Vector3(blockMiddle.anchoredPosition3D.x, 0f, blockMiddle.anchoredPosition3D.z), fSpeed).SetEase(Ease.OutCubic);

            blockTop.anchoredPosition3D = new Vector3(blockTop.anchoredPosition3D.x, blockTop.sizeDelta.y, blockTop.anchoredPosition3D.z);
            blockTop.DOAnchorPos3D(new Vector3(blockTop.anchoredPosition3D.x, 0f, blockTop.anchoredPosition3D.z), fSpeed).SetEase(Ease.OutCubic);

            blockBottom.anchoredPosition3D = new Vector3(blockBottom.anchoredPosition3D.x, -blockBottom.sizeDelta.y, blockBottom.anchoredPosition3D.z);
            blockBottom.DOAnchorPos3D(new Vector3(blockBottom.anchoredPosition3D.x, 0f, blockBottom.anchoredPosition3D.z), fSpeed).SetEase(Ease.OutCubic);
        }
    }

    public void Hide(HUD.Speed speed = HUD.Speed.Normal) {
        if (character == null) {
            return;
        }

        float fSpeed = (int)speed / 1000f;

        boardCharacter = null;
        character = null;
        
        isGoingEnabled = false;
        isGoingDisabled = true;

        blockMiddle.DOAnchorPos3D(new Vector3(blockMiddle.anchoredPosition3D.x, -1000f, blockMiddle.anchoredPosition3D.z), fSpeed).SetEase(Ease.OutCubic);

        blockTop.DOAnchorPos3D(new Vector3(blockTop.anchoredPosition3D.x, blockTop.sizeDelta.y + 10f, blockTop.anchoredPosition3D.z), fSpeed).SetEase(Ease.OutCubic);

        blockBottom.DOAnchorPos3D(new Vector3(blockBottom.anchoredPosition3D.x, -blockBottom.sizeDelta.y, blockBottom.anchoredPosition3D.z), fSpeed).SetEase(Ease.OutCubic)
        .OnComplete(DisableGameObject);

        BattleManager.instance.EnterTurnStepNone();
    }

    private void UpdateText() {
        statusText.text = $@"
            { character.name }
            HP: { character.GetCurrentHP() }/{ character.GetMaxHP() }
            SP: { character.GetCurrentMP() }/{ character.GetMaxMP() }
            PhyAtk: { character.GetPhysicalAttack() }
            PhyDef: { character.GetPhysicalDefense() }
            MagPow: { character.GetMagicalPower() }
            MagRes: { character.GetMagicalResistance() }
        ";
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
