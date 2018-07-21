using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder {
    private Board board;
    private Node[,] nodes;
    private List<Node> closed = new List<Node>();
    private List<Node> opened = new List<Node>();
    private int maxSearchDistance;

    public PathFinder(Board board, int maxSearchDistance) {
        this.board = board;
        this.maxSearchDistance = maxSearchDistance;

        nodes = new Node[board.width, board.height];

        for (int x = 0; x < board.width; x++) {
            for (int y = 0; y < board.height; y++) {
                nodes[x, y] = new Node(x, y);
            }
        }
    }

    /**
     * @param sx Start location X
     * @param sy Start location Y
     * @param tx Target location X
     * @param ty Target location Y
     * @return The path
     */
    public Path FindPath(int sx, int sy, int tx, int ty, Side.Type side) {
        if (board.GetSquare(tx, ty) == null || board.GetSquare(tx, ty).solid) {
            return null;
        }

        nodes[sx, sy].cost = 0;
        nodes[sx, sy].depth = 0;
        closed.Clear();
        opened.Clear();
        opened.Add(nodes[sx, sy]);

        nodes[tx, ty].parent = null;

        int maxDepth = 0;

        while ((maxDepth < maxSearchDistance) && (opened.Count != 0)) {
            Node current = opened[0];

            if (current == nodes[tx, ty]) {
                break;
            }

            opened.Remove(current);
            closed.Add(current);

            for (int x = -1; x < 2; x++) {
                for (int y = -1; y < 2; y++) {
                    if ((x == 0) && (y == 0)) {
                        continue;
                    }

                    if ((x != 0) && (y != 0)) {
                        continue;
                    }

                    int xp = x + current.x;
                    int yp = y + current.y;

                    if (IsValidLocation(sx, sy, xp, yp, tx, ty, side)) {
                        float nextStepCost = current.cost + 1;
                        Node neighbour = nodes[xp, yp];
                        // map.pathFinderVisited(xp, yp);

                        if (nextStepCost < neighbour.cost) {
                            if (opened.Contains(neighbour)) {
                                opened.Remove(neighbour);
                            }

                            if (closed.Contains(neighbour)) {
                                closed.Remove(neighbour);
                            }
                        }

                        if (!opened.Contains(neighbour) && !(closed.Contains(neighbour))) {
                            neighbour.cost = nextStepCost;
                            neighbour.heuristic = getCost(xp, yp, tx, ty);
                            maxDepth = Mathf.Max(maxDepth, neighbour.SetParent(current));
                            opened.Add(neighbour);
                        }
                    }
                }
            }
        }

        if (nodes[tx, ty].parent == null) {
            return null;
        }

        Path path = new Path();
        Node target = nodes[tx, ty];

        while (target != nodes[sx, sy]) {
            // Don't add the targeted square if it is blocking
            if (target != nodes[tx, ty] || (target == nodes[tx, ty] && board.GetSquare(tx, ty).IsNotBlocking(side))) {
                path.PrependStep(board.GetSquare(target.x, target.y));
            }

            target = target.parent;
        }

        //path.PrependStep(board.GetSquare(sx, sy));

        return path;
    }

    /**
     * @param sx Start location X
     * @param sy Start location Y
     * @param x  Current location X
     * @param y  Current location Y
     * @param tx Target location X
     * @param ty Target location Y
     */
    private bool IsValidLocation(int sx, int sy, int x, int y, int tx, int ty, Side.Type side) {
        bool invalid = (x < 0) || (y < 0) || (x >= board.width) || (y >= board.height);

        if ((!invalid) && ((sx != x) || (sy != y))) {
            if (board.GetSquare(x, y) == null) {
                invalid = true;
            } else {
                if (x == tx && y == ty) {
                    invalid = false;
                } else {
                    invalid = !board.GetSquare(x, y).IsNotBlocking(side);
                }
            }
        }

        return !invalid;
    }

    private int getCost(int x, int y, int tx, int ty) {
        return Mathf.Abs(tx - x) + Mathf.Abs(ty - y);
    }

    private class Node : IComparable {
        public int x;
        public int y;
        public float cost;
        public Node parent;
        public float heuristic;
        public int depth;

        public Node(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public int SetParent(Node parent) {
            depth = parent.depth + 1;
            this.parent = parent;

            return depth;
        }

        public int CompareTo(object other) {
            Node o = (Node)other;

            float f = heuristic + cost;
            float of = o.heuristic + o.cost;

            if (f < of) {
                return -1;
            } else if (f > of) {
                return 1;
            } else {
                return 0;
            }
        }
    }
}
