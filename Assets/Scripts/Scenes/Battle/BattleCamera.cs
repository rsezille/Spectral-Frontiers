using UnityEngine;
using DG.Tweening;
using SF;

[RequireComponent(typeof(Camera))]
public class BattleCamera : MonoBehaviour {
    private Camera battleCamera;

    [Header("Dependencies")]
    public BattleState battleState;
    public Board board;
    public CameraPosition mainCameraPosition;

    [Header("Events")]
    public GameEvent zoomChange;

    [Header("Data")]
    public float speed = 7f; // Camera speed

    private void Awake() {
        battleCamera = GetComponent<Camera>();
    }

    private void Start() {
        ResetCameraSize();
    }

    private void Update() {
        /*if (battleState.currentBattleStep == BattleState.BattleStep.Cutscene
                || battleState.currentTurnStep == BattleState.TurnStep.Enemy) {
            return;
        }*/

        #if UNITY_EDITOR
        // Do not use InputBinds as this code is for editor only
        if (Input.GetKeyDown(KeyCode.O)) {
            ResetCameraSize();
        }
        
        if (Input.GetKeyDown(KeyCode.P)) {
            mainCameraPosition.SetPosition(board.GetSquare(0, 0), true);
        }

        if (Input.GetAxis(InputManager.Axis.Zoom) != 0 && Time.timeScale > 0) {
            Zoom(Input.GetAxis(InputManager.Axis.Zoom));

            zoomChange.Raise();
        }
        #endif
        
        if (InputManager.CameraHorizontalAxis() != 0 || InputManager.CameraVerticalAxis() != 0) {
            float tmpSpeed = speed;

            // Lower the speed if the camera is going diagonally
            if (InputManager.CameraHorizontalAxis() != 0 && InputManager.CameraVerticalAxis() != 0) {
                tmpSpeed = speed / Mathf.Sqrt(2f);
            }

            mainCameraPosition.position += new Vector3(tmpSpeed * Time.deltaTime * InputManager.CameraHorizontalAxis(), tmpSpeed * Time.deltaTime * InputManager.CameraVerticalAxis());
        }

        transform.position = mainCameraPosition.position;
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
}
