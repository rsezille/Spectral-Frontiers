using UnityEngine;
using DG.Tweening;
using SF;

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
        if (BattleManager.instance.currentBattleStep != BattleManager.BattleStep.Cinematic) {
            float tmpSpeed = speed;

            // Lower the speed if the camera is going diagonally
            if (InputManager.CameraHorizontalAxis() != 0 && InputManager.CameraVerticalAxis() != 0) {
                tmpSpeed = speed / Mathf.Sqrt(2f);
            }

            transform.position += new Vector3(tmpSpeed * Time.deltaTime * InputManager.CameraHorizontalAxis(), tmpSpeed * Time.deltaTime * InputManager.CameraVerticalAxis());
        }
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
        return SetPosition(square.x, square.y, smooth, duration, ease);
    }

    public Tween SetPosition(int squareX, int squareY, bool smooth = false, float duration = 1f, Ease ease = Ease.OutCubic) {
        Square targetedSquare = BattleManager.instance.board.GetSquare(squareX, squareY);
        int height = targetedSquare != null ? targetedSquare.Height : 0;

        Vector3 target = new Vector3(
            squareX - squareY,
            (squareY + squareX) / 2f + (height / Globals.PixelsPerUnit) + positionYOffset,
            transform.position.z
        );

        if (!smooth) {
            transform.position = target;

            return null;
        } else {
            return transform.DOMove(target, duration).SetEase(ease);
        }
    }

    public bool IsOnSquare(Square square) {
        Vector3 squarePosition = new Vector3(
            square.x - square.y,
            (square.y + square.x) / 2f + (square.Height / Globals.PixelsPerUnit) + positionYOffset,
            transform.position.z
        );

        return transform.position == squarePosition;
    }
}
