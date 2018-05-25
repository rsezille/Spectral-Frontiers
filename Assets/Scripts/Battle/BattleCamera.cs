using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Camera))]
public class BattleCamera : MonoBehaviour {
    private Camera battleCamera;

    // Camera speed
    public float speed = 7f;

    void Awake() {
        battleCamera = GetComponent<Camera>();
    }

    void Update() {
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

    public void SetPosition(Square square) {
        SetPosition(square, false);
    }

    public void SetPosition(Square square, bool smooth) {
        Vector3 target = new Vector3(
            square.x - square.y,
            -(square.y + square.x) / 2f + square.vOffset / (square.sprite.bounds.size.y * Globals.TileHeight / 2),
            transform.position.z
        );

        if (!smooth) {
            transform.position = target;
        } else {
            transform.DOMove(target, 1).SetEase(Ease.OutCubic);
        }
    }
}
