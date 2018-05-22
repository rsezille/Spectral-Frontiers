using UnityEngine;
using System.IO;
using RawSquare = RawMap.RawSquare;

public class BattleManager : MonoBehaviour {
    public Board board;

    // Initialization
    void Awake() {
        board = (Board) FindObjectOfType(typeof(Board));
    }

    void Start() {
        board.loadMap("001");
    }

    // Update is called once per frame
    void Update() {

    }
}
