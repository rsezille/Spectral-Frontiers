using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace SF {
    /**
     * Load the map
     * Propagate marks for moving and attacking
     */
    [CreateAssetMenu(menuName = "SF/Systems/Board")]
    public class Board : ScriptableObject {
        private Square[] squares;

        public PathFinder pathFinder;

        public int width { get; private set; } = 0;
        public int height { get; private set; } = 0;

        public string mapName { get; private set; } = "";

        public List<Sequence> markedSquareAnimations = new List<Sequence>();

        public void ResetData() {
            squares = null;
            width = 0;
            height = 0;
            mapName = "";
            markedSquareAnimations = new List<Sequence>();
        }

        public void RemoveAllMarks() {
            markedSquareAnimations.Clear();

            foreach (Square s in squares) {
                s.RemoveMark();
            }
        }

        public void LoadMap(Mission mission) {
            GameObject map = Instantiate(mission.map, Vector3.zero, Quaternion.identity).gameObject;

            Square[] mapSquares = map.GetComponentsInChildren<Square>();

            width = 0;
            height = 0;

            foreach (Square mapSquare in mapSquares) {
                if (mapSquare.x > width) width = mapSquare.x;
                if (mapSquare.y > height) height = mapSquare.y;
            }

            width++; // Squares start from index 0
            height++; // Squares start from index 0

            squares = new Square[width * height];

            foreach (Square mapSquare in mapSquares) {
                int squareIndex = mapSquare.x + (mapSquare.y * width);

                squares[squareIndex] = mapSquare;
            }

            foreach (Mission.StartingSquare startingSquare in mission.startingSquares) {
                squares[PositionToIndexSquare(startingSquare.posX, startingSquare.posY)].startingDirection = startingSquare.direction;
            }

            pathFinder = new PathFinder(this, width + height);

            mapName = LanguageManager.instance.GetString("map." + mission.map.gameObject.name + ".name");
        }

        public Square[] GetSquares() {
            return squares;
        }

        /**
         * Return the square according to x and y coordinates ; null if coordinates are outside the board
         */
        public Square GetSquare(int x, int y) {
            if (x > width - 1 || x < 0 || y > height - 1 || y < 0) {
                return null;
            }

            return squares[PositionToIndexSquare(x, y)];
        }

        public int PositionToIndexSquare(int x, int y) {
            return x + (y * width);
        }

        /**
         * Return all square hit by a linear propagation, epicenter excluded
         * ignoreBlocking is usefull for attacks
         */
        public List<Square> PropagateLinear(Square epicenter, int distance, Side.Type side, bool ignoreBlocking) {
            List<Square> squaresHit = new List<Square>();

            List<Square> tmp1 = new List<Square>();
            List<Square> tmp2 = new List<Square>();

            squaresHit.Add(epicenter);
            tmp1.Add(epicenter);

            for (int i = 0; i < distance; i++) {
                foreach (Square square in tmp1) {
                    Square north = GetSquare(square.x, square.y - 1);
                    Square south = GetSquare(square.x, square.y + 1);
                    Square west = GetSquare(square.x - 1, square.y);
                    Square east = GetSquare(square.x + 1, square.y);

                    if (north != null && !squaresHit.Contains(north) && ((north.IsNotBlocking(side) && !ignoreBlocking) || ignoreBlocking)) {
                        tmp2.Add(north);
                    }

                    if (south != null && !squaresHit.Contains(south) && ((south.IsNotBlocking(side) && !ignoreBlocking) || ignoreBlocking)) {
                        tmp2.Add(south);
                    }

                    if (west != null && !squaresHit.Contains(west) && ((west.IsNotBlocking(side) && !ignoreBlocking) || ignoreBlocking)) {
                        tmp2.Add(west);
                    }

                    if (east != null && !squaresHit.Contains(east) && ((east.IsNotBlocking(side) && !ignoreBlocking) || ignoreBlocking)) {
                        tmp2.Add(east);
                    }
                }

                tmp1.Clear();
                tmp1.AddRange(tmp2);
                squaresHit.AddRange(tmp2);
                tmp2.Clear();
            }

            squaresHit.Remove(epicenter);

            return squaresHit;
        }
    }
}
