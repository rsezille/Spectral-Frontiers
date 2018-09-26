using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SF {
    public class TurnHUD : MonoBehaviour {
        private class FakeTick {
            public int tick;
            public BoardCharacter boardCharacter;

            public FakeTick(int tick, BoardCharacter boardCharacter) {
                this.tick = tick;
                this.boardCharacter = boardCharacter;
            }
        }

        [Header("Dependencies")]
        public BattleCharacters battleCharacters;
        public IntVariable turnSpeed;
        public Board board;

        [Header("Direct references")]
        public TextMeshProUGUI text;

        [Header("Data")]
        public int simulationTurns = 10;

        /**
         * /!\ Also update the turn algo to reflect changes, in BattleFightManager
         */
        public void Simulate() {
            List<BoardCharacter> turnCharacters = new List<BoardCharacter>();

            List<FakeTick> fakeTicks = new List<FakeTick>();

            battleCharacters.player.ForEach(boardCharacter => {
                fakeTicks.Add(new FakeTick(boardCharacter.tick, boardCharacter));
            });

            battleCharacters.enemy.ForEach(boardCharacter => {
                fakeTicks.Add(new FakeTick(boardCharacter.tick, boardCharacter));
            });

            while (turnCharacters.Count < simulationTurns) {
                while (!CharacterReadyTest(fakeTicks)) {
                    fakeTicks.ForEach(test => {
                        test.tick = test.tick + 1;
                    });
                }

                BoardCharacter nextBoardCharacter = GetCharacterToPlayTest(fakeTicks);
                turnCharacters.Add(nextBoardCharacter);

                FakeTick fakeTickToReset = fakeTicks.Find(test => test.boardCharacter == nextBoardCharacter);
                fakeTickToReset.tick = 0;
            }

            string t = "";

            turnCharacters.ForEach(c => t += string.IsNullOrEmpty(t) ? c.name : " > " + c.name);
            text.SetText(t);
        }

        private bool CharacterReadyTest(List<FakeTick> fakeTicks) {
            foreach (FakeTick fakeTick in fakeTicks) {
                if (fakeTick.tick >= turnSpeed - fakeTick.boardCharacter.character.spd) {
                    return true;
                }
            }

            return false;
        }

        private BoardCharacter GetCharacterToPlayTest(List<FakeTick> fakeTicks) {
            BoardCharacter characterToPlay = null;

            foreach (FakeTick fakeTick in fakeTicks) {
                if (fakeTick.tick >= turnSpeed - fakeTick.boardCharacter.character.spd) {
                    if (characterToPlay == null) {
                        characterToPlay = fakeTick.boardCharacter;
                    } else if (board.SquarePositionToIndex(fakeTick.boardCharacter.GetSquare()) < board.SquarePositionToIndex(characterToPlay.GetSquare())) {
                        characterToPlay = fakeTick.boardCharacter;
                    }
                }
            }

            return characterToPlay;
        }
    }
}
