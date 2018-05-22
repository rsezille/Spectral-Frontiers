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
    }

    // Update is called once per frame
    void Update() {

    }
}
