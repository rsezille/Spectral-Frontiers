using UnityEngine;
using System.IO;
using RawSquare = RawMap.RawSquare;

public class BattleManager : MonoBehaviour {
    public Board board;
    public BattleCamera battleCamera;

    // Initialization
    void Awake() {
        board = (Board) FindObjectOfType(typeof(Board));
        battleCamera = (BattleCamera) FindObjectOfType(typeof(BattleCamera));
    }

    void Start() {
        board.loadMap("001");

        battleCamera.ResetCameraSize();
        battleCamera.SetPosition(board.squares[0, 0]);
    }

    // Update is called once per frame
    void Update() {
#if UNITY_EDITOR
        // Do not use InputBinds as this code is for editor only
        if (Input.GetKeyDown(KeyCode.O)) {
            battleCamera.ResetCameraSize();
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            battleCamera.SetPosition(board.squares[0, 0], true);
        }
#endif
    }
}
