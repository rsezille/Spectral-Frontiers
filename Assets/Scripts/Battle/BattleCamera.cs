﻿using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Camera))]
public class BattleCamera : MonoBehaviour {
    private Camera battleCamera;

    // Camera speed
    public float speed = 7f;

    private float positionYOffset = 0.7f;

    private void Awake() {
        battleCamera = GetComponent<Camera>();
    }

    private void Update() {
        float tmpSpeed = speed;

        // Lower the speed if the camera is going diagonally
        if (Input.GetAxisRaw(InputBinds.CameraH) != 0 && Input.GetAxisRaw(InputBinds.CameraV) != 0) {
            tmpSpeed = speed / Mathf.Sqrt(2f);
        }

        transform.position += new Vector3(tmpSpeed * Time.deltaTime * Input.GetAxisRaw(InputBinds.CameraH), tmpSpeed * Time.deltaTime * Input.GetAxisRaw(InputBinds.CameraV));
    }
    
    public void Zoom(float axis) {
        battleCamera.orthographicSize = Mathf.Clamp(
            battleCamera.orthographicSize - axis / 0.5f,
            0.1f,
            Screen.height / (Globals.TileHeight * 2f) * 2f
        );
    }

    public void ResetCameraSize() {
        battleCamera.orthographicSize = Screen.height / (Globals.TileHeight * 2f);
    }

    public Tween SetPosition(BoardCharacter boardCharacter, bool smooth = false, float duration = 1f, Ease ease = Ease.OutCubic) {
        return SetPosition(boardCharacter.GetSquare(), smooth, duration, ease);
    }

    public Tween SetPosition(Square square, bool smooth = false, float duration = 1f, Ease ease = Ease.OutCubic) {
        Vector3 target = new Vector3(
            square.x - square.y,
            -(square.y + square.x) / 2f + square.vOffset / (square.sprite.bounds.size.y * Globals.TileHeight / 2) + positionYOffset,
            transform.position.z
        );

        if (!smooth) {
            transform.position = target;

            return null;
        } else {
            return transform.DOMove(target, duration).SetEase(ease);
        }
    }
}
